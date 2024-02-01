using System;
using System.Linq;
using Inventory;
using Inventory.Models;
using Mirror;
using UnityEngine;

namespace StructurePlacement
{
    public class StructurePlacer : NetworkBehaviour
    {
        [Header("Infrastructure")] 
        [SerializeField] private ItemDatabaseSO _itemDatabase;
        [SerializeField] private int _structuresTilemapIndex;

        [Header("Build properties")] [SerializeField]
        private float _placeRadius;

        [SyncVar] private bool _structureCanBePlaced;
        public bool StructureCanBePlaced => _structureCanBePlaced;

        [SyncVar] private string _currentStructureId = string.Empty;

        public ItemSO TempItem => _tempStructureItem;
        private Action _removeItemFromInventory;

        private PlayerInventory _playerInventory;

        private GameObject _tempStructure;
        private Structure _tempStructureComponent;
        private StructureItemSO _tempStructureItem;
        private Vector3 _mousePosition;

        private Transform _structuresTilemap;
        private Grid _grid;
        private GameObject _structuresHolder;

        private bool StructureInBuildRadius =>
            Vector2.Distance(transform.position, _tempStructure.transform.position) <= _placeRadius;

        private void Start()
        {
            _grid = FindObjectOfType<Grid>();
            _playerInventory = GetComponent<PlayerInventory>();

            _structuresTilemap = _grid.transform.GetChild(_structuresTilemapIndex);
        }

        void Update()
        {
            if (!isOwned)
            {
                return;
            }

            _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            HandlePreviewStructurePosition();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelPlacement();
            }
        }

        //todo should be optimized in the future (item count maybe is expensive)
        private void HandlePreviewStructurePosition()
        {
            if (_tempStructure is null)
            {
                return;
            }
            
            _tempStructure.transform.position = _mousePosition;
            CalculateStructurePosition(_tempStructure.transform);
            
            _tempStructureComponent.ChangePlacementState(StructureInBuildRadius);
            var newState = _tempStructureComponent is not null
                           && _tempStructureComponent.CanBePlaced
                           && _playerInventory.Inventory.ItemCount(_tempStructureItem) > 0; 
            UpdateStructurePlaceState(newState);
        }

        [Command(requiresAuthority = false)]
        private void UpdateStructurePlaceState(bool newState)
        {
            _structureCanBePlaced = newState;
        }

        public void CancelPlacement()
        {
            CmdUpdateCurrentStructure(string.Empty);
            Destroy(_tempStructure);
            _tempStructure = null;
            _tempStructureItem = null;
        }

        public void SelectStructure(ItemSO item, Action afterPlacement)
        {
            if (item is not StructureItemSO structureItem)
            {
                return;
            }

            var structureId = structureItem.Id;

            if (_currentStructureId == structureId)
            {
                return;
            }

            if (_currentStructureId != string.Empty)
            {
                Destroy(_tempStructure);
            }

            CmdUpdateCurrentStructure(structureId);
            _tempStructure = Instantiate(structureItem.StructurePrefab, _structuresTilemap);
            _tempStructureComponent = _tempStructure.GetComponent<Structure>();
            _tempStructureItem = structureItem;
            _removeItemFromInventory = afterPlacement;
        }


        [Command(requiresAuthority = false)]
        private void CmdUpdateCurrentStructure(string structureId)
        {
            _currentStructureId = structureId;
        }

        private Vector3 CalculateStructurePosition(Transform structure)
        {
            var localPosition = structure.localPosition;
            Vector3Int cellPosition = _grid.LocalToCell(localPosition);
            localPosition = _grid.GetCellCenterLocal(cellPosition);
            structure.localPosition = localPosition;

            return localPosition;
        }

        [Command(requiresAuthority = false)]
        public void PlaceStructure(Vector3 mousePosition)
        {
            if (_currentStructureId == string.Empty)
            {
                return;
            }

            var structureItem = _itemDatabase.Items.FirstOrDefault(i => i.Id == _currentStructureId) as StructureItemSO;
            var structure = Instantiate(structureItem!.StructurePrefab);
            NetworkServer.Spawn(structure);

            structure.transform.position = mousePosition;
            var spawnPosition = CalculateStructurePosition(structure.transform);
            var component = structure.GetComponent<Structure>();
            component.SpawnPosition = spawnPosition;

            _removeItemFromInventory?.Invoke();
            InitStructureOnClients(component.netId);
        }

        [ClientRpc]
        private void InitStructureOnClients(uint structureId)
        {
            var component = FindObjectsOfType<Structure>().Where(component => component.netId == structureId).First();
            component.Init();
        }
    }
}
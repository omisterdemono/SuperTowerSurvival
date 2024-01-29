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
        [SerializeField] private GameObject[] _structures;
        [SerializeField] private int _structuresTilemapIndex;

        [Header("Build properties")] [SerializeField]
        private float _placeRadius;

        [SyncVar] private bool _structureCanBePlaced;
        public bool StructureCanBePlaced => _structureCanBePlaced;

        [SyncVar] private int _currentStructureIndex = -1;

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

        private void HandlePreviewStructurePosition()
        {
            if (_currentStructureIndex == -1 && _tempStructure is null)
            {
                return;
            }
            
            _tempStructure.transform.position = _mousePosition;
            CalculateStructurePosition(_tempStructure.transform);
            
            _tempStructureComponent.ChangePlacementState(StructureInBuildRadius);
            var newState = _tempStructureComponent is not null
                           && _tempStructureComponent.CanBePlaced
                           && _playerInventory.Inventory.ItemCount(_tempStructureItem) > 0; //.inventoryData.GetQuantityOfItem(_structureItems[_currentStructureIndex]) > 0;
            UpdateStructurePlaceState(newState);
        }

        [Command(requiresAuthority = false)]
        private void UpdateStructurePlaceState(bool newState)
        {
            _structureCanBePlaced = newState;
        }

        public void CancelPlacement()
        {
            CmdUpdateCurrentStructure(-1);
            Destroy(_tempStructure);
            _tempStructure = null;
            _tempStructureItem = null;
        }

        public void SelectStructure(ItemSO item, Action afterPlacement)
        {
            var structureItem = item as StructureItemSO;

            if (structureItem == null)
            {
                return;
            }

            var structureIndex = structureItem.Index;
            
            if (structureIndex < 0 || structureIndex >= _structures.Length)
            {
                throw new IndexOutOfRangeException($"Incorrect structure index: structureIndex == {structureIndex}");
            }

            if (_currentStructureIndex != -1 && _currentStructureIndex == structureIndex)
            {
                return;
            }

            if (_currentStructureIndex != -1)
            {
                Destroy(_tempStructure);
            }

            CmdUpdateCurrentStructure(structureIndex);
            _tempStructure = Instantiate(structureItem.StructurePrefab, _structuresTilemap);
            _tempStructureComponent = _tempStructure.GetComponent<Structure>();
            _tempStructureItem = structureItem;
            _removeItemFromInventory = afterPlacement;
        }


        [Command(requiresAuthority = false)]
        private void CmdUpdateCurrentStructure(int structureIndex)
        {
            _currentStructureIndex = structureIndex;
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
            if (_currentStructureIndex == -1)
            {
                return;
            }

            var structure = Instantiate(_structures[_currentStructureIndex]);
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
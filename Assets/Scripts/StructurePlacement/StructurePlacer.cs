using System;
using System.Linq;
using Config;
using Inventory;
using Inventory.Models;
using Mirror;
using Structures;
using UnityEngine;

namespace StructurePlacement
{
    public class StructurePlacer : NetworkBehaviour
    {
        [Header("Infrastructure")] [SerializeField]
        private ItemDatabaseSO _itemDatabase;

        [SerializeField] private int _structuresTilemapIndex;

        [Header("Build properties")] [SerializeField]
        private float _placeRadius;

        public bool StructureCanBePlaced => _structureCanBePlaced;
        public ItemSO TempItem => _tempStructureItem;
        public string CurrentStructureId => _currentStructureId;

        [SyncVar] private bool _structureCanBePlaced;
        [SyncVar] private string _currentStructureId = string.Empty;

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
            Vector2.Distance(transform.position, TempStructure.transform.position) <= _placeRadius;

        public GameObject TempStructure => _tempStructure;

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

            if (Input.GetKeyDown(GameConfig.CancelPlacementKeyCode))
            {
                CancelPlacement();
            }
        }

        //todo should be optimized in the future (item count maybe is expensive)
        private void HandlePreviewStructurePosition()
        {
            if (TempStructure is null)
            {
                return;
            }

            TempStructure.transform.position = _mousePosition;
            CalculateStructurePosition(TempStructure.transform);

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

            Destroy(TempStructure);
            _tempStructure = null;
            _tempStructureItem = null;
            _currentStructureId = string.Empty;
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
                Destroy(TempStructure);
            }

            CmdUpdateCurrentStructure(structureId);
            _tempStructure = Instantiate(structureItem.StructurePrefab, _structuresTilemap);
            _tempStructureComponent = TempStructure.GetComponent<Structure>();
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

            InitStructureOnClients(component.netId, netId);
        }

        [ClientRpc]
        private void InitStructureOnClients(uint structureId, uint playerId)
        {
            var structures = FindObjectsOfType<Structure>();
            var structureComponent = structures?.Where(component => component.netId == structureId).First();

            if (structureComponent == null)
            {
                throw new NullReferenceException($"Structure was not found by given id {structureId}");
            }

            structureComponent.Init();

            if (playerId == netId)
            {
                _playerInventory.Inventory.TryRemoveItem(TempItem, 1);
            }
        }
    }
}
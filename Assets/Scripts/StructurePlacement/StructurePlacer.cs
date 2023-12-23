using Assets.Scripts.Inventory;
using Inventory.Model;
using Mirror;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StructurePlacer : NetworkBehaviour
{
    [Header("Infrastructure")]
    [SerializeField] private GameObject[] _structures;
    [SerializeField] private ItemSO[] _structureItems;
    [SerializeField] private int _structuresTilemapIndex;

    [Header("UI")]
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private const string StructureHolderTagName = "StructuresHolder";
    private bool _isMenuOpened = true;

    [Header("Build properties")]
    [SerializeField] private float _placeRadius;

    [SyncVar] private bool _structureCanBePlaced;
    public bool StructureCanBePlaced => _structureCanBePlaced;

    [SyncVar] private int _currentStructureIndex = -1;

    private PlayerInventory _inventory;

    private GameObject _tempStructure;
    private Structure _tempStructureComponent;
    private Vector3 _mousePosition;

    private Transform _structuresTilemap;
    private Grid _grid;
    private GameObject _structuresHolder;


    private bool StructureInBuildRadius => Vector2.Distance(transform.position, _tempStructure.transform.position) <= _placeRadius;

    private void Start()
    {
        _grid = FindObjectOfType<Grid>();
        _inventory = GetComponent<PlayerInventory>();

        _structuresTilemap = _grid.transform.GetChild(_structuresTilemapIndex);

        if (isOwned)
        {
            InitUI();
        }
    }

    void Update()
    {
        if (!isOwned)
        {
            return;
        }

        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        HandlePreviewStructurePosition();

        if (Input.GetKeyDown(KeyCode.B))
        {
            _isMenuOpened = !_isMenuOpened;
            _structuresHolder.SetActive(_isMenuOpened);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
        }
    }

    private void HandlePreviewStructurePosition()
    {
        throw new NotImplementedException();

        //if (_currentStructureIndex != -1 && _tempStructure != null)
        //{
        //    _tempStructure.transform.position = _mousePosition;
        //    CalculateStructurePosition(_tempStructure.transform);

        //    _tempStructureComponent.ChangePlacementState(StructureInBuildRadius);
        //    var newState = _tempStructureComponent != null
        //        && _tempStructureComponent.CanBePlaced
        //        && _inventory.inventoryData.GetQuantityOfItem(_structureItems[_currentStructureIndex]) > 0;
        //    UpdateStructurePlaceState(newState);
        //}
    }

    [Command(requiresAuthority = false)]
    private void UpdateStructurePlaceState(bool newState)
    {
        _structureCanBePlaced = newState;
    }

    private void InitUI()
    {
        _structuresHolder = GameObject.FindGameObjectWithTag(StructureHolderTagName);

        for (int i = 0; i < _structures.Length; i++)
        {
            var objectOfButton = Instantiate(_buttonPrefab);
            objectOfButton.transform.SetParent(_structuresHolder.transform);
            objectOfButton.name = _structures[i].name;
            
            var tmpPro = objectOfButton.GetComponentInChildren<TextMeshProUGUI>();
            tmpPro.text = _structures[i].name;

            var button = objectOfButton.GetComponent<Button>();
            var structureIndex = i;

            button.onClick.AddListener(() =>  
            { 
                SelectStructure(structureIndex); 
            });
        }
    }

    public void CancelPlacement()
    {
        CmdUpdateCurrentStructure(-1);
        Destroy(_tempStructure);
        _tempStructure = null;
    }

    public void SelectStructure(int structureIndex)
    {
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
        _tempStructure = Instantiate(_structures[structureIndex], _structuresTilemap);
        _tempStructureComponent = _tempStructure.GetComponent<Structure>();
    }


    [Command(requiresAuthority = false)]
    private void CmdUpdateCurrentStructure(int structureIndex)
    {
        _currentStructureIndex = structureIndex;
    }

    private Vector3 CalculateStructurePosition(Transform structure)
    {
        Vector3Int cellPosition = _grid.LocalToCell(structure.localPosition);
        structure.localPosition = _grid.GetCellCenterLocal(cellPosition);

        return structure.localPosition;
    }

    [Command(requiresAuthority = false)]
    public void PlaceStructure(Vector3 mousePosition)
    {
        throw new NotImplementedException();

        //if (_currentStructureIndex == -1)
        //{
        //    return;
        //}

        //var structure = Instantiate(_structures[_currentStructureIndex]);
        //NetworkServer.Spawn(structure);

        //structure.transform.position = mousePosition;
        //var spawnPosition = CalculateStructurePosition(structure.transform);
        //var component = structure.GetComponent<Structure>();
        //component.SpawnPosition = spawnPosition;

        //_inventory.inventoryData.RemoveItem(_structureItems[_currentStructureIndex], 1);

        //InitStructureOnClients(component.netId);
    }

    [ClientRpc]
    private void InitStructureOnClients(uint structureId)
    {
        var component = FindObjectsOfType<Structure>().Where(component => component.netId == structureId).First();
        component.Init();
    }
}

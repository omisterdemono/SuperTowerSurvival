using Mirror;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StructurePlacer : NetworkBehaviour
{
    [SerializeField] private GameObject[] _structures;
    [SerializeField] private int _structuresTilemapIndex;

    [SerializeField] private GameObject _buttonPrefab;

    [SyncVar] private int _currentStructureIndex = -1;
    private GameObject _tempStructure;

    private Transform _structuresTilemap;
    private Grid _grid;

    private Vector3 _mousePosition;

    private void Start()
    {
        _grid = FindObjectOfType<Grid>();

        _structuresTilemap = _grid.transform.GetChild(_structuresTilemapIndex);

        if (isOwned)
        {
            InitUI();
        }
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (_currentStructureIndex != -1)
        {
            _tempStructure.transform.position = _mousePosition;
            CalculateStructurePosition(_tempStructure.transform);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            //command to open build menu
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _currentStructureIndex = -1;
        }
    }

    private void InitUI()
    {
        var structuresHolder = GameObject.FindGameObjectWithTag("StructuresHolder");

        for (int i = 0; i < _structures.Length; i++)
        {
            var objectOfButton = Instantiate(_buttonPrefab);
            objectOfButton.transform.SetParent(structuresHolder.transform);

            var button = objectOfButton.GetComponent<Button>();
            var structureIndex = i;

            button.onClick.AddListener(delegate { SelectStructure(structureIndex); });
            objectOfButton.name = $"{_structures[i].name} button";

            var tmpPro = objectOfButton.GetComponentInChildren<TextMeshProUGUI>();
            tmpPro.text = _structures[i].name;
        }
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
        if (_currentStructureIndex == -1)
        {
            return;
        }

        var structure = Instantiate(_structures[_currentStructureIndex]);
        NetworkServer.Spawn(structure);

        structure.transform.position = mousePosition;
        var spawnPosition = CalculateStructurePosition(structure.transform);
        structure.GetComponent<Structure>().SpawnPosition = spawnPosition;
    }

    //[ClientRpc]
    //public void PlaceStructureOnClient()
    //{
    //    structure.GetComponent<IBuildable>().Init();

    //}
}
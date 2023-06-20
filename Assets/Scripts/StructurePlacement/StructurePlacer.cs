using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StructurePlacer : NetworkBehaviour
{
    [SerializeField] private GameObject[] _structures;
    [SerializeField] private Transform _structuresTilemap;

    private Grid _grid;

    private Vector3 _mousePosition;
    
    private GameObject _currentStructure;
    private int _currentIndex;

    private void Awake()
    {
        _grid = FindObjectOfType<Grid>();
    }

    void Update()
    {
        if (!isOwned)
            return;

        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        if(_currentStructure != null)
        {
            CalculateStructurePosition();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject())
        {
            PlaceStructure();
            _currentStructure = Instantiate(_structures[_currentIndex], _structuresTilemap);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            //command to open build menu
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //command to close build menu
        }
    }

    public void SelectStructure(int structureIndex)
    {
        if(_currentStructure != null)
        {
            GameObject.Destroy(_currentStructure);
        }

        _currentIndex = structureIndex;
        _currentStructure = Instantiate(_structures[structureIndex], _structuresTilemap);
    }

    private void CalculateStructurePosition() 
    {
        _currentStructure.transform.position = _mousePosition;
        Vector3Int cellPosition = _grid.LocalToCell(_currentStructure.transform.localPosition);
        _currentStructure.transform.localPosition = _grid.GetCellCenterLocal(cellPosition);
    }

    [Command(requiresAuthority = false)]
    private void PlaceStructure()
    {
        NetworkServer.Spawn(_currentStructure);
        CalculateStructurePosition();

        _currentStructure.GetComponent<IBuildable>().Init();
    }
}

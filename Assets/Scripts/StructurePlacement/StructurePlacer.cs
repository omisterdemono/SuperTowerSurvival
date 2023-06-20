using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class StructurePlacer : MonoBehaviour
{
    [SerializeField] private GameObject[] _structures;
    [SerializeField] private int _structuresTilemapIndex;

    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private Transform _buildMenu;

    private Transform _structuresTilemap;
    private Grid _grid;

    private Vector3 _mousePosition;

    private GameObject _currentStructure;
    private int _currentIndex;

    private void Awake()
    {
        _grid = FindObjectOfType<Grid>();

        _structuresTilemap = _grid.transform.GetChild(_structuresTilemapIndex);
        InitUI();
    }

    void Update()
    {
        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (_currentStructure != null)
        {
            CalculateStructurePosition();
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

    private void InitUI()
    {
        for (int i = 0; i < _structures.Length; i++)
        {
            var objectOfButton = Instantiate(_buttonPrefab, _buildMenu);
            var button = objectOfButton.GetComponent<Button>();
            button.onClick.AddListener(() => { SelectStructure(i); });
            objectOfButton.GetComponent<Text>().text = $"{_structures[i].name} button";
        }
    }

    public void SelectStructure(int structureIndex)
    {
        if (_currentStructure != null && _currentIndex == structureIndex)
        {
            return;
        }

        if (_currentStructure != null)
        {
            GameObject.Destroy(_currentStructure);
        }

        _currentIndex = structureIndex;
        _currentStructure = Instantiate(_structures[_currentIndex], _structuresTilemap);
    }

    private void CalculateStructurePosition()
    {
        _currentStructure.transform.position = _mousePosition;
        Vector3Int cellPosition = _grid.LocalToCell(_currentStructure.transform.localPosition);
        _currentStructure.transform.localPosition = _grid.GetCellCenterLocal(cellPosition);
    }

    public void PlaceStructure()
    {
        if (_currentStructure == null)
        {
            return;
        }

        NetworkServer.Spawn(_currentStructure);
        _currentStructure.transform.parent = _structuresTilemap;

        CalculateStructurePosition();

        _currentStructure.GetComponent<IBuildable>().Init();
        _currentStructure = Instantiate(_structures[_currentIndex], _structuresTilemap);
    }
}

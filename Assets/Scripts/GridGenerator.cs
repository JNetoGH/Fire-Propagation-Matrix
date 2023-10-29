using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridGenerator : MonoBehaviour
{
    
    // Singleton Pattern
    public static GridGenerator Instance { get; private set; }
    
    private static int[,] numericalMatrix;
    private static GameObject[,] PrefabMatrix; 
  
    [Header("Grid Settings")]
    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private Vector2 _cellPadding;
    [SerializeField] private Transform _origin;
    
    [Header("Materials")]
    [SerializeField] private Material _none;
    [SerializeField] private Material _green;
    [SerializeField] private Material _fire;
    [SerializeField] private Material _burnt;
    
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CreateGameDependencies();
    }

    public void CreateGameDependencies()
    {
        foreach (Transform child in _origin)
            Destroy(child.gameObject);
        
        int totLines = UiSync.Instance.TotLines;
        int totColumns = UiSync.Instance.TotColumns;
        GenerateRandomNumericMatrix(
            UiSync.Instance.ProbabilityOfGrass,
            totLines <= 0 ? 15 : totLines, totColumns <= 0 ? 20: totColumns
            );
        
        InstantiatePrefabMatrix();
        
        GridController gridController = FindObjectOfType<GridController>();
        gridController.PrefabMatrix = PrefabMatrix;
        gridController.HasPrefabMatrixBeenSet = true;
        gridController.OnNewMatrixGenerated();
    }

    private static void GenerateRandomNumericMatrix(int probabilityOfGrass, int lines, int columns)
    {
        numericalMatrix = new int[lines, columns];
        for (int i = 0; i < lines; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                float rand = Random.Range(1, 101);
                numericalMatrix[i, j] = rand <= probabilityOfGrass ? 1 : 0;
            }
        }
    }

    private void InstantiatePrefabMatrix()
    {
        _origin.transform.rotation = Quaternion.identity;
        
        int lines = numericalMatrix.GetLength(0);
        int columns = numericalMatrix.GetLength(1);
        PrefabMatrix = new GameObject[lines, columns];
        
        Debug.Log($"Matrix [lines:{lines}, columns:{columns}] CREATED");
        
        float cellWidthWithPadding = _cellPrefab.transform.localScale.x + _cellPadding.x;
        float cellHeightWithPadding = _cellPrefab.transform.localScale.y + _cellPadding.y;

        for (int i = 0; i < lines; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // Instantiates.
                GameObject cell = Instantiate(_cellPrefab, _origin);
                
                // Adds it to the prefab matrix.
                PrefabMatrix[i, j] = cell;
                
                // Sets it state.
                CellController cellController = cell.GetComponent<CellController>();
                cellController.CellState = OriginalMatrixIndexToCellState(i, j);
                cellController.lineIndex = i;
                cellController.columnIndex = j;
                
                // Sets the world position.
                float xOffset = i * cellWidthWithPadding;
                float yOffset = -j * cellHeightWithPadding; // "-" makes it from top to bottom
                Vector3 newPos = new Vector3(_origin.position.x + xOffset, _origin.position.y + yOffset, _origin.position.z);
                cell.transform.position = newPos;
            }
        }

        // Sets adjacent of each cell
        for (int i = 0; i < lines; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GameObject cell = PrefabMatrix[i, j];
                CellController cellController = cell.GetComponent<CellController>();
                cellController.adjacentCells = new List<CellController>();

                // Top // Bottom // Left // Right
                if (j > 0) cellController.adjacentCells.Add(PrefabMatrix[i, j - 1].GetComponent<CellController>());
                if (j < columns - 1) cellController.adjacentCells.Add(PrefabMatrix[i, j + 1].GetComponent<CellController>());
                if (i > 0) cellController.adjacentCells.Add(PrefabMatrix[i - 1, j].GetComponent<CellController>());
                if (i < lines - 1) cellController.adjacentCells.Add(PrefabMatrix[i + 1, j].GetComponent<CellController>());
            }
        }

        _origin.transform.rotation = Quaternion.Euler(0, 0, 270);
    }
    
    public Material StateToMaterial(CellStateEnum cellState)
    {
        return cellState switch
        {
            CellStateEnum.None => _none,
            CellStateEnum.Green => _green,
            CellStateEnum.Fire => _fire,
            CellStateEnum.Burnt => _burnt,
            _ => _none
        };
    }
    
    public CellStateEnum OriginalMatrixIndexToCellState(int line, int column)
    {
        return numericalMatrix[line, column] switch
        {
            0 => CellStateEnum.None,
            1 => CellStateEnum.Green,
            2 => CellStateEnum.Fire,
            3 => CellStateEnum.Burnt,
            _ => CellStateEnum.None
        };
    }
    
}

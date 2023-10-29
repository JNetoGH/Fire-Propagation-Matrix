using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GridController : MonoBehaviour
{
    public GameObject[,] PrefabMatrix; 
    private List<CellController> _onFireCells = new List<CellController>();
    
    public bool HasPrefabMatrixBeenSet { get; set; } = false;
    private bool _hasFirstInputBeenMade = false;
    private float _updatesCounter = 0;
    
    public void OnNewMatrixGenerated()
    {
        _hasFirstInputBeenMade = false;
        _onFireCells.Clear();
        _updatesCounter = 0;
        CellController.propagationsCounter = 0;
        UpdatedCounters(false);
    }
    
    private void Update()
    {
        // Resets
        // if (Input.GetKeyDown(KeyCode.R))
        //     SceneManager.LoadScene(0);
        
        if (!HasPrefabMatrixBeenSet)
            return;
        
        if (!_hasFirstInputBeenMade)
        {
            _hasFirstInputBeenMade = Input.GetKeyDown(KeyCode.F);
            if (!_hasFirstInputBeenMade)
                return;
            FirstUpdateOnCells();
            return;
        }

        bool anyCellsOnFire = _onFireCells.Count == 0;
        if (anyCellsOnFire )
            return;
        
        bool HasUpdate = Input.GetKeyDown(KeyCode.F);
        if (!HasUpdate)
            return;

        UpdateCells();
    }
    
    private void FirstUpdateOnCells()
    {
        // sets all the cell in the first line to be red
        Debug.Log("Cells Had Their First Update");
        
        int columns = PrefabMatrix.GetLength(1);
        for (int i = 0; i < columns; i++)
        {
            // if it's green goes to fire, if it's any other, just keeps it this way.
            CellController cellController = PrefabMatrix[0, i].GetComponent<CellController>();
            cellController.CellState = cellController.CellState == CellStateEnum.Green ? CellStateEnum.Fire : cellController.CellState;
            cellController.CellState = cellController.CellState == CellStateEnum.Green ? CellStateEnum.Fire : cellController.CellState;
            if (cellController.CellState == CellStateEnum.Fire)
            {
                _onFireCells.Add(cellController);
                CellController.propagationsCounter++;
            }
        }
        
        UpdatedCounters();
        
        // Start of first cell
        // PrefabMatrix[0, 0].GetComponent<CellController>().CellState = CellStateEnum.Fire;
        // _onFireCells.Add(PrefabMatrix[0, 0].GetComponent<CellController>());
    }
    
    private void UpdateCells()
    {
        int lines = PrefabMatrix.GetLength(0);
        int columns = PrefabMatrix.GetLength(1);
        
        // propagates and burns the cell in the list
        foreach (CellController cellController in _onFireCells)
        {
            cellController.TryPropagateToAdjacentCells();
            cellController.BurnCell();
        }
        
        // Clears and finds the new onFireCells
        _onFireCells.Clear();
        for (int i = 0; i < lines; i++)
            for (int j = 0; j < columns; j++)
                if (PrefabMatrix[i, j].GetComponent<CellController>().CellState == CellStateEnum.Fire)
                    _onFireCells.Add(PrefabMatrix[i, j].GetComponent<CellController>());

        UpdatedCounters();
    }

    private void UpdatedCounters(bool incrementUpdates = true)
    {
        if(incrementUpdates)
            _updatesCounter++;
        UiSync.Instance.updatesCounterTxt.text = _updatesCounter.ToString();
        UiSync.Instance.propagationsCounterTxt.text = CellController.propagationsCounter.ToString();
    }
    
}

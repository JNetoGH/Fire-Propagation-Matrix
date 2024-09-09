using System.Collections.Generic;
using UnityEngine;


public class CellController : MonoBehaviour
{
    
    public static int propagationsCounter = 0;
    
    [SerializeField] private CellStateEnum _cellState;
    [SerializeField] public int lineIndex;
    [SerializeField] public int columnIndex;
    [SerializeField] public List<CellController> adjacentCells;
    private Renderer _renderer;
    
    /// <summary>
    /// Sets the state and the material at once
    /// </summary>
    public CellStateEnum CellState
    {
        get => _cellState;
        set
        {
            _cellState = value;
            _renderer.material = GridGenerator.Instance.StateToMaterial(value);
        }
    }

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }
    
    public void BurnCell() => CellState = CellStateEnum.Burnt;
    
    public void TryPropagateToAdjacentCells()
    {
        foreach (CellController adjacentCell in adjacentCells)
        {
            if (adjacentCell.CellState == CellStateEnum.Green)
            {
                adjacentCell.CellState = CellStateEnum.Fire;
                propagationsCounter++;
            }
        }
    }
    
}

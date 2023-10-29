using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UiSync : MonoBehaviour
{

    // Singleton pattern.
    public static UiSync Instance { get; private set;  }
    public int TotLines => Convert.ToInt32(_linesSlider.value);
    public int TotColumns => Convert.ToInt32(_columnsSlider.value);
    public int ProbabilityOfGrass => Convert.ToInt32(_probabilityOfGrassSlider.value);
    
    [Header("Counters")]
    [SerializeField] public TextMeshProUGUI updatesCounterTxt;
    [SerializeField] public TextMeshProUGUI propagationsCounterTxt;
    
    [Header("Lines")] 
    [SerializeField] private TextMeshProUGUI _linesValue;
    [SerializeField] private Slider _linesSlider;
    
    [Header("Columns")] 
    [SerializeField] private TextMeshProUGUI _columnsValue;
    [SerializeField] private Slider _columnsSlider;
    
    [Header("Probability Of Grass")]
    [SerializeField] private TextMeshProUGUI _probabilityOfGrassValue;
    [SerializeField] private Slider _probabilityOfGrassSlider;
    
    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        _linesValue.text = _linesSlider.value.ToString();
        _columnsValue.text = _columnsSlider.value.ToString();
        _probabilityOfGrassValue.text = _probabilityOfGrassSlider.value.ToString();
    }
}

using com.FlowFree;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameProgress : MonoBehaviour
{
    public int _FlowCompleted = 0;

    public float _TotalFlow = 0;

    public float _Progress = 0;

    [SerializeField] private TextMeshProUGUI _FlowText;

    [SerializeField] private TextMeshProUGUI _ProgressText;

    [SerializeField] private LevelData m_LevelData;


    private void Start()
    {
        _TotalFlow = m_LevelData._CirclePositions.Count / 2;
        ShowProgress();
    }

    public void ShowProgress()
    {
        _FlowText.text = "Flows: "+_FlowCompleted.ToString()+"/"+_TotalFlow.ToString();

        if(_FlowCompleted == 0)
        {
            _ProgressText.text = "Pipe: " + 0 + "%";
            return;
        }
        _Progress = (_FlowCompleted % _TotalFlow) * 100;
        _ProgressText.text = "Pipe: "+ ((_FlowCompleted / _TotalFlow) * 100) + "%";

    }
}

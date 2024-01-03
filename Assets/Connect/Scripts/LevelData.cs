using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace com.FlowFree
{
    [CreateAssetMenu(fileName = "LevelData",menuName = "FlowFree/LevelData")]
    public class LevelData : ScriptableObject
    {

        public GridData _GridData;

        public List<CirclePos> _CirclePositions;


    }
    [Serializable]
    public class GridData
    {
        public int _Row;
        public int _Col;
    }

    [Serializable]
    public class CirclePos
    {
        public GameObject _Point;
        public GameObject m_Anim;
        public int _CellOnRow;
        public int _CellOnCol;
    }
}
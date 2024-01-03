using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace com.FlowFree
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private LevelData m_LevelData;
        [SerializeField] private GameObject m_Block;

        [SerializeField] private GameObject m_Text;

        [SerializeField] private Transform m_LevelParent;

        [SerializeField] private Level m_Level;

        public int[,] _GridArray;

        public List<Tile> _Tiles;



        // Start is called before the first frame update
        void Start()
        {
            _GridArray = new int[m_LevelData._GridData._Col, m_LevelData._GridData._Row];
            m_LevelParent = transform;
            MakeGrid();
            for (int i = 0; i < m_LevelData._CirclePositions.Count; i++)
            {
                PlaceCircels(m_LevelData._CirclePositions[i]._Point, m_LevelData._CirclePositions[i].m_Anim, m_LevelData._CirclePositions[i]._CellOnRow, m_LevelData._CirclePositions[i]._CellOnCol);
            }
            AddTargetTileCircle();
        }

        //to make grid 
        public void MakeGrid()
        {
            for (int i = 0; i < _GridArray.GetLength(0); i++)
            {
                for (int j = 0; j < _GridArray.GetLength(1); j++)
                {
                    float xPos = i;
                    float yPos = _GridArray.GetLength(1) - 1 - j;

                    GameObject block = Instantiate(m_Block) as GameObject;
                    block.transform.position = new Vector3(xPos, yPos, 0.4f);
                    block.transform.parent = m_LevelParent;
                    block.transform.name = "("+i+","+j+")";
                    block.transform.GetComponent<Tile>()._TileData._Row = i;
                    block.transform.GetComponent<Tile>()._TileData._Col = j;
                    _Tiles.Add(block.transform.GetComponent<Tile>());
                    //GameObject text = Instantiate(m_Text) as GameObject;
                    //text.transform.position = new Vector3(xPos, yPos, 0.4f);
                    //text.GetComponent<TextMeshPro>().text = j + "," + i;

                }
            }
        }

        //to fix the cricles
        public void PlaceCircels(GameObject point,GameObject anim,int row,int col)
        {
            for (int i = 0; i < _GridArray.GetLength(0); i++)
            {
                for (int j = 0; j < _GridArray.GetLength(1); j++)
                {
                    if(row == j && col == i)
                    {
                        float xPos = i;
                        float yPos = _GridArray.GetLength(1) - 1 - j;
                        GameObject block = Instantiate(point.gameObject) as GameObject;
                        block.transform.position = new Vector3(xPos, yPos, 0.4f);
                        block.transform.parent = m_LevelParent;

                        GameObject circleanim = Instantiate(anim.gameObject) as GameObject;
                        circleanim.transform.position = new Vector3(xPos, yPos, 0.4f);
                        circleanim.transform.parent = m_LevelParent;
                        circleanim.transform.gameObject.SetActive(false);

                        string name = "("+i +","+j+")";
                        Tile tile = GameObject.Find(name)?.GetComponent<Tile>();
                        tile._CircleAnim = circleanim;
                        m_Level._TileWithCircles.Add(tile);
                        if (tile != null)
                        {
                            tile._Circle = point; 
                        }
                    }
                    
                }
            }
        }

        
        //to add target circle
        public void AddTargetTileCircle()
        {
            for(int i = 0; i < m_Level._TileWithCircles.Count; i++) 
            {
                for (int j = i+1;j< m_Level._TileWithCircles.Count; j++)
                {
                    if (m_Level._TileWithCircles[i]._Circle == m_Level._TileWithCircles[j]._Circle)
                    {
                        m_Level._TileWithCircles[i]._TargetTiles.Add(m_Level._TileWithCircles[j]);
                        m_Level._TileWithCircles[j]._TargetTiles.Add(m_Level._TileWithCircles[i]);
                    }
                }
            }
        }
    }
}


using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.FlowFree
{
    public class Gameplay : MonoBehaviour
    {
        private List<Vector3> points = new List<Vector3>();
        bool m_DrawLine = false;
        public GameObject _LevelParent;

        [SerializeField] GameProgress m_GameProgress;

        [Header("Mouse Position Props")]
        private Vector3 initialMousePos;
        private Vector3 finalMousePos;

        [Header("Tile Props")]
        public Tile _CurrentTile;
        public GameObject _CurrentCircle;
        public Color _CurrentColor;
        public Tile _TileStart;
        public GameObject _FadedTile;

        [Header("Pipe Props")]
        [SerializeField] private GameObject m_Pipe;
        private Dictionary<DragDir, float> m_Angles = new Dictionary<DragDir, float>()
        {
            { DragDir.Up, 0f },
            { DragDir.Down, 180f },
            { DragDir.Right, 90f },
            { DragDir.Left, -90f }
        };
        private bool m_MakePipe = false;
        PipePath path;

        [Header("Events")]
        public static Action _LevelCompleteEvent;

        private void Start()
        {
            Tile._Event += Draw;
        }
        #region Draw Pipe
        public void Draw(string EventName,PointerEventData eventData)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            initialMousePos = Input.mousePosition;

            // Create a 2D ray from the mouse position
            Ray2D ray = new Ray2D(mousePosition, Vector2.zero);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null)
            {
                if (EventName == "PointerDown")
                {
                    if (hit.collider.transform.GetComponent<Tile>()._Circle || hit.collider.transform.GetComponent<Tile>()._HasPipe)
                    {
                         m_MakePipe = true;
                        _CurrentTile = hit.collider.transform.GetComponent<Tile>();


                        if (_TileStart != null)
                        {
                            if (hit.collider.transform.GetComponent<Tile>()._Circle)
                            {
                                _TileStart = null;
                                _TileStart = hit.collider.transform.GetComponent<Tile>();
                            }
                        }
                        else
                        {
                            if (hit.collider.transform.GetComponent<Tile>()._Circle)
                            {
                                _TileStart = hit.collider.transform.GetComponent<Tile>();
                            }
                        }

                        if (hit.collider.transform.GetComponent<Tile>()._Circle)
                        {
                            if(m_GameProgress._FlowCompleted > 0 && hit.collider.transform.GetComponent<Tile>()._CirclesConnected)
                            {
                                m_GameProgress._FlowCompleted -= 1;
                                m_GameProgress.ShowProgress();
                            }
                            
                            hit.collider.transform.GetComponent<Tile>().DeletePipePath();
                            hit.collider.transform.GetComponent<Tile>().DeleteHighlightedTiles();

                            hit.collider.transform.GetComponent<Tile>().DeleteTargetTilePipePath();
                            hit.collider.transform.GetComponent<Tile>().DeleteTargetHighlightedTiles();

                            hit.collider.transform.GetComponent<Tile>().Animate();
                            _CurrentCircle = hit.collider.transform.GetComponent<Tile>()._Circle;
                            _CurrentColor = hit.collider.transform.GetComponent<Tile>()._Circle.GetComponent<SpriteRenderer>().color;
                        }
                        AddTileHighlight();

                    }
                    else
                    {
                         m_MakePipe= false;
                    }
                }
                else if(EventName == "Drag")
                {
                    if (m_MakePipe)
                    {
                        if (IsValidNeighbour(hit.collider.gameObject.name) && !hit.collider.transform.GetComponent<Tile>()._HasPipe)
                        {
                            _CurrentTile._NextTile = hit.transform.GetComponent<Tile>();
                            hit.collider.transform.GetComponent<Tile>()._PreviousTile = _CurrentTile.transform.GetComponent<Tile>();

                           // to let make pipe if same color circle will be otherwise will block
                            if ((_CurrentTile._NextTile._Circle != null && _CurrentCircle != _CurrentTile._NextTile._Circle))
                            {
                                return;
                            }
                            MakePipe(eventData, hit);

                        }
                    }
                    
                }
            }
            else
            {
                print("none");
            }

        }

        //to add Tile highlight
        public void AddTileHighlight()
        {
            GameObject fadedtile = Instantiate(_FadedTile);
            fadedtile.transform.position = _CurrentTile.transform.position;
            var tempColor = fadedtile.transform.GetComponent<SpriteRenderer>().color;
            tempColor = _CurrentColor;
            tempColor.a = 0.2f;
            fadedtile.transform.GetComponent<SpriteRenderer>().color = tempColor;
            fadedtile.transform.parent = _LevelParent.transform;
            fadedtile.transform.gameObject.SetActive(false);
            if (fadedtile)
            {
                _TileStart._TileHighlight.Add(fadedtile);
            }
        }

        //show direction
        public void MakePipe(PointerEventData eventData,RaycastHit2D hit)
        {
            DragDir dir = GetDragDirection(eventData);
            Vector3 pos = hit.collider.transform.localPosition;
            float angle = m_Angles[dir];

            GameObject pipe = Instantiate(m_Pipe);

            _CurrentTile = hit.collider.transform.GetComponent<Tile>();

            path = new PipePath(_CurrentTile, pipe);

            _TileStart._PipePath.Add(path);

            pipe.transform.parent = _LevelParent.transform;
            pipe.GetComponent<SpriteRenderer>().color = _CurrentColor;
            pipe.transform.localRotation = Quaternion.Euler(new Vector3(0,0,angle));
            pipe.transform.localPosition = new Vector3(pos.x, _CurrentTile._PreviousTile.transform.localPosition.y, pos.z);

            hit.collider.transform.GetComponent<Tile>()._HasPipe = true;
            AddTileHighlight();
            _TileStart.TargetTilesConnected(_CurrentTile);
            if (_TileStart._ConnectedTiles == _TileStart._TargetTiles.Count)
            {
                _TileStart._CirclesConnected = true;
                m_GameProgress._FlowCompleted += 1;
                m_GameProgress.ShowProgress();
                foreach (Tile targettile in _TileStart._TargetTiles)
                {
                    m_MakePipe = false;
                    targettile._TileHighlight = _TileStart._TileHighlight;
                    targettile._CirclesConnected = true;
                    targettile._PipePath = _TileStart._PipePath;
                }
                _TileStart.ShowHighlightedTile();
                _LevelCompleteEvent.Invoke();
            }
        }
        #endregion

        #region Find neigbours and directions
        //to check  valid neighbour n_CurrentTile
        public bool IsValidNeighbour(string name)
        {
            if(name == "("+_CurrentTile._TileData._Row+","+(_CurrentTile._TileData._Col-1)+")" || 
               name == "("+_CurrentTile._TileData._Row+ ","+(_CurrentTile._TileData._Col + 1) + ")" ||
               name == "("+(_CurrentTile._TileData._Row - 1)+ "," +_CurrentTile._TileData._Col+ ")" || 
               name == "("+(_CurrentTile._TileData._Row + 1)+ "," +_CurrentTile._TileData._Col+ ")")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //to get which direction we are dragging
        public DragDir GetDragDirection(PointerEventData eventData)
        {
            if (eventData.delta.magnitude > 0)
            {
                // Get the direction of the drag
                Vector2 dragDirection = eventData.delta.normalized;

                // Determine the dominant direction (horizontal or vertical)
                if (Mathf.Abs(dragDirection.x) > Mathf.Abs(dragDirection.y))
                {
                    // Horizontal drag
                    if (dragDirection.x > 0)
                    {
                        return DragDir.Right;
                    }
                    else
                    {
                        return DragDir.Left;
                    }
                }
                else
                {
                    // Vertical drag
                    if (dragDirection.y > 0)
                    {
                        return DragDir.Up;
                    }
                    else
                    {
                        return DragDir.Down;
                    }
                }
            }
            return DragDir.Up;
        }
        #endregion
    }

    public enum DragDir
    {
        Left,
        Right,
        Up,
        Down
    }

}

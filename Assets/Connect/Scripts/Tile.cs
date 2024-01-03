using System;
using System.Collections;
using System.Collections.Generic;


namespace com.FlowFree
{
    public class Tile : MonoBehaviour, IPointerDownHandler, IDragHandler
    {

        [Header("Tile Props")]
        public GameObject _Circle;
        public Tile _PreviousTile;
        public Tile _NextTile;

        public TileData _TileData;
        public bool _HasPipe = false;
        public List<PipePath> _PipePath;
        public GameObject _Pipe;

        public List<GameObject> _TileHighlight;

        public bool _CirclesConnected = false;

        public List<Tile> _TargetTiles;

        public GameObject _CircleAnim;

        public int _ConnectedTiles = 0;

        [Header("Event Props")]
        public static Action<string, PointerEventData> _Event;

        public void OnPointerDown(PointerEventData eventData)
        {
            _Event.Invoke("PointerDown", eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _Event.Invoke("Drag", eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _Event.Invoke("PointerUp", eventData);
        }

        //to check connected on target tiles
        public void TargetTilesConnected(Tile tile)
        {
            if (_TargetTiles.Contains(tile))
            {
                _ConnectedTiles++;
            }
        }

        //to delete if pipes already made from tile
        public void DeletePipePath()
        {
            foreach (PipePath obj in _PipePath)
            {
                Destroy(obj._Pipe);
                obj._tile._HasPipe = false;
            }
            _PipePath.Clear();
        }
        //to delete highlighted tiles
        public void DeleteHighlightedTiles()
        {
            foreach (GameObject higlight in _TileHighlight)
            {
                Destroy(higlight);
            }
            _TileHighlight.Clear();
            _ConnectedTiles = 0;
            _CirclesConnected = false;
        }

        //to delete target tile pipe path
        public void DeleteTargetTilePipePath()
        {
            foreach (Tile tile in _TargetTiles)
            {
                tile.DeletePipePath();
            }
        }

        //to delete target tile highlighted tiles
        public void DeleteTargetHighlightedTiles()
        {
            foreach (Tile tile in _TargetTiles)
            {
                tile.DeleteHighlightedTiles();
            }

        }



        //to show highlighted tile
        public void ShowHighlightedTile()
        {
            foreach (GameObject higlight in _TileHighlight)
            {
                higlight.SetActive(true);
            }
        }

        //to delete pipe of particular tile
        public void DeleteTilePipe()
        {

        }

        //to animate circle
        public void Animate()
        {
            StartCoroutine(AnimateCircle());
        }

        public IEnumerator AnimateCircle()
        {
            this._Circle.SetActive(false);
            this._CircleAnim.SetActive(true);
            foreach (Tile targettile in _TargetTiles)
            {
                targettile._Circle.SetActive(false);
                targettile._CircleAnim.SetActive(true);
            }

            yield return new WaitForSeconds(0.55f);
            this._Circle.SetActive(true);
            this._CircleAnim.SetActive(false);

            foreach (Tile targettile in _TargetTiles)
            {
                targettile._Circle.SetActive(true);
                targettile._CircleAnim.SetActive(false);
            }

        }


    }
    //Tile Data
    [Serializable]
    public class TileData
    {
        public int _Row;
        public int _Col;
    }

    [Serializable]
    public class PipePath
    {
        public Tile _tile;
        public GameObject _Pipe;


        public PipePath(Tile tile, GameObject pipe)
        {
            _tile = tile;
            _Pipe = pipe;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace com.FlowFree
{
    public class Level : MonoBehaviour
    {
        public  List<Tile> _TileWithCircles;

        public bool _LevelCompleted = false;

        [SerializeField] private GameObject _LevelComplete;

        [SerializeField] private GameProgress m_Progress;

        private void Start()
        {
            Gameplay._LevelCompleteEvent += LevelCompleted;
        }

        public void LevelCompleted()
        {
            foreach (var tile in _TileWithCircles)
            {
                if (tile != null)
                {
                    if (tile._CirclesConnected == false)
                    {
                        return;
                    }

                }
            }
           StartCoroutine(ShowLevelComplete());
        }

        private IEnumerator ShowLevelComplete()
        {
            yield return new WaitForSeconds(0.6f);
            _LevelCompleted = true;
            _LevelComplete.SetActive(true);
        }

        //reset game
        public void ResetGame()
        {
            foreach (var tile in _TileWithCircles)
            {
                tile.DeletePipePath();
                tile.DeleteHighlightedTiles();

                tile.DeleteTargetTilePipePath();
                tile.DeleteTargetHighlightedTiles();
                m_Progress._FlowCompleted = 0;
                m_Progress.ShowProgress();
            }
        }
    }
}

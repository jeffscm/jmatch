using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JMatch.Services
{
    public class DetectorService : MonoBehaviour
    {

        public static Action<Vector2> OnDestroyTile;
        public static Action<int> OnDestroyTileFinished;

        public Transform detectorTilePrefab;
        public Transform detectorTileBase;
        List<Transform> listTiles;
        List<Transform> _listDetected = new List<Transform>();

        public void PopulateTiles()
        {
            listTiles = new List<Transform>();
            for (int i = 0; i < 20; i++)
            {
                var temp = Instantiate(detectorTilePrefab, detectorTileBase) as Transform;
                temp.gameObject.SetActive(false);
                listTiles.Add(temp);
            }
        }

        public void DetectNew(Transform newTileDetected)
        {
            if (newTileDetected == null)
            {
                //check for destruction
                if (_listDetected.Count >= 3)
                {
                    foreach (var item in _listDetected)
                    {
                        OnDestroyTile?.Invoke(item.position);
                    }
                    OnDestroyTileFinished?.Invoke(_listDetected.Count);
                }
                ResetTiles();
            }
            else
            {
                //add new tile
                var temp = GetTile();
                temp.position = newTileDetected.position;
                _listDetected.Add(temp);
            }
        }

        private Transform GetTile()
        {
            foreach (var item in listTiles)
            {
                if (!item.gameObject.activeInHierarchy)
                {
                    item.gameObject.SetActive(true);
                    return item;
                }
            }
            return null;
        }

        public void ResetTiles()
        {
            _listDetected = new List<Transform>();
            foreach (var item in listTiles)
                item.gameObject.SetActive(false);
        }
    }
}
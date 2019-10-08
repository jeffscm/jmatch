using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JMatch.Services
{
    public class GridService : MonoBehaviour
    {
        public static float StepGrid
        {
            get
            {
                return _stepGrid;
            }
        }

        static readonly float _stepGrid = 0.75f;
        static int _currentX, _currentY;

        public Transform gridBase;

        public List<Transform> tilesList;

        public List<Transform> tilePrefabs;

        Transform[,] mapGrid;


        #region Grid Public Calls        

        public void ResetGrid(int x, int y)
        {
            RemoveAllTiles();
            _currentX = x;
            _currentY = y;

            mapGrid = new Transform[x, y];

            for (var i = 0; i < x; i++)
            {
                for (var j = 0; j < y; j++)
                {
                    var temp = GetTile();

                    float startX = -((x / 2f) - 0.5f) + i;
                    float startY = ((y / 2f) - 0.5f) - j;

                    temp.position = new Vector3(startX, startY, 0) * _stepGrid;
                    mapGrid[i, j] = temp;
                }
            }
        }

        public void PopulateTiles()
        {
            for (int i = 0; i < 150; i++)
            {
                var temp = Instantiate(tilePrefabs[UnityEngine.Random.Range(0, tilePrefabs.Count)], gridBase) as Transform;
                temp.gameObject.SetActive(false);
                tilesList.Add(temp);
            }
        }

        public void RemoveAllTiles()
        {
            foreach (var item in tilesList)
            {
                item.gameObject.SetActive(false);
            }
        }

        private Transform GetTile()
        {

            bool IsActive(Transform t)
            {
                bool result = false;

                if (!t.gameObject.activeInHierarchy)
                {
                    result = true;
                    t.gameObject.SetActive(true);
                }
                return result;
            }

            var isForward = UnityEngine.Random.Range(0, 10) > 5;
            var random = UnityEngine.Random.Range(0, tilesList.Count / 2);
            if (isForward)
            {
                for (int i = random; i < tilesList.Count; i++)
                {
                    if (IsActive(tilesList[i]))
                        return tilesList[i];
                }
            }
            else
            {
                for (int i = tilesList.Count - 1; i >= random; i--)
                {
                    if (IsActive(tilesList[i]))
                        return tilesList[i];
                }
            }

            var temp = Instantiate(tilePrefabs[UnityEngine.Random.Range(0, tilePrefabs.Count)], gridBase) as Transform;
            temp.gameObject.SetActive(true);
            tilesList.Add(temp);

            return temp;
        }

        public void DestroyTile(Vector2 pos)
        {
            var matrixPos = ConvertFromTransform(pos);
            StartCoroutine(AnimateDestroy(mapGrid[matrixPos.Item1, matrixPos.Item2]));
            mapGrid[matrixPos.Item1, matrixPos.Item2] = null;
        }

        IEnumerator AnimateDestroy(Transform obj)
        {
            obj.GetChild(0).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            obj.GetChild(0).gameObject.SetActive(false);
            obj.gameObject.SetActive(false);
        }

        #endregion

        #region Helper Public Methods

        public bool ValidateLayer(int layerId, Transform obj)
        {
            var matrixPos = ConvertFromTransform(obj.position);

            var tempX = matrixPos.Item1;
            var tempY = matrixPos.Item2;

            bool result = false;

            if (tempX > 0) result = (mapGrid[tempX - 1, tempY].gameObject.layer == layerId) || result;
            if (tempX < _currentX - 1) result = (mapGrid[tempX + 1, tempY].gameObject.layer == layerId) || result;
            if (tempY > 0) result = (mapGrid[tempX, tempY - 1].gameObject.layer == layerId) || result;
            if (tempY < _currentY - 1) result = (mapGrid[tempX, tempY + 1].gameObject.layer == layerId) || result;

            if (tempX > 0 && tempY > 0) result = (mapGrid[tempX - 1, tempY - 1].gameObject.layer == layerId) || result;
            if (tempX > 0 && tempY < _currentY - 1) result = (mapGrid[tempX - 1, tempY + 1].gameObject.layer == layerId) || result;
            if (tempX < _currentX - 1 && tempY > 0) result = (mapGrid[tempX + 1, tempY - 1].gameObject.layer == layerId) || result;
            if (tempX < _currentX - 1 && tempY < _currentY - 1) result = (mapGrid[tempX + 1, tempY + 1].gameObject.layer == layerId) || result;

            return result;
        }

        public List<Transform> GetAllSameColor(Transform target)
        {
            var matrixPos = ConvertFromTransform(target.position);
            var tempLayer = mapGrid[matrixPos.Item1, matrixPos.Item2].gameObject.layer;
            var result = new List<Transform>();
            foreach(var item in mapGrid)
            {
                if (item != null && item.gameObject.layer == tempLayer)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public List<Transform> GetSameRow(Transform target)
        {
            var matrixPos = ConvertFromTransform(target.position);
            var tempLayer = mapGrid[matrixPos.Item1, matrixPos.Item2].gameObject.layer;
            var result = new List<Transform>();
            for(int i = 0; i < _currentX; i++)
            {
                if (mapGrid[i, matrixPos.Item2] != null)
                {
                    result.Add(mapGrid[i, matrixPos.Item2]);
                }
            }
            return result;
        }

        public List<Transform> GetSameCol(Transform target)
        {
            var matrixPos = ConvertFromTransform(target.position);
            var tempLayer = mapGrid[matrixPos.Item1, matrixPos.Item2].gameObject.layer;
            var result = new List<Transform>();
            for (int i = 0; i < _currentY; i++)
            {
                if (mapGrid[matrixPos.Item1, i] != null)
                {
                    result.Add(mapGrid[matrixPos.Item1, i]);
                }
            }
            return result;
        }

        public static Tuple<int, int> ConvertFromTransform(Vector2 pos)
        {
            float deltaX = (((_currentX / 2f) - 0.5f) * _stepGrid + pos.x) / _stepGrid;
            int x = Mathf.FloorToInt(deltaX);

            float deltaY = (((_currentY / 2f) - 0.5f) * _stepGrid - pos.y) / _stepGrid;
            int y = Mathf.FloorToInt(deltaY);
            
            return Tuple.Create(x, y);
        }

        public void CheckGrid()
        {
            MoveGridDown(onlyIfNull: true);
        }

        #endregion

        #region Private Calls

        private void MoveGridDown(bool onlyIfNull = false)
        {
            void GetNext(int nextX, int nextY)
            {
                for (int getNextY = nextY - 1; getNextY >= 0; getNextY--)
                {
                    if (mapGrid[nextX, getNextY] == null)
                    {
                        if (getNextY == 0)
                        {
                            break;
                        }                        

                        for (int tempNextY = getNextY - 1; tempNextY >= 0; tempNextY--)
                        {
                            if (mapGrid[nextX, tempNextY] != null)
                            {
                                mapGrid[nextX, getNextY] = mapGrid[nextX, tempNextY];
                                MoveOneTileDownXY(mapGrid[nextX, getNextY], getNextY);
                                mapGrid[nextX, tempNextY] = null;
                                break;
                            }
                        }
                    }
                }
            }

            for (int x = 0; x < _currentX; x++)
            {
                int added = 0;
                GetNext(x, _currentY);
                for (int y = _currentY - 1; y >= 0; y--)
                {
                    if (onlyIfNull)
                    {
                        if (mapGrid[x, y] == null)
                        {
                            added++;
                            mapGrid[x, y] = GetTileForPosition(x, y, added);
                        }
                    }
                    else
                    {
                        MoveOneTileDown(mapGrid[x, y]);
                    }
                }
            }
        }

        private void MoveOneTileDownXY(Transform tile, int newY)
        {
            if (tile != null)
            {                
                float startY = (_currentY / 2f) - 0.5f;
                LeanTween.cancel(tile.gameObject);
                var pos = tile.position;
                pos.y = _stepGrid * (startY - newY);
                LeanTween.move(tile.gameObject, pos, 0.25f);
            }
        }

        private void MoveOneTileDown(Transform tile)
        {
            if (tile != null)
            {
                var pos = tile.position;
                pos.y -= _stepGrid;
                LeanTween.move(tile.gameObject, pos, 0.25f);
            }
        }

        private Transform GetTileForPosition(int x, int y, int added)
        {
            float startX = -((_currentX / 2f) - 0.5f);
            float startY = (_currentY / 2f) - 0.5f;
            var temp = GetTile();
            var pos = new Vector3(startX + x, startY + added, 0) * _stepGrid;
            temp.position = pos;

            pos = new Vector3(startX + x, startY - y, 0) * _stepGrid;

            LeanTween.move(temp.gameObject, pos, 0.25f);
            return temp;
        }

        private void ResetTile(Transform tile)
        {
            if (tile != null) tile.gameObject.SetActive(false);
        }

        #endregion
    }
}
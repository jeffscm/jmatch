using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JMatch.Services
{
    public class ScoreService : MonoBehaviour
    {
        List<int> _listScores = new List<int>();
        bool _processing = false;

        public static Action<int> OnAddScore;

        public void AddScore(int added)
        {
            _listScores.Add(added);
            if (!_processing)
                ProcessScore();
            _processing = true;
        }

        void ProcessScore()
        {
            if (_listScores.Count == 0)
            {
                _processing = false;
                return;
            }

            var addValue = _listScores[0];
            _listScores.RemoveAt(0);
            OnAddScore?.Invoke(addValue);

            Util.Wait(0.1f, () =>
            {
                ProcessScore();
            });
        }
    }
}
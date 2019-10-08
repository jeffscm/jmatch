using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JMatch.Models
{
    public class PlayerModel
    {
        public PlayerModel()
        {
            powerUps = new int[3];
            playerLevel = PlayerPrefs.GetInt("playerLevel", 0);
            playerScore = PlayerPrefs.GetInt("playerScore", 0);
            powerUps[0] = PlayerPrefs.GetInt("powerUps0", 0);
            powerUps[1] = PlayerPrefs.GetInt("powerUps1", 0);
            powerUps[2] = PlayerPrefs.GetInt("powerUps2", 0);
            gameStarted = false;
            currentPowerUp = null;
        }

        public int playerLevel;
        public int currentTime;
        public int playerScore;
        public int playerPlayScore;

        public int targetScore;

        public bool gameStarted;

        public int[] powerUps;

        public bool isGameOver;

        public PowerUps.IPowerUp currentPowerUp;

        public void SaveData()
        {
            PlayerPrefs.SetInt("playerLevel", playerLevel);
            PlayerPrefs.SetInt("playerScore", playerScore);

            for (int i = 0; i < 3; i++)
            {
                PlayerPrefs.SetInt($"powerUps{i}", powerUps[i]);
            }
        }

        public void SavePowerUp(int idx)
        {
            PlayerPrefs.SetInt($"powerUps{idx}", powerUps[idx]);
        }
    }
}
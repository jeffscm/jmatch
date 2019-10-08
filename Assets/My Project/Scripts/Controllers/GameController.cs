using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JMatch.Controllers
{
    public class GameController : MonoBehaviour
    {
        public static Models.PlayerModel playerModel;

        public Services.DetectorService detectorService;
        public Services.InputService inputService;
        public Services.GridService gridService;
        public Services.ScoreService scoreService;
        public Services.SoundService soundService;

        public UIController uiController;

        LevelSettings[] levels;

        #region RX Main Declararions

        private void Start()
        {
            playerModel = new Models.PlayerModel();

            Services.InputService.OnDetectPressedTile += (newTileDetected) =>
            {
                detectorService.DetectNew(newTileDetected);
            };

            Services.InputService.OnDetectPossibleTile += (layerId, obj) =>
            {
                if (!playerModel.gameStarted) return;

                if (layerId == -1 || gridService.ValidateLayer(layerId, obj))
                {
                    inputService.AddNewTile(obj);
                    soundService.PlayUISound(Constants.AUDIOCLIPS.SELECT_TILE);
                }
            };

            Services.DetectorService.OnDestroyTile += (pos) =>
            {                
                if (playerModel.gameStarted)
                {
                    gridService.DestroyTile(pos);
                    scoreService.AddScore(5);
                    soundService.PlayUISound(Constants.AUDIOCLIPS.DESTROY_TILE);
                }
            };

            Services.DetectorService.OnDestroyTileFinished += (qtd) =>
            {
                if (!playerModel.gameStarted) return;

                var delay = 0.5f;

                if (qtd > 3)
                {
                    delay = 1f;
                    scoreService.AddScore(3 * qtd);
                }

                Util.Wait(delay, () =>
                {
                    if (!playerModel.gameStarted) return;
                    gridService.CheckGrid();
                });
            };

            EventController.OnEventReceived += (evt) =>
            {
                switch (evt)
                {
                    case Constants.UIEVENT.CLICK_PLAY:
                        Util.Wait(0.2f, () =>
                        {
                            StartGame(playerModel.playerLevel);
                        });
                        break;

                    case Constants.UIEVENT.CLICK_STOP:
                        StopGame();
                        EventController.OnEventReceived?.Invoke(Constants.UIEVENT.CLICK_MENU);
                        break;

                    case Constants.UIEVENT.EXECUTE_POWERUP:

                        Controllers.GameController.playerModel.currentPowerUp.ExecutePower(this);

                        soundService.PlayUISound(Constants.AUDIOCLIPS.POWERUP);

                        break;
                    case Constants.UIEVENT.CLICK_TOGGLE_SOUND:

                        soundService.ToggleMute();                        
                        break;

                }
            };

            Services.ScoreService.OnAddScore += (added) =>
            {
                if (!playerModel.gameStarted) return;

                GameController.playerModel.playerPlayScore += added;
                EventController.OnEventReceived?.Invoke(Constants.UIEVENT.UPDATE_GAME_UI);
                GameController.playerModel.SaveData();

                CheckGameOver();
            };

            // --- StartUp
            Util.Wait(0.15f, () =>
            {
                DelayStartup();
            });
        }

        #endregion

        private void DelayStartup()
        {
            var resData = Resources.Load("levels") as TextAsset;
            var obj = JsonUtility.FromJson<LevelContainer>(resData.text);

            levels = obj.levels;

            gridService.PopulateTiles();
            detectorService.PopulateTiles();

            Util.Wait(1f, () =>
            {
                InitShowMenu();
            });
        }

        private void InitShowMenu()
        {
            EventController.OnEventReceived?.Invoke(Constants.UIEVENT.CLICK_MENU);
        }

        private void StartGame(int level)
        {
            int tempX = Mathf.RoundToInt(levels[level].xMax - levels[level].xMax / 15f);
            int tempY = Mathf.RoundToInt(levels[level].yMax * 0.6f - levels[level].yMax / 150f);

            if (tempY > tempX)
                tempX = tempY;

            ReferenceManager.instance.mainCamera.orthographicSize = tempX;
            gridService.ResetGrid(levels[level].xMax, levels[level].yMax);
            playerModel.currentTime = 300;
            playerModel.playerPlayScore = 0;
            playerModel.targetScore = levels[level].targetScore;
            playerModel.isGameOver = true;

            EventController.OnEventReceived?.Invoke(Constants.UIEVENT.UPDATE_GAME_UI);
            InvokeRepeating(nameof(CheckGameLoop), 1f, 1f);
            playerModel.gameStarted = true;
        }

        private void StopGame()
        {
            CancelInvoke(nameof(CheckGameLoop));
            inputService.StopGame();
            gridService.RemoveAllTiles();
            playerModel.gameStarted = false;
        }

        private void CheckGameLoop()
        {
            playerModel.currentTime--;
            EventController.OnEventReceived?.Invoke(Constants.UIEVENT.UPDATE_GAME_UI);
            CheckGameOver();            
        }

        private void CheckGameOver()
        {
            if (!playerModel.gameStarted) return;

            if (playerModel.playerPlayScore >= playerModel.targetScore)
            {
                //GameWin
                StopGame();
                playerModel.isGameOver = false;
                playerModel.playerLevel++;
                EventController.OnEventReceived?.Invoke(Constants.UIEVENT.CLICK_GAMEOVER);
                if (!CheckHighScore())
                {
                    playerModel.SaveData();
                }
                soundService.PlayUISound(Constants.AUDIOCLIPS.GAMEWIN);
            }
            else if (playerModel.currentTime <= 0)
            {
                //GameLost
                StopGame();
                playerModel.isGameOver = true;
                EventController.OnEventReceived?.Invoke(Constants.UIEVENT.CLICK_GAMEOVER);
                CheckHighScore();
                soundService.PlayUISound(Constants.AUDIOCLIPS.GAMEOVER);
            }
        }

        private bool CheckHighScore()
        {
            if (playerModel.playerPlayScore > playerModel.playerScore)
            {
                playerModel.playerScore = playerModel.playerPlayScore;
                playerModel.SaveData();
                return true;
            }
            return false;
        }
    }

    [System.Serializable]
    public class LevelSettings
    {
        public int xMax;
        public int yMax;
        public int targetScore;
    }

    [System.Serializable]
    public class LevelContainer
    {
        public LevelSettings[] levels;
    }

}
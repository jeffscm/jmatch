using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace JMatch.Controllers
{
    public class UIController : MonoBehaviour
    {

        //Public Pages
        public List<UIPage> listPages;

        //GamePlay
        public Text gameScoreLabel, timerGameLabel, gameLevelLabel;

        //Menu
        public Text menuLevelLabel;
        public Image soundSpriteMenu;

        //GameWin
        public Text gameOverScore, gameOverHigh, gameOverLabel;

        //GameWin
        public GameObject[] gameOverPlayButtons;

        //PowerUp
        public GameObject powerButtonsArea, powerIcon;

		public UnityEngine.UI.Image fillProgress;

        //Store
        public Text[] powerUpQtds;

        //Game
        public GameObject powerUpColor, powerUpDir, powerUpVertIcon, powerUpHorIcon;

        //More than 3
        public Text greatJobLabel;

        

        #region Unity Methods

        private void Awake()
        {
            ShowPage(Constants.UIEVENT.NONE);

            EventController.OnEventReceived += (evt) =>
            {
                switch (evt)
                {
                    case Constants.UIEVENT.UPDATE_GAME_UI:
                        gameScoreLabel.text = GameController.playerModel.playerPlayScore.ToString("000000");
                        timerGameLabel.text = Util.TimeToString(GameController.playerModel.currentTime);
						gameOverScore.text = $"Score {GameController.playerModel.playerPlayScore.ToString("000000")}";
						DrawGamePowerUps();
                        break;
                    case Constants.UIEVENT.CLICK_ADD_POWER1:
                        AddPowerUp(0, 5);
                        break;
                    case Constants.UIEVENT.CLICK_ADD_POWER2:
                        AddPowerUp(1, 3);
                        break;
                    case Constants.UIEVENT.CLICK_ADD_POWER3:
                        AddPowerUp(2, 3);
                        break;
                    case Constants.UIEVENT.CLICK_USE_POWER_COLOR:
                        GameController.playerModel.powerUps[0]--;
                        DrawGamePowerUps();
                        GameController.playerModel.SavePowerUp(0);
                        GameController.playerModel.currentPowerUp = new PowerUps.PowerUpKillColor();
                        powerButtonsArea.SetActive(false);
                        powerIcon.SetActive(true);
                        break;
                    case Constants.UIEVENT.CLICK_USE_POWER_DIR:
                        
                        if (powerUpVertIcon.activeInHierarchy)
                        {
                            GameController.playerModel.powerUps[1]--;
                            GameController.playerModel.SavePowerUp(1);
                            GameController.playerModel.currentPowerUp = new PowerUps.PowerUpKillcol();
                        }
                        else
                        {
                            GameController.playerModel.powerUps[2]--;
                            GameController.playerModel.SavePowerUp(2);
                            GameController.playerModel.currentPowerUp = new PowerUps.PowerUpKillrow();
                        }                        
                        DrawGamePowerUps();
                        powerButtonsArea.SetActive(false);
                        powerIcon.SetActive(true);

						break;
                    case Constants.UIEVENT.CLICK_USE_POWERUP:
                        powerButtonsArea.SetActive(true);
                        powerIcon.SetActive(false);
                        break;
                    case Constants.UIEVENT.CHECK_SOUND_UI:

                        soundSpriteMenu.sprite = ReferenceManager.instance.audioSprites[(AudioListener.volume > 0.5f) ? 0 : 1];

                        break;
                    default:
                        ShowPage(evt);
                        break;
                }
            };

            Services.DetectorService.OnDestroyTileFinished += (qtd) =>
            {
                if (qtd > 3)
                {
                    ShowPage(Constants.UIEVENT.SHOW_GREAT_JOB);
                    greatJobLabel.text = $"x{qtd}";                    
                    Util.Wait(1f, () =>
                    {
                        HideModals(Constants.UIEVENT.SHOW_GREAT_JOB);
                        GameController.playerModel.powerUps[0]++;                        
                        GameController.playerModel.SavePowerUp(0);
                        DrawGamePowerUps();
                    });
                }
            };

            Services.ScoreService.OnAddScore += (added) =>
            {
                GameController.playerModel.playerPlayScore += added;
                gameScoreLabel.text = GameController.playerModel.playerPlayScore.ToString("000000");
				fillProgress.fillAmount = (float)GameController.playerModel.playerPlayScore / (float)GameController.playerModel.targetScore;
			};
        }

        #endregion

        #region Public Calls

        public void ShowPage(Constants.UIEVENT pageType)
        {
            var item = listPages.FirstOrDefault(t => t.pageType == pageType);
            if (item != null)
            {
                foreach (var pageItem in listPages)
                {
                    if (!item.isModal || pageItem.isModal)
                    {
                        if (pageItem.pageType != pageType)
                        {
                            pageItem.AnimPageOut();
                        }
                    }
                }
                item.AnimPageIn();
                ProcessPage(item);
            }
        }

        public void HideModals(Constants.UIEVENT pageType)
        {
            foreach (var pageItem in listPages)
            {
                if (pageItem.isModal && pageItem.pageType == pageType)
                {
                    pageItem.AnimPageOut();
                }
            }
        }

        public void HideModals()
        {
            foreach (var pageItem in listPages)
            {
                if (pageItem.isModal)
                {
                    pageItem.AnimPageOut();
                }
            }
        }

        #endregion

        #region Private Calls

        private void ProcessPage(UIPage pageItem)
        {
            switch (pageItem.pageType)
            {
                case Constants.UIEVENT.CLICK_MENU:
                    menuLevelLabel.text = $"Level\n{GameController.playerModel.playerLevel + 1}";
                    break;
                case Constants.UIEVENT.CLICK_PLAY:
                    gameLevelLabel.text = $"Level {GameController.playerModel.playerLevel + 1}";
                    gameScoreLabel.text = "0";
                    timerGameLabel.text = "05:00";
                    powerButtonsArea.SetActive(true);
                    powerIcon.SetActive(false);
                    DrawGamePowerUps();
					fillProgress.fillAmount = 0f;

					break;
                case Constants.UIEVENT.CLICK_GAMEOVER:
                    gameOverLabel.text = (GameController.playerModel.isGameOver) ? "Game Over" : "GameWin";
                    gameOverScore.text = $"Score {GameController.playerModel.playerPlayScore.ToString("000000")}";
                    gameOverHigh.gameObject.SetActive(GameController.playerModel.playerPlayScore >= GameController.playerModel.playerScore);
					gameOverPlayButtons[0].SetActive(!GameController.playerModel.isGameOver);
					gameOverPlayButtons[1].SetActive(GameController.playerModel.isGameOver);
					break;
                case Constants.UIEVENT.CLICK_BUY:
                    DrawPowerUps();
                    break;
            }
        }

        private void DrawPowerUps()
        {
            for (int i = 0; i < 3; i++)
            {
                powerUpQtds[i].text = $"x{GameController.playerModel.powerUps[i]}";
            }
        }

        private void DrawGamePowerUps()
        {
            powerUpColor.SetActive(GameController.playerModel.powerUps[0] > 0);
            powerUpDir.SetActive(GameController.playerModel.powerUps[1] > 0 || GameController.playerModel.powerUps[2] > 0);
            powerUpVertIcon.SetActive(GameController.playerModel.powerUps[1] > 0);
            powerUpHorIcon.SetActive(GameController.playerModel.powerUps[1] == 0);
        }

        private void AddPowerUp(int idx, int qtd)
        {
            GameController.playerModel.powerUps[idx] += qtd;
            GameController.playerModel.SavePowerUp(idx);
            DrawPowerUps();
        }

        #endregion
    }

    [System.Serializable]
    public class UIPage
    {
        public Constants.UIEVENT pageType;
        public CanvasGroup page;
        public bool isModal;

        public void AnimPageIn()
        {
            page.alpha = 1f;
            page.gameObject.SetActive(true);
            LeanTween.cancel(page.gameObject);
            LeanTween.move(page.transform as RectTransform, Vector3.zero, 0.25f).setEaseInOutExpo();
        }

        public void AnimPageOut()
        {
            if (!page.gameObject.activeInHierarchy) return;

            var rectTrans = page.transform as RectTransform;
            var v = rectTrans.anchoredPosition;
            v.x = rectTrans.rect.width * 2f;

            LeanTween.cancel(page.gameObject);
            LeanTween.move(rectTrans, v, 0.25f).setEaseInOutExpo().setOnComplete(() =>
            {
                page.gameObject.SetActive(false);
            });

        }
    }
} 
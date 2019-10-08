using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace JMatch.Services
{
    public class InputService : MonoBehaviour
    {

        public static Action<Transform> OnDetectPressedTile;

        public static Action<int, Transform> OnDetectPossibleTile;

        public LayerMask layerMask;

        #region Click Methods fields

        int _lastReported = -1;
        bool _hasClick = false;
        Dictionary<int, Transform> _lastID = new Dictionary<int, Transform>();

        #endregion

        #region Cool Down fields

        float _coolDownTimer = 0f;
        bool _hasCoolDown = false;

        #endregion

        void Update()
        {
            if (_hasCoolDown)
            {
                _coolDownTimer -= Time.deltaTime;
                if (_coolDownTimer < 0f)
                {
                    _hasCoolDown = false;
                    if (Controllers.GameController.playerModel.currentPowerUp != null)
                    {
                        Controllers.EventController.OnEventReceived?.Invoke(Constants.UIEVENT.CLICK_USE_POWERUP);
                    }
                    Controllers.GameController.playerModel.currentPowerUp = null;
                }
                else
                {
                    return;
                }
            }

            if (Input.GetMouseButton(0))
            {

                var worldTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit;
                hit = Physics2D.Raycast(worldTouch, Vector2.zero, Mathf.Infinity, layerMask);
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.layer == _lastReported || _lastReported == -1)
                    {
                        _lastReported = hit.collider.gameObject.layer;
                        _hasClick = true;
                        var tempId = hit.collider.gameObject.GetInstanceID();
                        if (!_lastID.ContainsKey(tempId))
                        {
                            if (_lastID.Count > 0)
                            {
                                var last = _lastID.Last().Value;
                                var distance = Vector3.Distance(last.position, hit.collider.gameObject.transform.position);
                                if (distance > GridService.StepGrid * 1.5f)
                                {
                                    Debug.Log("Skipped Tile");
                                    return;
                                }
                            }

                            OnDetectPossibleTile?.Invoke((_lastID.Count == 0) ? -1 : hit.collider.gameObject.layer, hit.collider.gameObject.transform);
                        }
                    }
                }
            }
            else
            {
                if (_hasClick)
                {
                    if (Controllers.GameController.playerModel.currentPowerUp != null && _lastID.Count == 1)
                    {
                        Controllers.GameController.playerModel.currentPowerUp.Target = _lastID.Values.ToList()[0];
                        SetCoolDown(Controllers.GameController.playerModel.currentPowerUp.CoolDownTime);
                        Controllers.EventController.OnEventReceived?.Invoke(Constants.UIEVENT.EXECUTE_POWERUP);
                    }
                    else
                    {
                        SetCoolDown(0.65f);
                    }
                    _lastID = new Dictionary<int, Transform>();
                    OnDetectPressedTile?.Invoke(null);
                }
                _hasClick = false;
                _lastReported = -1;
            }
        }

        public void AddNewTile(Transform obj)
        {
            if (Controllers.GameController.playerModel.currentPowerUp != null && _lastID.Count > 0)
                return;

            _lastID.Add(obj.gameObject.GetInstanceID(), obj);
            OnDetectPressedTile?.Invoke(obj);
        }

        public void SetCoolDown(float newTime)
        {
            if (_hasClick)
            {
                _lastID = new Dictionary<int, Transform>();
            }
            _hasClick = false;
            _lastReported = -1;

            _coolDownTimer = newTime;
            _hasCoolDown = true;
        }

        public void StopGame()
        {
            SetCoolDown(0.5f);
            _lastID = new Dictionary<int, Transform>();
        }
    }
}
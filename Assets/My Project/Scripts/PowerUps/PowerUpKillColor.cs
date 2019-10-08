using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JMatch.PowerUps
{
    public class PowerUpKillColor : IPowerUp
    {
        public float CoolDownTime
        {
            get
            {
                return 1f;
            }
        }

        public Transform Target { get; set; }

        public void ExecutePower(Controllers.GameController gameController)
        {
            var allSameColor = gameController.gridService.GetAllSameColor(Target);
            foreach(var item in allSameColor)
            {
                Services.DetectorService.OnDestroyTile?.Invoke(item.position);
            }
            Services.DetectorService.OnDestroyTileFinished?.Invoke(0);
        }
    }
}

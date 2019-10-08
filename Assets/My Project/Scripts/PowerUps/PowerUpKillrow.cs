using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JMatch.PowerUps
{
    public class PowerUpKillrow : IPowerUp
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
            var allSameRow = gameController.gridService.GetSameRow(Target);
            foreach (var item in allSameRow)
            {
                Services.DetectorService.OnDestroyTile?.Invoke(item.position);
            }
            Services.DetectorService.OnDestroyTileFinished?.Invoke(0);
        }
    }
}

using UnityEngine;

namespace JMatch.PowerUps
{
    public interface IPowerUp
    {
        float CoolDownTime { get; }
        Transform Target { get; set; }
        void ExecutePower(Controllers.GameController gameController);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JMatch.Views
{
    public class ButtonView : MonoBehaviour
    {
        public Constants.UIEVENT eventType;

        public void ExecuteButton()
        {
            Controllers.EventController.OnEventReceived?.Invoke(eventType);
			Services.SoundService.OnPlayClip?.Invoke(Constants.AUDIOCLIPS.UICLICK);
		}
    }
}

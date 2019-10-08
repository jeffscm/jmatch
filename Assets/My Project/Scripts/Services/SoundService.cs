using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JMatch.Services
{

    public class SoundService : MonoBehaviour
    {
        public AudioSource audioSource;
        public List<AudioClip> audioClips;

        public static Action<Constants.AUDIOCLIPS> OnPlayClip;

        private void Start()
        {
            var isMuted = (PlayerPrefs.GetInt("SOUND", 0) == 0) ? false : true;
            AudioListener.volume = (isMuted) ? 0f : 1f;
            Controllers.EventController.OnEventReceived?.Invoke(Constants.UIEVENT.CHECK_SOUND_UI);

            OnPlayClip += (clip) =>
            {
                PlayUISound(clip);
            };
        }

        public void PlayUISound(Constants.AUDIOCLIPS clip)
        {
            audioSource.PlayOneShot(audioClips[(int)clip]);
        }

        public void ToggleMute()
        {
            var muted = (AudioListener.volume < 0.5f);
            AudioListener.volume = (muted) ? 1f : 0f;
            PlayerPrefs.SetInt("SOUND", (muted) ? 0 : 1);
            Controllers.EventController.OnEventReceived?.Invoke(Constants.UIEVENT.CHECK_SOUND_UI);
        }
    }
}
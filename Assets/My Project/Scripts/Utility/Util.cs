using System;

namespace JMatch
{
    public class Util
    {
        public static void Wait(float t, Action onComplete)
        {
            LeanTween.value(0f, 1f, t).setOnComplete(onComplete);
        }

        public static string TimeToString(int time)
        {
            var result = string.Empty;
            var minutes = time / 60;
            var seconds = time - (minutes * 60);
            result = minutes.ToString("00") + ":" + seconds.ToString("00");
            return result;
        }
    }
}
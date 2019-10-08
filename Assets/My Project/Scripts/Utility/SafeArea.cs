
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace JMatch
{
    public class SafeArea : MonoBehaviour
    {

#if UNITY_IOS
        [DllImport("__Internal")]
        private extern static void GetSafeAreaImpl(out float x, out float y, out float w, out float h);
#endif

        private Rect GetSafeArea()
        {
            float x, y, w, h;
#if UNITY_IOS && !UNITY_EDITOR
		GetSafeAreaImpl(out x, out y, out w, out h);
#else
            x = 0;
            y = 0;
            w = Screen.width;
            h = Screen.height;
#endif
            return new Rect(x, y, w, h);
        }

        public RectTransform canvas;
        public RectTransform panel;
        public Rect lastSafeArea = new Rect(0, 0, 0, 0);

        bool IsSafeAreaActive = true;

        void ApplySafeArea(Rect area)
        {

            var anchorMin = area.position;
            var anchorMax = area.position + area.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            panel.anchorMin = anchorMin;
            panel.anchorMax = anchorMax;

            lastSafeArea = area;
        }

        void Start()
        {
            IsSafeAreaActive = true;
//#if UNITY_EDITOR
//            Invoke("TestSafe", 1f);
//#endif
            Invoke("DeactivateCheckSafe", 5f);

        }

        void DeactivateCheckSafe()
        {
            IsSafeAreaActive = false;
        }

        void TestSafe()
        {
            Rect safeArea = new Rect(0, 118, 1125, 2436 - 236);
            ApplySafeArea(safeArea);
        }

        public float GetSafeOffset()
        {
            return (Screen.height - lastSafeArea.size.y);
        }

        void Update()
        {
#if !UNITY_EDITOR
        if (IsSafeAreaActive)
        {
			Rect safeArea = GetSafeArea (); // or Screen.safeArea if you use a version of Unity that supports it

			if (safeArea != lastSafeArea)
				ApplySafeArea (safeArea);

			Canvas.ForceUpdateCanvases ();            
        }
#endif
        }
    }
}
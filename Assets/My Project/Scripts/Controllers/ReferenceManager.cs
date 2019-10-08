using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JMatch
{
    public class ReferenceManager : MonoBehaviour
    {

        public static ReferenceManager instance;

        private void Awake()
        {
            instance = this;
        }

        public Camera mainCamera;
		public Sprite[] audioSprites;

	}
}

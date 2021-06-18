using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Game.UI.Utility
{
    public class CharacterCanvas : MonoBehaviour
    {
        private void Update()
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}
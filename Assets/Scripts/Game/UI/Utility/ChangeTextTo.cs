using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

namespace Game.UI.Utility
{
    public class ChangeTextTo : MonoBehaviour
    {
        private TextMeshProUGUI title;

        private void OnEnable()
        {
            title = GetComponent<TextMeshProUGUI>();
        }

        public void ChangeText(string desiredText)
        {
            if (title != null)
                title.text = desiredText;
        }
    }
}
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Game.UI.Utility
{
    public class SegmentedPB : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _segments = new List<GameObject>();

        public void SetSlider(int amount)
        {
            amount--;

            if (amount <= _segments.Count)
            {
                for (int i = 0; i < _segments.Count; i++)
                {
                    _segments[i].SetActive(false);
                }

                for (int i = 0; i < _segments.Count; i++)
                {
                    if (i <= amount)
                        _segments[i].SetActive(true);
                }
            }

            else
                Debug.LogError("Wrong amount for this segmented slider");
        }
    }
}
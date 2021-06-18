using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupTextManager : MonoBehaviour
{
    public PopupText m_PopupTextPrefab;

    // Singleton
    protected static PopupTextManager _instance = null;
    public static PopupTextManager Instance
    {
        get
        {
            if (_instance == null) { _instance = (PopupTextManager)FindObjectOfType(typeof(PopupTextManager)); }
            return _instance;
        }
        protected set { _instance = value; }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public PopupText CreatePopupText(Vector3 worldPosition, string text, Vector3? positionOffset = null)
    {
        if (m_PopupTextPrefab == null)
            return null;

        if (positionOffset.HasValue)
            worldPosition += positionOffset.Value;

        var instantiatedPopupText = Instantiate(m_PopupTextPrefab, worldPosition, Camera.main.transform.rotation, transform);
        instantiatedPopupText.SetText(text);
        return instantiatedPopupText;
    }
}

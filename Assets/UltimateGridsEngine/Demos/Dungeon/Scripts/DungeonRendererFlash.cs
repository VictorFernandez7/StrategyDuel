using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRendererFlash : MonoBehaviour {

    [Header ("Settings")]
    public Renderer m_Renderer;
    public Color m_FlashColor = Color.red;
    public float m_FlashTime = 0.1f;

    protected Material m_material;
    protected Color m_originalColor;
    protected IEnumerator m_currentRoutine;

    protected virtual void Awake() {
        Setup();
    }

    protected virtual void Setup() {
        if (m_Renderer == null) {
            m_Renderer = GetComponent<Renderer>();
            if (m_Renderer == null) {
                m_Renderer = GetComponentInChildren<Renderer>();
                if (m_Renderer == null) {
                    Debug.Log("RendererFlasher component has no Renderer assigned");
                }
            }
        }
        // Cache the material and save the original color
        if (m_Renderer != null) {
            m_material = m_Renderer.material;
            if (m_material != null) {
                m_originalColor = m_material.color;
            }
        }
    }

    public virtual void Flash() {
        if (m_currentRoutine != null) {
            StopCoroutine(m_currentRoutine);
        }

        m_currentRoutine = FlashRoutine(m_FlashTime);
        StartCoroutine(m_currentRoutine);
    }

    protected virtual IEnumerator FlashRoutine(float time) {
        if (m_material != null) {
            m_material.color = m_FlashColor;
        }

        yield return new WaitForSeconds(time);
        
        if (m_material != null) {
            m_material.color = m_originalColor;
        }
    }
}

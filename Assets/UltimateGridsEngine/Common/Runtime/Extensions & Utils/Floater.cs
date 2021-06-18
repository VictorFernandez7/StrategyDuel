using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    [Header("Settings")]
    public float m_Amplitude = 0.5f;
    public float m_Frequency = 1f;

    // Position Storage Variables
    protected Vector3 _posOffset;
    protected Vector3 _tempPos;

    // Use this for initialization
    protected void Start()
    {
        // Store the starting position & rotation of the object
        _posOffset = transform.localPosition;
    }

    // Update is called once per frame
    protected void Update()
    {
        // Float up/down with a Sin()
        _tempPos = _posOffset;
        _tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * m_Frequency) * m_Amplitude;

        transform.localPosition = _tempPos;
    }
}

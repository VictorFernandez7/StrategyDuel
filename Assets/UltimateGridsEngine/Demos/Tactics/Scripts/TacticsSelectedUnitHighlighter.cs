using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsSelectedUnitHighlighter : MonoBehaviour
{

    [Header("Offset Position")]
    public Vector3 m_Offset = Vector3.zero;
    [Header("Model Prefab")]
    public GameObject m_HighlighterModelPrefab;

    [SerializeField]
    protected Transform _targetTransform;
    protected Transform _visualHolder;
    protected string _visualHolderName = "SelectedUnitHighlight";

    void OnEnable()
    {
        TacticsGameManager.onUnitSelectedEvent += OnUnitSelected;
        TacticsGameManager.onUnitDeSelectedEvent += OnUnitDeSelected;
    }
    void OnDisable()
    {
        TacticsGameManager.onUnitSelectedEvent -= OnUnitSelected;
        TacticsGameManager.onUnitDeSelectedEvent -= OnUnitDeSelected;
    }

    private void OnUnitSelected(TacticsCharacter unit)
    {
        Enable(unit.transform);
    }

    private void OnUnitDeSelected()
    {
        Disable();
    }

    // initialize
    protected void Awake()
    {
        if (_visualHolder == null)
        {
            // Holder Gameobject
            _visualHolder = new GameObject(_visualHolderName).transform;
            _visualHolder.parent = transform;
            _visualHolder.gameObject.SetActive(false);

            // Instantiate model
            Instantiate(m_HighlighterModelPrefab, _visualHolder.position, Quaternion.identity, _visualHolder);
        }
    }

    // Update is called once per frame
    protected void Update()
    {
        // Set the highlighter's position
        if (_targetTransform != null)
        {
            if (_visualHolder.position != _targetTransform.position + m_Offset)
                _visualHolder.position = _targetTransform.position + m_Offset;

            if (!_visualHolder.gameObject.activeSelf)
                _visualHolder.gameObject.SetActive(true);
        }
        else
        {
            if (_visualHolder.gameObject.activeSelf)
                _visualHolder.gameObject.SetActive(false);
        }
    }

    public void Enable(Transform targetTransform)
    {
        _targetTransform = targetTransform;

        if (_visualHolder.position != _targetTransform.position + m_Offset)
            _visualHolder.position = _targetTransform.position + m_Offset;

        if (!_visualHolder.gameObject.activeSelf)
            _visualHolder.gameObject.SetActive(true);
    }

    public void Disable()
    {
        _targetTransform = null;

        if (_visualHolder.gameObject.activeSelf)
            _visualHolder.gameObject.SetActive(false);
    }
}

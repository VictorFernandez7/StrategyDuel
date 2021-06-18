using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class RangeVisualizer : MonoBehaviour
{
    public enum UpdateMode { None, TimeInterval, OnMoveToAnotherTile }

    [Header("Activation")]
    public bool m_TriggerOnStart = true;
    public bool m_TriggerOnKeyPress = false;
    [ShowIf("m_TriggerOnKeyPress")]
    public KeyCode m_TriggerKey = KeyCode.Space;

    [Header("Range Parameters")]
    public RangeParameters m_RangeParameters;

    [Header("Visual Node Settings")]
    public GameObject m_NodePrefab;
    public Vector3 m_NodeWorldPositionOffSet;

    [Header("Update Mode")]
    public UpdateMode m_UpdateMode = UpdateMode.None;
    [ShowIf("UpdateModeIsTimeInterval")]
    public float m_UpdateTimeInterval = 0.5f;
    public bool UpdateModeIsTimeInterval() { return m_UpdateMode == UpdateMode.TimeInterval; }

    protected float _updateIntervalTimeLeft;
    protected List<GridTile> _currentRange;
    protected Transform _visualPathHolder;
    protected GridObject _gridObject;
    protected GridMovement _gridMovement;
    protected string _holderName = "Visual Path Holder";
    protected bool _enabled = false;

    private void OnDisable()
    {
        if (_gridMovement != null)
        {
            _gridMovement.OnMovementEnd -= MovementEnded;
        }
    }

    private void Awake()
    {
        _gridObject = GetComponent<GridObject>();
        _gridMovement = GetComponent<GridMovement>();
        if (_gridMovement != null)
        {
            _gridMovement.OnMovementEnd += MovementEnded;
        }
        _updateIntervalTimeLeft = m_UpdateTimeInterval;
    }

    private void Start()
    {
        if (m_TriggerOnStart)
        {
            CalculateRange();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_TriggerOnKeyPress)
        {
            if (Input.GetKeyDown(m_TriggerKey))
            {
                TriggerVisualizer();
            }
        }

        if (m_UpdateMode == UpdateMode.TimeInterval)
        {
            // Interval timer
            if (_updateIntervalTimeLeft > 0f)
            {
                _updateIntervalTimeLeft = _updateIntervalTimeLeft - Time.deltaTime;
                if (_updateIntervalTimeLeft <= 0f)
                {
                    // Set the timer
                    _updateIntervalTimeLeft = m_UpdateTimeInterval;
                    if (_currentRange != null && _currentRange.Count > 0)
                    {
                        CalculateRange();
                    }
                }
            }
        }
    }

    public void TriggerVisualizer()
    {
        if (_enabled)
        {
            ClearVisualPath();
        }
        else
        {
            CalculateRange();
        }
    }

    public void CalculateRange()
    {
        var newRange = new List<GridTile>();

        newRange = RangeAlgorithms.SearchByParameters(_gridObject.m_CurrentGridTile, m_RangeParameters);

        if (newRange != null && newRange.Count > 0)
        {
            ClearVisualPath();
            _currentRange = newRange;
            PopulateVisualRange();
        }
    }


    private void PopulateVisualRange()
    {
        _enabled = true;
        if (_visualPathHolder == null)
        {
            _visualPathHolder = new GameObject(_holderName).transform;
        }

        for (int i = 0; i < _currentRange.Count; i++)
        {
            var newNode = Instantiate(m_NodePrefab, _currentRange[i].m_WorldPosition + m_NodeWorldPositionOffSet, Quaternion.identity).transform;
            newNode.parent = _visualPathHolder;
        }
    }

    private void ClearVisualPath()
    {
        _enabled = false;
        if (_visualPathHolder != null)
        {
            DestroyImmediate(_visualPathHolder.gameObject);
            _currentRange.Clear();
        }
    }

    // Callback for the movement ended on GridMovement, used to execute queued input
    private void MovementEnded(GridMovement movement, GridTile fromGridPos, GridTile toGridPos)
    {
        if (m_UpdateMode == UpdateMode.OnMoveToAnotherTile && _currentRange != null && _currentRange.Count > 0)
        {
            CalculateRange();
        }
    }
}

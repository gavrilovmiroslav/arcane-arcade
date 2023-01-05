using DG.Tweening;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan 
    : MonoBehaviour
    , IReactsToEvent<ITurnPropertyChanged.UnitChanged>
{
    public static CameraPan Instance;

    private bool _PanTouchInitiated = false;
    private Vector3 _TouchStartPosition;
    public float TransitionDuration = 0.2f;
    public Transform CameraTarget;

    public static bool CanPan = true;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        TurnManager.Instance.OnUnitChanged += React_OnUnitChanged;
    }

    private void React_OnUnitChanged(TurnBasedUnit oldUnit, TurnBasedUnit newUnit)
    {
        PanTo(newUnit.transform.position);
    }

    public void PanTo(Vector3 target)
    {
        CameraTarget.DOMove(target, TransitionDuration);
    }

    private void Update()
    {
        if (CanPan)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _PanTouchInitiated = true;
                _TouchStartPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButton(0) && _PanTouchInitiated)
            {
                var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var dir = _TouchStartPosition - pos;
                CameraTarget.position += dir;
            } 
            else
            {
                if (_PanTouchInitiated)
                {
                    _PanTouchInitiated = false;
                }
            }
        } 
        else 
        {
            _PanTouchInitiated = false;
        }
    }

}

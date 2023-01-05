using System.Collections;

using UnityEngine;

public class PlayerFlingController 
    : AbstractFlingController
    , IReactsToEvent<IFlingCollisionPropertyChanged.FlingCollisionChainDone>
{
    public override void SetupEvents()
    {
        FlingCollisionManager.Instance.OnFlingCollisionChainDone += React_OnFlingCollisionChainDone;
    }

    private void React_OnFlingCollisionChainDone()
    {
        if (_TurnBasedUnit.Current)
        {
            _FlingChainComplete = true;
        }
    }

    public void Update()
    {
        if (!_TurnBasedUnit.Current) return;

        Timer += Time.deltaTime;
        if (Timer >= 1.0f)
            Timer = 0.0f;

        _InputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _InputPosition.z = 0.0f;

        if (_HasShotAlready)
        {
            if (_FlingProcessCoroutine == null)
            {                
                _FlingProcessCoroutine = StartCoroutine(WaitForFlingToEndCo());
            }
            return;
        }

        var indicators = GameManager.Instance.TargetingIndicators;
        var count = GameManager.Instance.Config.TargetingIndicatorCount;

        if (!FlingInProgress)
        {
            for (int i = 0; i < count; i++)
            {
                indicators[i].SetActive(false);
            }

            if (Input.GetMouseButtonDown(0) && _Collider.OverlapPoint(_InputPosition))
            {
                FlingInProgress = true;
                _FlingPhysics.HasAlreadyHit = false;
                InvokeFlingStarted();
            }
        }
        else
        {
            var pos = this.transform.position;
            pos.z = 0.0f;

            _FlingingRay.origin = pos;
            _FlingingRay.direction = _InputPosition - pos;

            _AimingRay.origin = pos;
            _AimingRay.direction = pos - _InputPosition;

            _AimingDistance = Vector3.Distance(pos, _InputPosition);

            var min = GameManager.Instance.Config.MinShootingDistance;
            var max = GameManager.Instance.Config.MaxShootingDistance;

            var target = _InputPosition;
            if (_AimingDistance > max)
            {
                target = _FlingingRay.GetPoint(max);
            }

            var color = Color.white;
            if (_AimingDistance > min)
            {
                var pts = Mathf.Min(count - 1, Mathf.RoundToInt(_AimingDistance * 10.0f));

                for (int i = 0; i < count; i++)
                {
                    var visible = i < pts;
                    indicators[i].SetActive(visible);
                    if (visible)
                    {
                        var t = (i + Timer) * (1.0f / count);
                        indicators[i].transform.position = _AimingRay.GetPoint(min + t);
                        color.a = 1 - t;
                        indicators[i].GetComponent<SpriteRenderer>().color = color;
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    FlingInProgress = false;

                    _AimingDistance = Mathf.Round(_AimingDistance * 10.0f) / 10.0f;
                    if (_AimingDistance > 1.0f) _AimingDistance = 1.0f;

                    var speed = _AimingDistance * 2.0f * _AimingRay.direction;
                    _HasShotAlready = true;

                    for (int i = 0; i < count; i++)
                    {
                        indicators[i].SetActive(false);
                    }
                    GameManager.Instance.ShootingTarget.SetActive(false);
                    FlingCollisionManager.Instance.AddMoving(_FlingPhysics);
                    InvokeFlingCompleted(speed);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    indicators[i].SetActive(false);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    FlingInProgress = false;
                    CameraPan.CanPan = true;
                    
                    GameManager.Instance.ShootingTarget.SetActive(false);
                    InvokeFlingCancelled();
                }
            }

            GameManager.Instance.ShootingTarget.transform.position = target;
        }
    }
}

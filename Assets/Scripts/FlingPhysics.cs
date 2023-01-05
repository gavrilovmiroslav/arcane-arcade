using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public struct FlingCollision
{
    public TurnBasedUnit Attacker;
    public TurnBasedUnit Target;
    public Vector3 Position;
}

public class FlingPhysics
    : MonoBehaviour
    , IReactsToEvent<IFlingingPropertyChanged.FlingCompleted>
{
    public bool HasAlreadyHit = false;
    private Vector3 _Speed = Vector3.zero;

    public Vector3 Speed { get => _Speed; }
    
    public void Start()
    {
        if (TryGetComponent<FlingController>(out var flingController))
        {
            flingController.OnFlingCompleted += React_OnFlingCompleted;
        }
    }

    private void React_OnFlingCompleted(Vector2 fling)
    {
        AddForce(fling);
    }

    IEnumerator EnableHitsCo()
    {
        yield return new WaitForEndOfFrame();
        this.HasAlreadyHit = false;
    }

    public void AddForce(Vector3 force)
    {
        _Speed = force;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<FlingPhysics>(out FlingPhysics other))
        {
            if (other.HasAlreadyHit) return;
            if (this == other) return;

            var faster = (other._Speed.sqrMagnitude > this._Speed.sqrMagnitude) ? other : this;
            var slower = (other._Speed.sqrMagnitude > this._Speed.sqrMagnitude) ? this : other;

            var attackerUnit = faster.GetComponent<TurnBasedUnit>();
            var targetUnit = slower.GetComponent<TurnBasedUnit>();
            
            if (attackerUnit != null)
            {
                var flingCollision = new FlingCollision
                {
                    Attacker = attackerUnit,
                    Target = targetUnit,
                    Position = collision.contacts[0].point
                };

                FlingCollisionManager.Instance.AddCollision(flingCollision);
            }

            (other._Speed, this._Speed) = (this._Speed, other._Speed);
            this.HasAlreadyHit = true;
        }
        else
        {
            var flingCollision = new FlingCollision
            {
                Attacker = this.GetComponent<TurnBasedUnit>(),
                Target = null,
                Position = collision.contacts[0].point
            };

            FlingCollisionManager.Instance.AddCollision(flingCollision);
            this._Speed = Vector2.Reflect(this._Speed * 0.75f, collision.contacts[0].normal);
        }

        StartCoroutine(EnableHitsCo());
    }

    public void Update()
    {
        if (_Speed.magnitude > 0.04f)
        {
            this.transform.position += _Speed * 1.75f * Time.deltaTime;
            _Speed *= 0.98f;
            return;
        }
        else
        {
            _Speed = Vector3.zero;
        }
    }
}

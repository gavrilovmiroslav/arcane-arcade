using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlingCollisionManager
    : MonoBehaviour
    , IFlingCollisionPropertyChanged
{
    public static FlingCollisionManager Instance;

    public event IFlingCollisionPropertyChanged.FlingCollisionsCleared OnFlingCollisionsCleared;
    public event IFlingCollisionPropertyChanged.FlingCollisionHappened OnFlingCollisionHappened;
    public event IFlingCollisionPropertyChanged.FlingCollisionChainDone OnFlingCollisionChainDone;

    public List<FlingPhysics> UnitsInMotion = new();
    
    public float ChainWaitDuration = 2.0f;
    public readonly Queue<FlingCollision> Collisions = new();
    
    public void Awake()
    {
        Instance = this;
    }

    public void ClearCollisions()
    {
        Collisions.Clear();
        OnFlingCollisionsCleared?.Invoke();
    }

    public void AddMoving(FlingPhysics obj)
    {
        if (obj == null) return;
        if (!UnitsInMotion.Contains(obj))
            UnitsInMotion.Add(obj);
    }

    public void AddCollision(FlingCollision collision)
    {
        Collisions.Enqueue(collision);
        OnFlingCollisionHappened?.Invoke(collision);
        
        AddMoving(collision.Attacker.GetComponent<FlingPhysics>());

        if (collision.Target != null)
        {
            AddMoving(collision.Target.GetComponent<FlingPhysics>());
            GameManager.Instance.CameraFollow(collision.Target.transform);
        }            
    }

    public IEnumerator WaitForCollisionsToStop()
    {
        int remaining = UnitsInMotion.Count;
        List<FlingPhysics> removePile = new();

        yield return new WaitForSeconds(ChainWaitDuration);

        while (UnitsInMotion.Count > 0)
        {
            foreach (var unit in UnitsInMotion)
            {
                Debug.Log(unit.Speed);
                if (unit.Speed.sqrMagnitude < 0.04f)
                {
                    unit.StopMotion();
                    removePile.Add(unit);
                }
            }

            foreach (var unit in removePile)
                UnitsInMotion.Remove(unit);

            removePile.Clear();

            yield return new WaitForSeconds(ChainWaitDuration);
        }
    }
}

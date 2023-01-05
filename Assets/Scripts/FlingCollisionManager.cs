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

    public float ChainWaitDuration = 1.0f;
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

    public void AddCollision(FlingCollision collision)
    {
        Collisions.Enqueue(collision);
        OnFlingCollisionHappened?.Invoke(collision);
        StartCoroutine(WaitForFlingChainContinuationCo(Collisions.Count));
    }

    private IEnumerator WaitForFlingChainContinuationCo(int chainLength)
    {
        yield return new WaitForSeconds(ChainWaitDuration);
        if (Collisions.Count == chainLength)
        {
            OnFlingCollisionChainDone?.Invoke();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSystem 
    : MonoBehaviour
    , IReactsToEvent<IFlingCollisionPropertyChanged.FlingCollisionsCleared>
    , IReactsToEvent<IFlingCollisionPropertyChanged.FlingCollisionHappened>
    , IReactsToEvent<IFlingCollisionPropertyChanged.FlingCollisionChainDone>
{
    public void Start()
    {
        FlingCollisionManager.Instance.OnFlingCollisionsCleared += React_OnFlingCollisionsCleared;
        FlingCollisionManager.Instance.OnFlingCollisionHappened += React_OnFlingCollisionHappened;
        FlingCollisionManager.Instance.OnFlingCollisionChainDone += React_OnFlingCollisionChainDone;
    }

    public int CollisionCount = 0;
    public int TargetCollisionCount = 0;

    private void React_OnFlingCollisionsCleared()
    {
        CollisionCount = 0;
        TargetCollisionCount = 0;
    }

    private void React_OnFlingCollisionChainDone()
    {
        Debug.Log($"+{TargetCollisionCount} experience!");
    }

    private void React_OnFlingCollisionHappened(FlingCollision collision)
    {
        var (atk, tar) = (collision.Attacker, collision.Target);
        if (tar != null)
        {
            if (TargetCollisionCount == 0)
            {
                // first blood!
                if (atk.Definition.Friend != tar.Definition.Friend)
                {
                    tar.Health -= atk.Power;
                    if (tar.Health <= 0)
                    {
                        EmotionManager.Instance.Emote(tar, EmotionType.Sad);
                    }
                }
            }

            TargetCollisionCount++;
        }

        CollisionCount++;
    }
}

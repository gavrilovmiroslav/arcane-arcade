using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackManager 
    : MonoBehaviour
    , IReactsToEvent<IFlingCollisionPropertyChanged.FlingCollisionHappened>
{    
    public List<FeedbackScriptable> Feedbacks = new();
    
    public void Start()
    {
        FlingCollisionManager.Instance.OnFlingCollisionHappened += React_OnFlingCollisionHappened;
    }

    private void React_OnFlingCollisionHappened(FlingCollision collision)
    {
        var current = TurnManager.Instance.CurrentTurnOwner;
        if (collision.Attacker == current || collision.Target == current)
        {
            foreach (var feedback in Feedbacks)
            {
                if (feedback.ShouldActivateWhenTargetIsNone || collision.Target != null)
                    StartCoroutine(feedback.Action());
            }
        }
    }
}

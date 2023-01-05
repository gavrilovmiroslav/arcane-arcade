using System.Collections;

using UnityEngine;

[RequireComponent(typeof(FlingPhysics))]
public class EnemyAIController 
    : AbstractFlingController
    , IReactsToEvent<ITurnPropertyChanged.UnitChanged>
    , IReactsToEvent<IFlingCollisionPropertyChanged.FlingCollisionChainDone>
{
    public override void SetupEvents()
    {
        TurnManager.Instance.OnUnitChanged += React_OnUnitChanged;
        FlingCollisionManager.Instance.OnFlingCollisionChainDone += React_OnFlingCollisionChainDone;
    }

    private void React_OnUnitChanged(TurnBasedUnit oldUnit, TurnBasedUnit newUnit)
    {
        if (TurnManager.Instance.IsCurrentUnit(this))
        {
            StartCoroutine(DoAIActionCo());
        }
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

        if (_HasShotAlready)
        {
            if (_FlingProcessCoroutine == null)
            {
                _FlingProcessCoroutine = StartCoroutine(WaitForFlingToEndCo());
            }
            return;
        }
    }

    public IEnumerator DoAIActionCo()
    {
        int thinking = Random.Range(1, 3);
        yield return EmotionManager.Instance.Emote(this, EmotionType.Whisper, thinking);

        InvokeFlingStarted();
        var speed = Random.insideUnitCircle.normalized * ((Random.Range(1, 10) * 2) / 10.0f);
        yield return EmotionManager.Instance.Emote(this, EmotionType.Idea, 1);
        InvokeFlingCompleted(speed);
        _FlingPhysics.AddForce(speed);
        FlingCollisionManager.Instance.AddMoving(_FlingPhysics);
        _HasShotAlready = true;
        yield return new WaitForSeconds(0.5f);
    }
}

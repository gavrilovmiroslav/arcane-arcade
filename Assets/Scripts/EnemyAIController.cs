using System.Collections;

using UnityEngine;

[RequireComponent(typeof(FlingPhysics))]
public class EnemyAIController 
    : MonoBehaviour
    , IFlingingPropertyChanged
    , IReactsToEvent<ITurnPropertyChanged.UnitChanged>
    , IReactsToEvent<IFlingCollisionPropertyChanged.FlingCollisionChainDone>
{
    public event IFlingingPropertyChanged.FlingingStarted OnFlingingStarted;
    public event IFlingingPropertyChanged.FlingCompleted OnFlingCompleted;
    public event IFlingingPropertyChanged.FlingingCancelled OnFlingCancelled;

    private TurnBasedUnit _TurnBasedUnit;    
    private FlingPhysics _FlingPhysics;

    private bool _FlingChainComplete = false;
    private bool _HasShotAlready = false;

    public void Start()
    {
        _FlingPhysics = GetComponent<FlingPhysics>();
        _TurnBasedUnit = GetComponent<TurnBasedUnit>();

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
            var collisionCount = FlingCollisionManager.Instance.Collisions.Count;
            if (_FlingPhysics.Speed.sqrMagnitude == 0.0f 
                && ((collisionCount > 0 && _FlingChainComplete) || collisionCount == 0))
            {
                GameManager.BroadcastNextTurn();
                _FlingChainComplete = false;
                _HasShotAlready = false;
                CameraPan.CanPan = true;
            }

            return;
        }
    }

    public IEnumerator DoAIActionCo()
    {
        int thinking = Random.Range(1, 3);
        yield return EmotionManager.Instance.Emote(this, EmotionType.Whisper, thinking);
        OnFlingingStarted?.Invoke();
        var speed = Random.insideUnitCircle.normalized * ((Random.Range(1, 10) * 2) / 10.0f);
        yield return EmotionManager.Instance.Emote(this, EmotionType.Idea, 1);
        OnFlingCompleted?.Invoke(speed);
        _FlingPhysics.AddForce(speed);
        _HasShotAlready = true;
        yield return new WaitForSeconds(0.5f);
    }
}

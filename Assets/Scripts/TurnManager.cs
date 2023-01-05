using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager 
    : MonoBehaviour
    , ITurnPropertyChanged
    , IReactsToEvent<IGamePropertyChanged.NextTurnStarted>
    , IReactsToEvent<IGamePropertyChanged.TurnOrderUnitRegistered>
{
    public event ITurnPropertyChanged.UnitDescVisibilityChanged OnUnitDescVisibilityChanged;
    public event ITurnPropertyChanged.UnitChanged OnUnitChanged;

    private UnitVisibility _UnitDescVisibility = UnitVisibility.Shown;
    private readonly Queue<TurnBasedUnit> _TurnOrder = new();
    public TurnBasedUnit CurrentTurnOwner = null;
    
    public static TurnManager Instance = null;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        GameManager.Instance.OnRegisterTurnOrder += React_OnRegisterTurnOrder;
        GameManager.Instance.OnStartNextTurn += React_OnStartNextTurn;
    }

    public UnitVisibility UnitDescriptionVisibility
    {
        get => _UnitDescVisibility;
        set
        {
            var old = _UnitDescVisibility;
            _UnitDescVisibility = value;
            OnUnitDescVisibilityChanged?.Invoke(old, _UnitDescVisibility);
        }
    }

    public bool IsCurrentUnit<T>(T t) where T: MonoBehaviour
    {
        if (CurrentTurnOwner == null) return false;
        return CurrentTurnOwner.gameObject == t.gameObject;
    }

    private void React_OnStartNextTurn()
    {
        FlingCollisionManager.Instance.ClearCollisions();

        var old = CurrentTurnOwner;
        if (CurrentTurnOwner != null)
        {
            CurrentTurnOwner.Current = false;
            if (CurrentTurnOwner.Health > 0)
            {
                _TurnOrder.Enqueue(CurrentTurnOwner);
            }
            CurrentTurnOwner = null;
        }

        if (_TurnOrder.Count > 0)
        {
            var nextUnit = _TurnOrder.Dequeue();
            nextUnit.Current = true;
            CurrentTurnOwner = nextUnit;

            OnUnitChanged?.Invoke(old, CurrentTurnOwner);
        }
    }

    private void React_OnRegisterTurnOrder(TurnBasedUnit turn)
    {
        _TurnOrder.Enqueue(turn);
    }
}

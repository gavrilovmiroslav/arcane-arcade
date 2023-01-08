using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    public Queue<TurnBasedUnit> TurnOrder = new();
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

    public int TurnCleanupTokens = 0;
    private void React_OnStartNextTurn()
    {
        StartCoroutine(StartNextTurnCo());
    }

    private IEnumerator StartNextTurnCo()
    {
        FlingCollisionManager.Instance.ClearCollisions();
        GameManager.BroadcastPrepareForTurnCleanup();
        GameManager.BroadcastTurnCleanup();

        while (TurnCleanupTokens > 0) yield return new WaitForEndOfFrame();

        var old = CurrentTurnOwner;
        if (CurrentTurnOwner != null)
        {
            CurrentTurnOwner.Current = false;
            if (CurrentTurnOwner.Definition == null || CurrentTurnOwner.Health > 0)
            {
                TurnOrder.Enqueue(CurrentTurnOwner);
            }
            CurrentTurnOwner = null;
        }

        if (TurnOrder.Count > 0)
        {
            var nextUnit = TurnOrder.Dequeue();
            nextUnit.Current = true;
            CurrentTurnOwner = nextUnit;
            Debug.Assert(CurrentTurnOwner != null);

            OnUnitChanged?.Invoke(old, CurrentTurnOwner);
        }
    }

    private void React_OnRegisterTurnOrder(TurnBasedUnit turn)
    {
        TurnOrder.Enqueue(turn);
    }
}

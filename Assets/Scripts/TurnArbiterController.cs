using System.Collections;

using UnityEngine;

[RequireComponent(typeof(FlingPhysics))]
public class TurnArbiterController 
    : AbstractFlingController
    , IReactsToEvent<ITurnPropertyChanged.UnitChanged>
{
    public int Turn = 1;

    public override void SetupEvents()
    {
        TurnManager.Instance.OnUnitChanged += React_OnUnitChanged;
        this._TurnBasedUnit.Health = 100;
    }

    public void OnDestroy()
    {
        TurnManager.Instance.OnUnitChanged -= React_OnUnitChanged;
    }

    private void React_OnUnitChanged(TurnBasedUnit oldUnit, TurnBasedUnit newUnit)
    {
        if (this._TurnBasedUnit == newUnit)
        {
            Debug.Log($"TURN {Turn}!");
            Turn += 1;
            FinishTurn();
        }
    }
}

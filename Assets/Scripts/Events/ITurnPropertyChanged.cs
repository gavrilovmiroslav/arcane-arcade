using System.Collections;

using UnityEngine;

public enum UnitVisibility
{
    Hidden,
    Halfway,
    Shown,
}

public interface ITurnPropertyChanged : IReactiveEventInterface
{
    public delegate void UnitDescVisibilityChanged(UnitVisibility oldVis, UnitVisibility newVis);
    public delegate void UnitChanged(TurnBasedUnit oldUnit, TurnBasedUnit newUnit);

    public event UnitDescVisibilityChanged OnUnitDescVisibilityChanged;
    public event UnitChanged OnUnitChanged;
}
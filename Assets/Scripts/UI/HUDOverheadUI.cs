using UnityEngine;

public class HUDOverheadUI 
    : MonoBehaviour
    , IReactsToEvent<IUnitPropertyChanged.HealthChanged>
    , IReactsToEvent<IUnitPropertyChanged.PowerChanged>
{
    private TurnBasedUnit _TurnTaker;
    public TMPro.TextMeshPro HealthText;
    public TMPro.TextMeshPro PowerText;

    public void Awake()
    {
        _TurnTaker = GetComponentInParent<TurnBasedUnit>();
        _TurnTaker.OnHealthChanged += React_OnHealthChanged;
        _TurnTaker.OnPowerChanged += React_OnPowerChanged;
    }

    private void React_OnHealthChanged(int oldValue, int newValue) 
    {
        HealthText.text = $"{_TurnTaker.Health}";
    }

    private void React_OnPowerChanged(int oldValue, int newValue) 
    {
        PowerText.text = $"{_TurnTaker.Power}";
    }
}

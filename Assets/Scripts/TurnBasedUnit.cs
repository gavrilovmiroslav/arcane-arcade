using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteOutline))]
public class TurnBasedUnit 
    : MonoBehaviour
    , IUnitPropertyChanged
    , IUnitStatsChanged    
    , IReactsToEvent<IFlingingPropertyChanged.FlingingStarted>
    , IReactsToEvent<IFlingingPropertyChanged.FlingCompleted>
    , IReactsToEvent<IFlingingPropertyChanged.FlingingCancelled>
{
    public event IUnitPropertyChanged.HealthChanged OnHealthChanged;
    public event IUnitPropertyChanged.PowerChanged OnPowerChanged;
    public event IUnitStatsChanged.EnemyKilled OnEnemyKilled;
    public event IUnitStatsChanged.UnitLost OnUnitLost;

    private SpriteOutline _SpriteOutline;
    private int _Health = 0;
    private int _Power = 0;
    private bool _Current = false;

    public UnitScriptable Definition;

    public int Health
    {
        get { return _Health; }
        set
        {
            var oldHealth = _Health;
            _Health = value;
            if (_Health < 0) _Health = 0;
            if (_Health > Definition.Health) _Health = Definition.Health;
            OnHealthChanged?.Invoke(oldHealth, _Health);
        }
    }

    public int Power
    {
        get { return _Power; }
        set
        {
            var oldPower = _Power;
            _Power = value;
            if (_Power < 0) _Power = 0;
            OnPowerChanged?.Invoke(oldPower, _Power);
        }
    }

    public bool Current
    {
        get => _Current;

        set
        {
            _Current = value;            
            SetOutlining(_Current);
        }
    }


    public void Awake()
    {
        _SpriteOutline = GetComponent<SpriteOutline>();
        Current = false;
    }

    public void ConnectController()
    {
        if (TryGetComponent<FlingController>(out FlingController controller))
        {
            controller.OnFlingingStarted += React_OnFlingingStarted;
            controller.OnFlingCompleted += React_OnFlingCompleted;
            controller.OnFlingCancelled += React_OnFlingCancelled;
        }

        if (TryGetComponent<EnemyAIController>(out EnemyAIController ai))
        {
            ai.OnFlingingStarted += React_OnFlingingStarted;
            ai.OnFlingCompleted += React_OnFlingCompleted;
            ai.OnFlingCancelled += React_OnFlingCancelled;
        }
    }

    private void React_OnFlingCompleted(Vector2 fling)
    {
        GameManager.Instance.ShootingTarget.SetActive(false);
        TurnManager.Instance.UnitDescriptionVisibility = UnitVisibility.Shown;
    }

    private void React_OnFlingingStarted()
    {
        CameraPan.CanPan = false;
        GameManager.Instance.ShootingTarget.SetActive(Definition.Friend);
        TurnManager.Instance.UnitDescriptionVisibility = UnitVisibility.Halfway;
        GameManager.Instance.CameraFollow(this.transform);
    }

    private void React_OnFlingCancelled()
    {
        TurnManager.Instance.UnitDescriptionVisibility = UnitVisibility.Halfway;
        GameManager.Instance.ShootingTarget.SetActive(false);
        CameraPan.Instance.CameraTarget.SetParent(null);
    }

    public void Start()
    {
        Health = Definition.Health;
        Power = Definition.Power;
    }

    public void SetOutlining(bool state)
    {
        _SpriteOutline.directions = state
            ? SpriteOutline.Directions.ON
            : SpriteOutline.Directions.OFF;
    }
}

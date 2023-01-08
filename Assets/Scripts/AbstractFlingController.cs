using System.Collections;

using UnityEngine;

[RequireComponent(typeof(FlingPhysics))]
public class AbstractFlingController 
    : MonoBehaviour
    , IFlingingPropertyChanged
{
    public event IFlingingPropertyChanged.FlingingStarted OnFlingingStarted;
    public event IFlingingPropertyChanged.FlingCompleted OnFlingCompleted;
    public event IFlingingPropertyChanged.FlingingCancelled OnFlingCancelled;

    public void InvokeFlingStarted()
    {
        OnFlingingStarted?.Invoke();
    }

    public void InvokeFlingCancelled()
    {
        OnFlingCancelled?.Invoke();
    }

    public void InvokeFlingCompleted(Vector2 v)
    {
        OnFlingCompleted.Invoke(v);
    }

    public float Timer = 0.0f;

    public static bool FlingInProgress = false;
    public static Vector3 FlingDirection = Vector3.zero;

    protected TurnBasedUnit _TurnBasedUnit;
    protected Ray _FlingingRay;
    protected Ray _AimingRay;

    protected float _AimingDistance;
    protected Vector3 _InputPosition;
    protected CircleCollider2D _Collider;
    protected FlingPhysics _FlingPhysics;

    protected bool _FlingChainComplete = false;
    protected bool _HasShotAlready = false;
    protected Coroutine _FlingProcessCoroutine = null;

    public void Awake()
    {
        _FlingingRay = new Ray();
    }

    public virtual void SetupEvents() { }

    public void Start()
    {
        _Collider = GetComponent<CircleCollider2D>();
        _FlingPhysics = GetComponent<FlingPhysics>();
        _TurnBasedUnit = GetComponent<TurnBasedUnit>();

        SetupEvents();
    }

    private void React_OnFlingCollisionChainDone()
    {
        if (_TurnBasedUnit.Current)
        {
            _FlingChainComplete = true;
        }
    }

    protected void FinishTurn()
    {
        GameManager.BroadcastNextTurn();
        _FlingChainComplete = false;
        _HasShotAlready = false;
        GameManager.Instance.CameraFollow(null);
        CameraPan.CanPan = true;
        _FlingProcessCoroutine = null;
    }

    public IEnumerator WaitForFlingToEndCo()
    {
        yield return FlingCollisionManager.Instance.WaitForCollisionsToStop();

        FinishTurn();
    }
}

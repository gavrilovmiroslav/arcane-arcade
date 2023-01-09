using DG.Tweening;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LocationKind
{
    Friend, Enemy, Obstacle
}

public struct Location
{
    public Transform place;
    public LocationKind kind;
}

public class GameManager : MonoBehaviour
    , IGamePropertyChanged
    , IReactsToEvent<IFlingCollisionPropertyChanged.FlingCollisionHappened>
{
    public static GameManager Instance { get; private set; }

    public event IGamePropertyChanged.TurnCleanup OnTurnCleanup;
    public event IGamePropertyChanged.NextTurnStarted OnStartNextTurn;
    public event IGamePropertyChanged.TurnOrderUnitRegistered OnRegisterTurnOrder;

    public static void BroadcastTurnCleanup()
    {
        Instance.OnTurnCleanup?.Invoke();
    }

    public static void BroadcastNextTurn()
    {
        Instance.OnStartNextTurn?.Invoke();
    }

    public static void BroadcastTurnOrderRegistration(TurnBasedUnit turn)
    {
        if (turn != null)
            Instance.OnRegisterTurnOrder?.Invoke(turn);
    }

    private GameConfiguration _GameConfig;
    public GameConfiguration Config { get { return _GameConfig; } }

    public void Awake()
    {
        Instance = this;
        _GameConfig = GetComponent<GameConfiguration>();

        DOTween.Init();
    }
    
    public Transitioner Transition;
    public Vector3 NextPosition;

    public Cinemachine.CinemachineVirtualCamera Camera;
    public GameObject ShootingTarget;

    [HideInInspector]
    public List<GameObject> TargetingIndicators = new List<GameObject>();

    public GameObject TouchColliderPrefab;

    private List<GameObject> _Instantiated = new List<GameObject>();

    public void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        for (int i = 0; i < Config.TargetingIndicatorCount; i++)
        {
            TargetingIndicators.Add(Instantiate(Config.TargetingIndicator, Vector3.zero, Quaternion.identity));
        }

        GameManager.Instance.ShootingTarget.SetActive(false);
        StartCoroutine(StartCo());
    }

    public IEnumerator StartCo()
    {
        yield return new WaitForSeconds(0.1f);
        Restart();
        yield return new WaitForSeconds(0.1f);
        BroadcastNextTurn();
    }

    void AddFlingRequirements(GameObject inst)
    {
        inst.AddComponent<FlingPhysics>();
        var circle = inst.AddComponent<CircleCollider2D>();
        var prefab = GameManager.Instance.TouchColliderPrefab.GetComponent<CircleCollider2D>();
        circle.radius = prefab.radius;
        circle.offset = prefab.offset;
        circle.isTrigger = prefab.isTrigger;
    }

    void AddRigidbody(GameObject inst, bool isStatic = false)
    {
        var rb = inst.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.useFullKinematicContacts = true;            
        rb.constraints = isStatic ? RigidbodyConstraints2D.FreezeAll : RigidbodyConstraints2D.FreezeRotation;        
        rb.gravityScale = 0.0f;        
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    public void CameraFollow(Transform target)
    {
        CameraPan.Instance.CameraTarget.SetParent(target);
    }

    public void Restart() 
    {
        foreach (var g in _Instantiated) Destroy(g);
        _Instantiated.Clear();

        var collections = GameObject.FindGameObjectWithTag("CharacterCollections").GetComponent<CharacterCollections>();
        var friendlyStarts = GameObject.FindGameObjectWithTag("FriendlyStarter").transform;
        var enemyStarts = GameObject.FindGameObjectWithTag("EnemyStarter").transform;
        var obstacleStarts = GameObject.FindGameObjectWithTag("Obstacles").transform;

        var freeAllStarts = new List<Location>();

        var freeFriendStarts = new List<Transform>();
        foreach (Transform t in friendlyStarts)
        {
            freeFriendStarts.Add(t);
        }

        var freeEnemyStarts = new List<Transform>();
        foreach (Transform t in enemyStarts)
        {
            freeEnemyStarts.Add(t);
            freeAllStarts.Add(new Location() { kind = LocationKind.Enemy, place = t });
        }

        var freeObstacleStarts = new List<Transform>();
        foreach (Transform t in obstacleStarts)
        {
            freeObstacleStarts.Add(t);
            freeAllStarts.Add(new Location() { kind = LocationKind.Obstacle, place = t });
        }

        // ARBITER
        var arbiter = new GameObject("Arbiter");
        arbiter.AddComponent<TurnArbiterController>();
        var tbu = arbiter.AddComponent<TurnBasedUnit>();
        tbu.ConnectController();
        BroadcastTurnOrderRegistration(tbu);

        // OBSTACLES
        for (int i = 0; i < Random.Range(8, 20); i++)
        {
            var place = freeAllStarts[Random.Range(0, freeAllStarts.Count)];
            freeAllStarts.Remove(place);

            switch(place.kind)
            {
                case LocationKind.Enemy: freeEnemyStarts.Remove(place.place); break;
                case LocationKind.Obstacle: freeObstacleStarts.Remove(place.place); break;
            }

            var prefab = collections.GetRandomObstacle();
            var inst = Instantiate(prefab, place.place.position, Quaternion.identity);
            inst.AddComponent<DepthSort>().OnlyOnStart = true;
            _Instantiated.Add(inst);

            AddRigidbody(inst, true);

            var turn = inst.GetComponent<TurnBasedUnit>();
            BroadcastTurnOrderRegistration(turn);
        }

        // PARTY
        int partySize = Random.Range(3, 5);
        for (int i = 0; i < partySize; i++)
        {
            var place = freeFriendStarts[Random.Range(0, freeFriendStarts.Count)];
            freeFriendStarts.Remove(place);
            var prefab = collections.GetRandomFriendly();
            var inst = Instantiate(prefab, place.position, Quaternion.identity);
            inst.AddComponent<DepthSort>();
            _Instantiated.Add(inst);

            AddFlingRequirements(inst);
            inst.AddComponent<PlayerFlingController>();
            AddRigidbody(inst);
            inst.GetComponent<TurnBasedUnit>().ConnectController();

            var hud = Instantiate(Config.Overhead, inst.transform);
            var turn = inst.GetComponent<TurnBasedUnit>();
            BroadcastTurnOrderRegistration(turn);
        }

        // ENEMIES
        for (int i = 0; i < Random.Range(partySize - 1, partySize + 10); i++)
        {
            var place = freeEnemyStarts[Random.Range(0, freeEnemyStarts.Count)];
            freeEnemyStarts.Remove(place);
            var prefab = collections.GetRandomEnemy();
            var inst = Instantiate(prefab, place.position, Quaternion.identity);
            inst.AddComponent<DepthSort>();
            _Instantiated.Add(inst);

            AddFlingRequirements(inst);
            inst.AddComponent<EnemyAIController>();
            AddRigidbody(inst);
            inst.GetComponent<TurnBasedUnit>().ConnectController();

            var hud = Instantiate(Config.Overhead, inst.transform);
            var turn = inst.GetComponent<TurnBasedUnit>();
            BroadcastTurnOrderRegistration(turn);
        }
    }

    private void OnGUI()
    {        
        if (GUI.Button(new Rect(100, 100, 200, 50), "Next Turn"))
        {
            GameManager.BroadcastNextTurn();
        }

        if (GUI.Button(new Rect(400, 100, 200, 50), "Export Event Graph"))
        {
            EventSockets.ExportGraph();
        }
    }

}
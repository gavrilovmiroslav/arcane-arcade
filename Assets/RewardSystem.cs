using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardSystem 
    : MonoBehaviour
    , IReactsToEvent<IUnitKilledPropertyChanged.EnemyKilled>
{
    public static RewardSystem Instance;

    public List<GameObject> Rewards = new();

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {        
        DeathSystem.Instance.OnEnemyKilled += React_OnEnemyKilled;
    }

    private void React_OnEnemyKilled(UnitScriptable unit, Vector3 position)
    {
        Debug.Log($"{unit} killed");
    }
}

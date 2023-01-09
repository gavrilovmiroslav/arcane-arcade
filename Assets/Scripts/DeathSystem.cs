using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DeathSystem 
    : MonoBehaviour
    , IUnitKilledPropertyChanged
    , IReactsToEvent<IGamePropertyChanged.TurnCleanup>
{
    public static DeathSystem Instance;
    public GameObject DeathPrefab;

    public event IUnitKilledPropertyChanged.EnemyKilled OnEnemyKilled;
    public event IUnitKilledPropertyChanged.FriendKilled OnFriendKilled;

    public void Awake()
    {
        Instance = this;
        GameManager.Instance.OnTurnCleanup += React_OnTurnCleanup;
    }

    private void React_OnTurnCleanup()
    {
        TurnManager.Instance.TurnCleanupTokens++;
        var turnOrder = TurnManager.Instance.TurnOrder;
        var dead = turnOrder.Where(t => t.Health <= 0).ToList();

        if (dead.Count > 0)
            StartCoroutine(DestroyAllDeadCo(dead));
        else
            TurnManager.Instance.TurnCleanupTokens--;
    }

    private IEnumerator DestroyAllDeadCo(List<TurnBasedUnit> dead)
    {        
        foreach (var t in dead)
        {
            if (t != null)
            {
                CameraPan.Instance.PanTo(t.transform.position);
                yield return new WaitForSeconds(1.2f);
                Destroy(Instantiate(DeathPrefab, t.transform.position, Quaternion.identity), 1.0f);

                if (t.Definition != null)
                {
                    if (t.Definition.Friend) 
                        OnFriendKilled?.Invoke(t.Definition, t.transform.position);
                    else
                        OnEnemyKilled?.Invoke(t.Definition, t.transform.position);
                }

                yield return new WaitForSeconds(0.1f);
                Destroy(t.gameObject);
                yield return new WaitForSeconds(1.0f);
            }
        }

        TurnManager.Instance.TurnOrder = new Queue<TurnBasedUnit>(TurnManager.Instance.TurnOrder.Where(t => t != null && t.Health > 0));        
        TurnManager.Instance.TurnCleanupTokens--;
    }
}

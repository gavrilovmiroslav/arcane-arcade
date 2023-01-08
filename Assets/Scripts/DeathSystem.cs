using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DeathSystem 
    : MonoBehaviour
    , IReactsToEvent<IGamePropertyChanged.TurnCleanupStarted>
{
    public void Awake()
    {
        GameManager.Instance.OnTurnCleanupStarted += React_OnTurnCleanupStarted;
    }

    private void React_OnTurnCleanupStarted()
    {
        var turnOrder = TurnManager.Instance.TurnOrder;
        foreach(var t in turnOrder.Where(t => t.Health <= 0))
        {
            if (t != null)
                Destroy(t.gameObject);
        }

        TurnManager.Instance.TurnOrder = new Queue<TurnBasedUnit>(turnOrder.Where(t => t != null && t.Health > 0));
    }
}

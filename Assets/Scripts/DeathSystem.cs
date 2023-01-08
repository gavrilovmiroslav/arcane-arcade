using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DeathSystem 
    : MonoBehaviour
    , IReactsToEvent<IGamePropertyChanged.PrepareForTurnCleanup>
    , IReactsToEvent<IGamePropertyChanged.TurnCleanup>
{
    public GameObject DeathPrefab;

    public void Awake()
    {
        GameManager.Instance.OnPrepareForTurnCleanup += React_OnPrepareForTurnCleanup;
        GameManager.Instance.OnTurnCleanup += React_OnTurnCleanup;
    }

    private void React_OnPrepareForTurnCleanup()
    {
        TurnManager.Instance.TurnCleanupTokens++;
    }

    private void React_OnTurnCleanup()
    {
        var turnOrder = TurnManager.Instance.TurnOrder;
        var dead = turnOrder.Where(t => t.Health <= 0).ToList();

        if (dead.Count > 0)
            StartCoroutine(DestroyAllDeadCo(dead));
        else
            TurnManager.Instance.TurnCleanupTokens--;
    }

    private IEnumerator DestroyAllDeadCo(List<TurnBasedUnit> dead)
    {
        var oldPan = CameraPan.CanPan;
        CameraPan.CanPan = false;
        foreach (var t in dead)
        {
            if (t != null)
            {
                CameraPan.Instance.PanTo(t.transform.position);
                yield return new WaitForSeconds(0.8f);
                Destroy(Instantiate(DeathPrefab, t.gameObject.transform.position, Quaternion.identity), 1.0f);
                yield return new WaitForSeconds(0.1f);
                Destroy(t.gameObject);
                yield return new WaitForSeconds(0.5f);
            }
        }

        TurnManager.Instance.TurnOrder = new Queue<TurnBasedUnit>(TurnManager.Instance.TurnOrder.Where(t => t != null && t.Health > 0));
        CameraPan.CanPan = oldPan;

        TurnManager.Instance.TurnCleanupTokens--;
    }
}

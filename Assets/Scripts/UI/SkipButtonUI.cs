using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipButtonUI : MonoBehaviour
{
    private BoxCollider2D _Collider;
    private bool _Tapped = false;

    public void OnMouseExit()
    {
        _Tapped = false;
    }

    public void Start()
    {
        _Collider = GetComponent<BoxCollider2D>();
    }

    public void Update()
    {
        var touchInside = _Collider.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (touchInside)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _Tapped = true;
                UnitCardUI.IsTapped = true;
            }
            else
            {
                if (_Tapped)
                {
                    UnitCardUI.IsTapped = false;
                    GameManager.BroadcastNextTurn();
                    _Tapped = false;
                }
            }
        }        
    }
}

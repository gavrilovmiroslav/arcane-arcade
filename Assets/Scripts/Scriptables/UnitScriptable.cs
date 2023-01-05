using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UnitScriptable : ScriptableObject
{
    public bool Friend = false;
    public int Power = 1;
    public int Health = 1;
    public Sprite Image;

    public string Name;
    public OnHitScriptable SuccessfulAttack = null;
    public OnHitScriptable TakingDamage = null;
}
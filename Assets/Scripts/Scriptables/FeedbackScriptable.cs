using System.Collections;

using UnityEngine;

[System.Serializable]
public class FeedbackScriptable : ScriptableObject
{
    public bool ShouldActivateWhenTargetIsNone = false;

    public virtual IEnumerator Action() { yield return null; }
}
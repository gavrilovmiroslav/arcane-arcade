using System.Collections;

using UnityEngine;

[CreateAssetMenu(menuName = "Feedbacks/Time Stop")]
public class TimeStopFeedback : FeedbackScriptable
{
    public float StopTimeDuration = 0.1f;

    public override IEnumerator Action()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(StopTimeDuration);
        Time.timeScale = 1;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionManager : MonoBehaviour
{
    public static EmotionManager Instance;
    public GameObject EmotionPrefab;
    public float Scale = 0.25f;
    public Vector2 Offset;
    
    public void Awake()
    {
        Instance = this;
    }

    public Coroutine Emote<T>(T self, EmotionType emo, int repeats = 1) where T: MonoBehaviour 
    {
        var emotion = Instantiate(EmotionPrefab, self.transform.position + new Vector3(Offset.x, Offset.y, 0.0f), Quaternion.identity);
        emotion.transform.SetParent(self.transform);
        var emote = emotion.GetComponent<EmotionCom>();
        emotion.transform.localScale *= Scale;
        return emote.PlayEmote(emo, repeats);
    }
}

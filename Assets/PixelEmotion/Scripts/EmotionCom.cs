using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionCom : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public Coroutine PlayEmote(EmotionType emo, int repeats)
    {
        return StartCoroutine(Play(emo, repeats));
    }

    protected IEnumerator Play(EmotionType emotionType, int repeats)
    {   
        string enumName = GetEnumName<EmotionType>((int)emotionType);
        animator.Play(enumName, 0, 0f);
        yield return new WaitForSeconds(1.2f * repeats);
        Destroy(this.gameObject);
    }

    public string GetEnumName<T>(int value)
    {
        string name = "";
        name = Enum.Parse(typeof(T), Enum.GetName(typeof(T), value)).ToString();
        return name;
    }


}

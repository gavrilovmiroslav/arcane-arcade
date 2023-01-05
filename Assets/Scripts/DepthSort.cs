using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthSort : MonoBehaviour
{
    public bool OnlyOnStart = false;
    private SpriteRenderer _Renderer;

    void Resort()
    {
        var value = 32768 - (int)(100 * (151 + transform.position.y));
        _Renderer.sortingOrder = value;
    }

    void Start()
    {
        _Renderer = GetComponent<SpriteRenderer>();
        Resort();
        if (OnlyOnStart)
            Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        Resort();
    }
}

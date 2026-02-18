using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundScroll : MonoBehaviour
{
    [SerializeField] float backGroundScrollSpeed = 0.02f;
    Material background;
    Vector2 bgOffset;
    void Start()
    {
        background = GetComponent<Renderer>().material;
        bgOffset = new Vector2(0, backGroundScrollSpeed);
    }

    void Update()
    {
        background.mainTextureOffset += bgOffset * Time.deltaTime;
    }
}

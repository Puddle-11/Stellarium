using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ColorLerpTest : MonoBehaviour
{
    public Color StartCol;
    public Color CurrentCol;
    public Color EndCol;
    public bool Lerping;
    public float timer;
    public float dur;

    // Update is called once per frame
    void Update()
    {
        if (Lerping)
        {
            timer += Time.deltaTime;
            CurrentCol = Color.Lerp(StartCol, EndCol, timer / dur);
        }
        else
        {
            timer = 0;
        }
    }
}

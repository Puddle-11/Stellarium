using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitDraw : MonoBehaviour
{
    public int lineResolution;
    public LineRenderer LN;
    public float Radius;
    // Start is called before the first frame update
    void Start()
    {
        LN.positionCount = lineResolution;
    }

    // Update is called once per frame
    void Update()
    {
        drawCircle(Radius);
        
    }
    public void drawCircle(float radius)
    {
        for (int i = 0; i < LN.positionCount; i++)
        {
            float circumferenceProg = (float) i / LN.positionCount;
            float rad = circumferenceProg * 2 * Mathf.PI;
            Vector3 pos = new Vector3(Mathf.Sin(rad) * radius, 0, Mathf.Cos(rad) * radius);
            LN.SetPosition(i, pos);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class SimulationVariables : MonoBehaviour
{
    public static SimulationVariables SimRef;
    public float GravitationalConstant;
    public float TimeScale;
    private void Update()
    {
        TimeScale = Mathf.Clamp(TimeScale, 0.1f, Mathf.Infinity);
        if (TimeScale != Time.timeScale)
        {
            Time.timeScale = TimeScale;
        }
    }
    public void Awake()
    {
        if(SimRef == null)
        {
            SimRef = gameObject.GetComponent<SimulationVariables>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

}

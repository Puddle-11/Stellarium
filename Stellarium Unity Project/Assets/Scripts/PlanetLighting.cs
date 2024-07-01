using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetLighting : MonoBehaviour
{
    public Color BaseColor;
    private MeshRenderer MR;
    public GameObject Star;
    
    void Update()
    {
            if (MR == false)
            {
                TryGetComponent<MeshRenderer>(out MR);
            }
        if (SimulationVariables.SimRef.lighting)
        {
            MR.material.color = CalculateColor(Mathf.Pow(Vector3.Distance(transform.position, Star.transform.position), SimulationVariables.SimRef.lightFalloff) / SimulationVariables.SimRef.lightBrightness);
        }
        else
        {
            MR.material.color = BaseColor;
        }
    }
    public Color CalculateColor(float _weight)
    {
        Color res = Color.black;

        res.r = (BaseColor.r + (SimulationVariables.SimRef.DarkColor.r * _weight))  / (1 + _weight);
        res.g = (BaseColor.g + (SimulationVariables.SimRef.DarkColor.g * _weight))  / (1 + _weight);
        res.b = (BaseColor.b + (SimulationVariables.SimRef.DarkColor.b * _weight))  / (1 + _weight);

        return res;
    }
}

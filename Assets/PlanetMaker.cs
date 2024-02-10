using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.Mathematics;
using Unity.VisualScripting;

public class PlanetMaker : MonoBehaviour
{
    public TMP_InputField planetSizeInput;
    public GameObject planet;
    private float planetSize = 1;
    public Vector2 minMaxPlanetSize;
    public Vector2 minMaxRingSize;
    private float ringSize = 1;
    public GameObject rings;
    public TMP_InputField planetRingInput;
    public TMP_InputField axisInput_x;
    public TMP_InputField axisInput_y;
    private Vector3 Axis;


    // Update is called once per frame
    void Update()
    {
        if (float.TryParse(planetSizeInput.text, out planetSize))
        {
            float planetSizeClamped = Mathf.Clamp(planetSize, minMaxPlanetSize.x, minMaxPlanetSize.y);
            planet.transform.localScale = new Vector3(planetSizeClamped, planetSizeClamped, planetSizeClamped);
        }
      
        if (float.TryParse(planetRingInput.text, out ringSize))
        {
            if (ringSize == 0)
            {
                rings.transform.localScale = new Vector3(0, 0, rings.transform.localScale.z);

            }
            else
            {
                float ringClamped = Mathf.Clamp(ringSize, minMaxRingSize.x, minMaxRingSize.y);
                rings.transform.localScale = new Vector3(ringClamped + 50, ringClamped + 50, rings.transform.localScale.z);
            }
        }
    }
}

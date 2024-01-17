using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlanetCreator : MonoBehaviour
{
    public GameObject SystemObjectRef;
    public GameObject BasePlanet;
    public Vector2 DefaultDistanceRange;
    private SystemManager SystemManagerRef;
    [SerializeField] private float DefaultMass;
    [SerializeField] private Vector2 DefaultVolume;
    [SerializeField] private Vector2 DefaultAxis;
    [SerializeField] private float minDistance;
    private float currentVolume = 0f;
    private float currentDistanceFromSun = 0f;
    private float currentMass = 0f;
    public Vector2 currentAxisTilt = Vector2.zero;

    public float currentOrbitalSpeed = 100f;
    public bool Def;
    public TMP_InputField VolumeField;
    public TMP_InputField DistanceField;
    public TMP_InputField AxisTileField;
    public TMP_InputField OrbitalSpeedField;
    public EventSystem UIEventSys;
    bool currentOrbitType = true;
    public Vector2 VolumeMinMax;
    public Vector2 OrbitSpeedMinMax;

    private void Start()
    {
        SystemManagerRef = SystemObjectRef.GetComponent<SystemManager>();
       
    }
    public void OnButtonPress()
    {
        Debug.Log("generate");
        Generate(currentVolume, currentDistanceFromSun, DefaultMass, currentAxisTilt, currentOrbitalSpeed, currentOrbitType);
    }
    public void OrbitalTypeToggle(bool Tog)
    {
        currentOrbitType = Tog;
    }
    private void Update()
    {
        if (float.TryParse(VolumeField.text, out currentVolume))
        {
            currentVolume = Mathf.Clamp(currentVolume, VolumeMinMax.x, VolumeMinMax.y);
            
        }
        if(VolumeField.isFocused == false) VolumeField.text = currentVolume.ToString();
      
        if (float.TryParse(DistanceField.text, out currentDistanceFromSun))
        {
            currentOrbitalSpeed = Mathf.Clamp(currentOrbitalSpeed, 5, 30);

        }
      
         if (DistanceField.isFocused == false) DistanceField.text = currentDistanceFromSun.ToString();
        if (float.TryParse(AxisTileField.text, out currentAxisTilt.x))
        {
        }
        else if (AxisTileField.isFocused == false) AxisTileField.text = currentAxisTilt.x.ToString();
        if (float.TryParse(OrbitalSpeedField.text, out currentOrbitalSpeed))
        {
            currentOrbitalSpeed = Mathf.Clamp(currentOrbitalSpeed, OrbitSpeedMinMax.x, OrbitSpeedMinMax.y);
        }


        if (OrbitalSpeedField.isFocused == false) OrbitalSpeedField.text = currentOrbitalSpeed.ToString();

    }



    public void Generate(float _Volume, float _Distance, float _Mass, Vector2 _AxisTilt, float _OrbitalSpeed, bool _AutoOrbit)
    {
       

        Vector3 SpawnPos = new Vector3();

        if(SystemManagerRef.SysType == SystemManager.Systemtype.SingleBody) {

                SpawnPos = SystemManagerRef.BodiesInSystem[0].b_transform.position;
                if (_Distance == 0)
                {

                    SpawnPos.x += Random.Range(DefaultDistanceRange.x, DefaultDistanceRange.y) + minDistance;

                }
                else
                {

                    SpawnPos.x += _Distance + minDistance;

                }

                GameObject x = Instantiate(BasePlanet, SpawnPos, Quaternion.identity, SystemObjectRef.transform);
                if (_Mass == 0)
                {
                    x.GetComponent<Rigidbody>().mass = DefaultMass;
                }
                else
                {
                    x.GetComponent<Rigidbody>().mass = _Mass;

                }
                if (_Volume == 0)
                {
                    float Volume = Random.Range(DefaultVolume.x, DefaultVolume.y);
                    x.transform.localScale = new Vector3(Volume, Volume, Volume);

                }
                else
                {
                    x.transform.localScale = new Vector3(_Volume, _Volume, _Volume);

                }


                Gravity XGravRef = x.GetComponent<Gravity>();
                Vector3 Axis = Vector3.zero;
                if (_AxisTilt == Vector2.zero)
                {
                    Axis = Vector3.Normalize(new Vector3(DefaultAxis.x, DefaultAxis.y, 1));

                }
                else
                {
                    Axis = Vector3.Normalize(new Vector3(0, _AxisTilt.x, 1));

                }

                XGravRef.StartVelocity = Axis;

                if (_AutoOrbit)
                {
                    XGravRef.StartVelocityType = Gravity.StartVelType.Auto;
                }
                else
                {
                    XGravRef.StartVelocityType = Gravity.StartVelType.Manual;
                }

                XGravRef.StartvelFactor = _OrbitalSpeed;



            }
         
    }
  

}

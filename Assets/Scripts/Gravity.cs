using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
public class Gravity : MonoBehaviour
{
     [HideInInspector] public SystemManager SysManager; //Assigned system manager

    [HideInInspector] public Rigidbody RB;

    [HideInInspector] public float Mass;

    public string bodyName = "";


    //ONLY USE AUTO START VELOCITY TYPE IF SIMULATION IS SINGLEBODY
    //Auto sets the startvelocity to create a desireable orbit
    //None doesnt add any start velocity
    //Manual lets you customize the start velocity
    public StartVelType StartVelocityType;

    //Gives the start velocity direction. this value is normalized on auto but not on manual, increasing the numbers higher than 1 or lower than -1 will result in faster orbits
    public Vector3 StartVelocity;

    //100 gives best results however if you want larger orbits increase the value, for more eliptical orbits decrease the value, this is especially useful when on auto
    public float StartvelFactor = 100;
    public enum StartVelType
    {
        Auto,
        Manual,
        None,

    }
    private void Awake()
    {
        RB = GetComponent<Rigidbody>();


        Mass = RB.mass;
    }
    private void FixedUpdate()
    {
        /*
        if(StartVelocityType == StartVelType.Auto)
        {
            StartvelFactor = 100;
        }
        */
        CalculateGravity();
    }
    private void Start()
    {
      
        //I didnt know if there was a late start method so i made my own. Optimize at your own risk
        StartCoroutine(LateStart(0.01f));
    }
    public IEnumerator LateStart(float waittime)
    {
        SysManager = GetComponentInParent<SystemManager>();
        RB.useGravity = false;
        RB.angularDrag = 0;
        RB.drag = 0;
        yield return new WaitForSeconds(waittime);

        if (StartVelocityType == StartVelType.Auto)
        {
            //since auto is only applied on singlebody systems, it checks to make sure its not applying start velocity to the primary body
            if (SysManager.SysType == SystemManager.Systemtype.SingleBody)
            {
                if (this != SysManager.BodiesInSystem[0].b_gravity )
                {
                    if(StartVelocity != Vector3.zero)
                    {
                        float StartForce = Mathf.Sqrt(SysManager.BodiesInSystem[0].b_mass * SysManager.LocalgravityScale / (Vector3.Distance(SysManager.BodiesInSystem[0].b_transform.position, transform.position) /** 2.6f*/)) /*+ Vector3.Distance(SysManager.BodiesInSystem[0].b_transform.position, transform.position) / 50 - 1*/;
                        Vector3 StartVel = Vector3.Normalize(StartVelocity) * StartForce * StartvelFactor;

                    RB.AddForce(StartVel);
                    }
                    else
                    {
                        Debug.LogWarning("Failed to apply start velocity, please set StartVelocity to a non-zero value");
                    }
                    //Applying Auto-orbit velocity math
                }
            }
            else
            {
                Debug.LogWarning("AutoOrbit only works on single body systems, defaulted to manual");
                //Default to manual if not on a single body system
                RB.AddForce(StartVelocity * StartvelFactor * Mass);
            }
        }
        else if (StartVelocityType == StartVelType.Manual)
        {
            RB.AddForce(StartVelocity * StartvelFactor * Mass);
        }
    }
    public void CalculateGravity()
    {
        Vector3 Force = Vector3.zero;
        //Checking to see if this body is inside the system, if outside the bounds, apply force towards the center of the system
        if(Vector3.Distance(transform.position, SysManager.transform.position) > SysManager.SystemSize)
        {
            gameObject.SetActive(false);
            // Vector3 dir = Vector3.Normalize(SysManager.transform.position - transform.position);
            //Force += dir * SysManager.SystemSafeForce * Mass;
        }
        //Loop through all the bodies in system and average the force between them
        //the Big O notation is O((n-1)n) keep each system bellow 50 bodies, feel free to optimize i couldnt find a way to, while maintaining accuracy
        for (int i = 0; i < SysManager.BodiesInSystem.Count; i++)
        {
            //Skip this object when applyinf force
            if (SysManager.BodiesInSystem[i].b_gravity != this)
            {
                float distance = Vector3.Distance(SysManager.BodiesInSystem[i].b_transform.position, transform.position);
                float tempForce = (SysManager.LocalgravityScale * Mass * SysManager.BodiesInSystem[i].b_mass) / Mathf.Pow(distance, 2);
                Vector3 dir = Vector3.zero;
                //If object is within repulsion distance then reverse the gravity direction
                if (distance < SysManager.RepulsionDistance)
                {
                     dir = Vector3.Normalize(transform.position - SysManager.BodiesInSystem[i].b_transform.position);
                }
                else
                {
                     dir = Vector3.Normalize(SysManager.BodiesInSystem[i].b_transform.position - transform.position);
                }
                    Force += dir * tempForce;
            }
        }
        RB.AddForce(Force);
    }
}

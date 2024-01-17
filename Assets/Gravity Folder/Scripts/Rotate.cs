using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{

    public float speed;
    public int dir;
    public Vector3 Axis;
    void Update()
    {
       
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x + (Axis.x * Time.deltaTime * speed), transform.localEulerAngles.y + (Axis.y *Time.deltaTime * speed), transform.localEulerAngles.z + (Axis.z * Time.deltaTime * speed ));
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    public float offset;
    public Camera MainCam;
    
    // Update is called once per frame
    void Update()
    {
        transform.position =  MainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, offset));
    }
}

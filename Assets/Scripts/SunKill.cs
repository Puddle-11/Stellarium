using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunKill : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Planet" || other.tag == "Asteroid")
        {
            Destroy(other.gameObject);
        }
    }
}

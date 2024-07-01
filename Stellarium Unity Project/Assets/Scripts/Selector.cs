using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Selector : MonoBehaviour
{
    [SerializeField] private Camera CM;
    private RaycastHit hit;
    [SerializeField] private LayerMask LM;
    [SerializeField] private Transform RaycastDir;
    public Vector3 Screenpos;
    public Vector3 Worldpos;
    [SerializeField] private Camera UICam;
    [SerializeField] private GameObject testUI;
    public Vector3 SelectedObjPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        Screenpos = Input.mousePosition;
        
        Worldpos = CM.ScreenToWorldPoint(Screenpos);

        Vector3 dir = RaycastDir.position - transform.position;
        if (Physics.Raycast(CM.ScreenToWorldPoint(Input.mousePosition), dir, out hit, Mathf.Infinity, LM))
        {
            SelectedObjPos = UICam.WorldToScreenPoint(hit.collider.gameObject.transform.position);
        }

    }
    
}

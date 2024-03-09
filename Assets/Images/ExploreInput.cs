using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExploreInput : MonoBehaviour
{
    private bool selected = false;
    [SerializeField] private EventSystem EV;
    [SerializeField] private Button ClickButton;
    [SerializeField] private GameObject[] Edges;
    private Vector3[] edgeStartPos;
    [SerializeField] private Vector3[] edgeEndPos;
    [SerializeField] private float edgeChangeSpeed;
    private float edgeLerpTimer;
    [SerializeField] private PlanetCreator PC;
    private void Start()
    {
        edgeStartPos = new Vector3[Edges.Length];

        for (int i = 0; i < Edges.Length; i++)
        {
            edgeStartPos[i] = Edges[i].transform.localPosition;
        }

    }

    void Update()
    {
        edgeLerpTimer += Time.deltaTime;

        if(EV.currentSelectedGameObject == ClickButton.gameObject && Input.GetMouseButton(0))
        {
            if (selected == false)
            {
                edgeLerpTimer = 0;
                selected = true;
            }
           
        }
        if (Input.GetMouseButtonUp(0) && selected)
        {
            PC.RunExplore();
            edgeLerpTimer = 0;
            selected = false;
        }
        if (selected)
        {
            for (int i = 0; i < Edges.Length; i++)
            {
                Edges[i].transform.localPosition = Vector3.Lerp(Edges[i].transform.localPosition, edgeEndPos[i], edgeLerpTimer * edgeChangeSpeed);
            }
        }
        else
        {
            for (int i = 0; i < Edges.Length; i++)
            {
                Edges[i].transform.localPosition = Vector3.Lerp(Edges[i].transform.localPosition, edgeStartPos[i], edgeLerpTimer * edgeChangeSpeed);
            }
        }
        if (selected)
        {
            if (Input.anyKeyDown)
            {

            }



        }
    }
}

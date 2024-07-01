using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FreeCameraInput : MonoBehaviour
{
    private EventSystem EV;
    [SerializeField] private Button ClickButton;
    [SerializeField] private GameObject[] Edges;
    [SerializeField] private Vector3[] edgeEndPos;
    [SerializeField] private float edgeChangeSpeed;
    [SerializeField] private CameraRotate cameraScr;
    [SerializeField] private Image icon;
    private bool selected = false;
    private Vector3[] edgeStartPos;
    private float edgeLerpTimer;
    private bool freeState;
    [SerializeField] private Color dissableColor;
    [SerializeField] private Color enableColor;

    private void Start()
    {
        EV = EventSystem.current;
        edgeStartPos = new Vector3[Edges.Length];

        for (int i = 0; i < Edges.Length; i++)
        {
            edgeStartPos[i] = Edges[i].transform.localPosition;
        }

    }

    void Update()
    {
        if (freeState && icon.color != enableColor)
        {
            icon.color = enableColor;
        }
        else if(!freeState && icon.color != dissableColor)
        {
            icon.color = dissableColor;
        }



        edgeLerpTimer += Time.deltaTime;

        if (EV.currentSelectedGameObject == ClickButton.gameObject && Input.GetMouseButton(0))
        {
            if (selected == false)
            {
                edgeLerpTimer = 0;
                selected = true;
            }

        }
        if (Input.GetMouseButtonUp(0) && selected)
        {
            freeState = !freeState;
            cameraScr.isControlable = freeState;
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
   
    }
}

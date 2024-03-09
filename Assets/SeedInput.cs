using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SeedInput : MonoBehaviour
{
    [SerializeField] private KeyCode[] acceptableKeys;
    private bool selected = false;
    [SerializeField] private EventSystem EV;
    [SerializeField] private Button ClickButton;
    [SerializeField] private GameObject[] Edges;
    private Vector3[] edgeStartPos;
    [SerializeField] private Vector3[] edgeEndPos;
    [SerializeField] private float edgeChangeSpeed;
    private float edgeLerpTimer;
    [SerializeField] private TextMeshProUGUI TextBox;
    [SerializeField] private PlanetCreator PC;
    private float tickTimer;
    [SerializeField] private float tickSpeed;
    private bool tickState = false;
    private string finalText;
    private string internalText = "";
    public int maxSeedLength;
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
        EdgeMove();
        if (selected)
        {
            UpdateTicker(tickState);
            tickTimer += Time.deltaTime;
            if (tickTimer >= tickSpeed)
            {
                tickTimer = 0;

                tickState = !tickState;
            }

            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
                {
                    if (internalText.Length > 0)
                    {
                        updateString(internalText.Remove(internalText.Length - 1, 1));

                    }
                    else
                    {
                        updateString(internalText);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    Debug.Log("exited via Return");

                    ExitBox();

                }
                else if(internalText.Length < maxSeedLength)
                {


                    foreach (KeyCode kCode in acceptableKeys)
                    {
                        if (Input.GetKeyDown(kCode))
                        {
                            switch (kCode)
                            {

                                case KeyCode.Alpha0:
                                case KeyCode.Keypad0:
                                    updateString(internalText + 0);
                                    break;
                                case KeyCode.Alpha1:
                                case KeyCode.Keypad1:
                                    updateString(internalText + 1);
                                    break;
                                case KeyCode.Alpha2:
                                case KeyCode.Keypad2:
                                    updateString(internalText + 2);
                                    break;
                                case KeyCode.Alpha3:
                                case KeyCode.Keypad3:
                                    updateString(internalText + 3);
                                    break;
                                case KeyCode.Alpha4:
                                case KeyCode.Keypad4:
                                    updateString(internalText + 4);
                                    break;
                                case KeyCode.Alpha5:
                                case KeyCode.Keypad5:
                                    updateString(internalText + 5);
                                    break;
                                case KeyCode.Alpha6:
                                case KeyCode.Keypad6:
                                    updateString(internalText + 6);
                                    break;
                                case KeyCode.Alpha7:
                                case KeyCode.Keypad7:
                                    updateString(internalText + 7);
                                    break;
                                case KeyCode.Alpha8:
                                case KeyCode.Keypad8:
                                    updateString(internalText + 8);
                                    break;
                                case KeyCode.Alpha9:
                                case KeyCode.Keypad9:
                                    updateString(internalText + 9);
                                    break;

                                default:
                                    updateString(internalText + kCode.ToString());
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }


    private void ExitBox()
    {
        PC.seed = internalText;
        PC.RunGenerate();
        selected = false;
        if (tickState)
        {
            finalText = finalText.Remove(finalText.Length - 1, 1);
        }
        tickState = false;
        edgeLerpTimer = 0;
        tickTimer = 0;
        TextBox.text = "";
        internalText = "";
        EV.SetSelectedGameObject(null);
    }
    public void updateString(string newString)
    {
        tickState = true;
        tickTimer = 0;
        internalText = newString;
        
        


    }
 private void UpdateTicker(bool state)
    {
               
                if (state == true)
                {
                    finalText = internalText + "_";
                }
                else
                {
                    finalText = internalText;
                }
            TextBox.text = finalText;
       

        
    }
    private void EdgeMove()
    {
        edgeLerpTimer += Time.deltaTime;
        if (EV.currentSelectedGameObject != ClickButton.gameObject)
        {
            if (selected == true)
            {

                ExitBox();
            }

            for (int i = 0; i < Edges.Length; i++)
            {
                Edges[i].transform.localPosition = Vector3.Lerp(Edges[i].transform.localPosition, edgeStartPos[i], edgeLerpTimer * edgeChangeSpeed);
            }
        }
        else
        {
            if (selected == false)
            {
                edgeLerpTimer = 0;
                selected = true;
            }
            for (int i = 0; i < Edges.Length; i++)
            {
                Edges[i].transform.localPosition = Vector3.Lerp(Edges[i].transform.localPosition, edgeEndPos[i], edgeLerpTimer * edgeChangeSpeed);
            }
        }
    }
}

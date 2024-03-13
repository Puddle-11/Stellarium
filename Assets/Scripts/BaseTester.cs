using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTester : MonoBehaviour
{
    public int OriginNum;
    public int Res;
    public int Base;
    public int[] newDigits;

    // Update is called once per frame
    void Update()
    {
        ConvertBase(OriginNum, Base);
    }
    public void ConvertBase(int Original, int _Base)
    {
        List<int> Digits = new List<int>();
        Digits.Add(Original);
        for (int p = 0; p < Digits.Count; p++)
        {
            if (Digits[p] % _Base != 0)
            {
                if(p+1 > Digits.Count - 1)
                {
                    Digits.Add(Digits[p] % _Base);

                }
                else
                {

                Digits.Insert(p + 1, Digits[p] % _Base);
                }
            }
            Digits[p] = (int)Digits[p] / _Base;
        }

        newDigits = Digits.ToArray();

    }
}

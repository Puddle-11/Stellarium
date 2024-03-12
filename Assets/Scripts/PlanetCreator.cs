using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using JetBrains.Annotations;
using System;
using UnityEngine.Rendering.Universal;
using System.Runtime.CompilerServices;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using NUnit.Framework;

public class PlanetCreator : MonoBehaviour
{
    [Space]
    [Space]
    [Header("General")]
    [SerializeField] public string seed;
    public string subSeed;
    [SerializeField] private GameObject SystemObjectRef;
    [SerializeField] private Volume PostProfile;
    [SerializeField] private GameObject BasePlanet;
    [SerializeField] private GameObject BaseAsteroid;
    [SerializeField] private GameObject star;
    [SerializeField] private GameObject BlackHole;
    [SerializeField] private TextMeshProUGUI SeedText;
    [SerializeField] private TextMeshProUGUI starNamefield;
    [SerializeField] private TextMeshProUGUI constellationNamefield;
    [SerializeField] private TextMeshProUGUI descriptionField;
    [SerializeField] private UnityEngine.UI.Image constelationImage;

    [Space]
    [Space]
    [Header("Generation Parameters")]
    [SerializeField] private int minRingSize;
    [SerializeField] private float defaultMass;
    [SerializeField] private float minMass;
    [SerializeField] private float defaultVolume;
    [SerializeField] private float minVolume;

    [Space]
    [Space]
    [Header("Star Parameters")]
    [SerializeField] private float starSize;
    [SerializeField] private float minStarSize;
    [SerializeField] private float minBlackHoleSize;

    [SerializeField] private float starChangeTime;
    [SerializeField] private float starChangeDelay;
    [Space]
    [Space]
    [Header("Terra Planets Parameters")]
    [SerializeField] private Vector2 terraPlanetSize;
    [SerializeField] private float startDistance;
    [SerializeField] private float terraRingClamp;
    [SerializeField] private float terraPlanetDensity;
    [Space]
    [Space]
    [Header("Gas Planets Parameters")]
    [SerializeField] private float gasRingClamp;
    [SerializeField] private Vector2 gasPlanetSize;
    [SerializeField] private float gasPlanetDensity;

    [Space]
    [Space]
    [Header("Asteroid Planets Parameters")]
    [SerializeField] private float asteroidBeltSize;
    [SerializeField] private int asteroidBeltDensity;
    [SerializeField] private float asteriodFalloff;
    [SerializeField] private Vector2 asteroidSize;
    [SerializeField] private Vector2 asteroidBeltRange;

    [Space]
    [Space]
    [Header("Colors")]
    [SerializeField] private Gradient PlanetColors;
    [SerializeField] private Gradient StarColor;
    [SerializeField] private Gradient AsteroidColor;



    private SystemManager SystemManagerRef;
    private Bloom _bloom;



    private System.Random RandGen = new System.Random();
    private int[] SeedDigits;
    private int[] FilteredSeed;
    private Vector2[] TerraRings;
    private Vector2[] GasRings;
    private Vector2[] AsteroidBelts;
    private List<GameObject> AllPlanets = new List<GameObject>();
    private bool Explore;
    private Color CurrentStarColor = Color.black;
    private int tempseed;
    private string StarName;
    public Constelation[] ConstelationList;
    private bool abletoGenerate = true;
    public string[] greekLettes;
    private string[] PlanetNames;
    public SpecialStar[] specialStars;
    private bool Quasar;
    private void Start()
    {
        PostProfile.profile.TryGet(out _bloom);
        SystemManagerRef = SystemObjectRef.GetComponent<SystemManager>();
        RunGenerate();

    }


    public void RunGenerate()
    {
        Explore = false;
        Debug.Log(seed);
        GenerateSystem(seed);
    }
    public void RunExplore()
    {
        Explore = true;
        Debug.Log(seed);

        GenerateSystem(seed);

    }

    private void Update()
    {

        SeedText.text = seed;
    }
    public void GenerateSystem(string _seed)
    {
        //=============================================
        //Digit 0: number of Terestrial planets (per ring)
        //Digit 1: number of gasious planets (per ring)
        //Digit 2: number of Rings Terestrial
        //Digit 3: number of Rings Gassious
        //Digit 4: Size of rings Range
        //Digit: 
        //Digit 5: Distance of Between rings (this effects how big the asteroid belts are)
        //Digit 8: density of asteroid belts
        //Digit 9: star size;
        //Digit 10-11: star Tempeture;
        //Digit 12: terra-gas belt
        //==============================================================
        //Initialize Generator
        //==============================================================
        #region Initialize Generator
        if (!abletoGenerate) return;
        abletoGenerate = false;
        SpecialStar SP = null;
        for (int i = 0; i < AllPlanets.Count; i++)
        {
            Destroy(AllPlanets[i]);
        }
        AllPlanets = new List<GameObject>();
        RandGen = new System.Random(seed.GetHashCode());
        #endregion

        //==============================================================
        //Generate Custom Stars
        //==============================================================
        #region Generate Custom Star Variables
        if (!Explore) // if e
        {
            for (int i = 0; i < specialStars.Length; i++)
            {
                if (seed == specialStars[i].name.ToUpper() || seed == specialStars[i].designatedSeed)
                {
                    seed = specialStars[i].designatedSeed;
                    subSeed = specialStars[i].designatedSeed;
                    Quasar = specialStars[i].isQuasar;
                    SP = specialStars[i];
                    break;
                }
            }
        }
        #endregion

        GenerateSeed();

        //==============================================================
        //Generate Quasars
        //==============================================================
        #region Generate Quasars
        int r = RandGen.Next(0, 100);
        if (r > 4) Quasar = false;
        else Quasar = true;
        BlackHole.SetActive(Quasar);
        Vector3 StarScale = Vector3.zero;
        if (!Quasar)
        {
            StarScale = Vector3.one * (FilteredSeed[8] * starSize + minStarSize);
        }
        else
        {
            BlackHole.transform.localScale = (Vector3.one * (FilteredSeed[8] * starSize + minBlackHoleSize)) / 2;
            StarScale = new Vector3(FilteredSeed[8] * starSize + minBlackHoleSize, 0.2f, FilteredSeed[8] * starSize + minBlackHoleSize);
        }
        #endregion

        //Start Generation Process
        generateStar(StarScale, StarColor.Evaluate((float)(FilteredSeed[9] * 10 + FilteredSeed[10]) / 100), SP);
    }


    #region Seed Generation and Filtration
    public void GenerateSeed()
    {
        subSeed = "";
        if (Explore)
        {
            seed = "";
            int[] res = new int[12];
            for (int i = 0; i < 12; i++)
            {
                res[i] = UnityEngine.Random.Range(1, 10);
                seed += res[i].ToString();
            }
            SeedDigits = res;
        }
        else
        {
            SeedDigits = new int[12];

            for (int x = 0; x < SeedDigits.Length; x++)
            {
                int temp;
                if (x < seed.Length && int.TryParse(seed[x].ToString(), out temp))
                {
                    SeedDigits[x] = temp;

                }
                else
                {
                    SeedDigits[x] = RandGen.Next(0, 10);
                }

            }
        }
        for (int i = 0; i < SeedDigits.Length; i++)
        {
            subSeed = subSeed + SeedDigits[i].ToString();
        }
        SeedText.text = subSeed;

        if (seed == null || seed == string.Empty) seed = subSeed;
        FilteredSeed = new int[SeedDigits.Length];
        FilteredSeed[0] = (int)Mathf.Round(Mathf.Clamp(SeedDigits[0] * terraPlanetDensity, 1, 10));
        FilteredSeed[1] = (int)Mathf.Round(Mathf.Clamp(SeedDigits[1] * gasPlanetDensity, 1, 10));
        FilteredSeed[2] = (int)Mathf.Round(Mathf.Clamp(SeedDigits[2] / terraRingClamp, 1, 10));
        FilteredSeed[3] = (int)Mathf.Round(Mathf.Clamp(SeedDigits[3] / gasRingClamp, 1, 10));
        FilteredSeed[4] = SeedDigits[4] + minRingSize;
        FilteredSeed[5] = SeedDigits[5];
        FilteredSeed[6] = (int)Mathf.Clamp((SeedDigits[6] * asteroidBeltSize), asteroidBeltRange.x, asteroidBeltRange.y);
        FilteredSeed[8] = SeedDigits[8];
        FilteredSeed[7] = SeedDigits[7];
        FilteredSeed[9] = SeedDigits[9];
        FilteredSeed[10] = SeedDigits[10];
        FilteredSeed[11] = SeedDigits[11];
    }
    #endregion



    public void generateStar(Vector3 _starSize, Color _starCol, SpecialStar _CustomStar)
    {
        int starIndex = RandGen.Next(0, ConstelationList.Length);
        bool isspecial = false;
        if (_CustomStar != null)
        {
            StarName = _CustomStar.name;
            constellationNamefield.text = _CustomStar.constellationName;
            descriptionField.text = _CustomStar.description;
            if (_CustomStar.useCustomStarColor)
            {
                float ATemp = _starCol.a;
                _starCol = _CustomStar.customStarColor;
                _starCol.a = ATemp;
            }
            for (int x = 0; x < ConstelationList.Length; x++)
            {
                if (ConstelationList[x].constelatioName == _CustomStar.constellationName)
                {
                    constelationImage.sprite = ConstelationList[x].ConstelationImage;

                }
            }

            isspecial = true;

        }
        if (isspecial == false)
        {

            descriptionField.text = "";
            StarName = greekLettes[RandGen.Next(0, ConstelationList[starIndex].starNum)] + " " + ConstelationList[starIndex].suffixName + "-" + ConvertB24(RandGen.Next(0, 24), 24) + RandGen.Next(0, 10000);
            constellationNamefield.text = ConstelationList[starIndex].constelatioName;
            constelationImage.sprite = ConstelationList[starIndex].ConstelationImage;
        }
        starNamefield.text = StarName;
        StopCoroutine("LerpStarColor");
        StopCoroutine("LerpStarSize");
        StartCoroutine(LerpStarSize(star.transform.localScale, _starSize, starChangeTime));

        StartCoroutine(LerpStarColor(CurrentStarColor, _starCol, starChangeTime));
    }


    public IEnumerator LerpStarSize(Vector3 _Start, Vector3 _End, float _Speed)
    {
        yield return new WaitForSeconds(starChangeDelay);

        float stimer = 0;
        Vector3 current = _Start;
        while (current != _End)
        {

            current = Vector3.Lerp(_Start, _End, stimer / _Speed);
            star.transform.localScale = current;
            yield return 0;
            stimer += Time.deltaTime;
        }
        yield return new WaitForSeconds(starChangeDelay);

    }
    public IEnumerator LerpStarColor(Color _Start, Color _End, float _Speed)
    {
        yield return new WaitForSeconds(starChangeDelay);
        bool CustomTint = false;
        Color CustomTintColor = Color.white;
        for (int i = 0; i < specialStars.Length; i++)
        {
            if (seed == specialStars[i].designatedSeed && specialStars[i].useCustomTint == true)
            {
                CustomTint = true;
                CustomTintColor = specialStars[i].customTint;
            }
        }
        float stimer = 0;
        MeshRenderer[] m = star.GetComponentsInChildren<MeshRenderer>();
        while (CurrentStarColor != _End)
        {

            CurrentStarColor = Color.Lerp(_Start, _End, stimer / _Speed);
            for (int i = 0; i < m.Length; i++)
            {
                m[i].material.color = CurrentStarColor;
            }
            m[0].material.color = Color.white;
            Color c = Color.white;
            c = CurrentStarColor;
            c.a = 255;

            ColorParameter CP = new ColorParameter(c, true);
            if (CustomTint)
            {
                CP = new ColorParameter(CustomTintColor, true);
            }

            _bloom.tint.SetValue(CP);
            yield return 0;
            stimer += Time.deltaTime;
        }
        yield return new WaitForSeconds(starChangeDelay);

        GenerateTerraRings();
        GenerateGasRings();
        generateAsteroidBelt();
        for (int i = 0; i < AllPlanets.Count; i++)
        {

            if (AllPlanets[i].gameObject.tag == "Planet")
            {

                AllPlanets[i].GetComponent<Gravity>().bodyName = StarName + "-" + ConvertB24(i + 1, 26);
            }
        }
        abletoGenerate = true;
    }

    private string ConvertB24(long decimalNumber, int radix)
    {
        const int BitsInLong = 64;
        const string Digits = "abcdefghijklmnopqrstuvwxyz";
        //======================
        //Saftey
        //======================
        if (radix < 2 || radix > Digits.Length)
            throw new ArgumentException("The radix must be >= 2 and <= " + Digits.Length.ToString());

        if (decimalNumber == 0)
            return "0";
        //======================


        int index = BitsInLong - 1;
        long currentNumber = Math.Abs(decimalNumber);
        char[] charArray = new char[BitsInLong];

        while (currentNumber != 0)
        {
            int remainder = (int)(currentNumber % radix);
            charArray[index--] = Digits[remainder];
            currentNumber = currentNumber / radix;
        }

        string result = new String(charArray, index + 1, BitsInLong - index - 1);
        if (decimalNumber < 0)
        {
            result = "-" + result;
        }
        return result;
    }



    public void GenerateTerraRings()
    {
        float _startDist = 0;
        if (Quasar)
        {
            _startDist = startDistance * 1.5f;
        }
        else
        {
            _startDist = startDistance;
        }
        for (int i = 0; i < specialStars.Length; i++)
        {
            if (subSeed == specialStars[i].designatedSeed && specialStars[i].binaryStar != null)
            {
                Generate(specialStars[i].binaryStarSize, specialStars[i].binaryStarDistance, defaultMass, Vector2.zero, 1, specialStars[i].binaryStar);

                _startDist = (_startDist + 15);
                for (int x = 0; x < AllPlanets.Count; x++)
                {
                    if (AllPlanets[x].tag == "Binary Star")
                    {
                        MeshRenderer[] m = AllPlanets[x].GetComponentsInChildren<MeshRenderer>();
                        for (int y = 1; y < m.Length; y++)
                        {

                            m[y].material.color = StarColor.Evaluate(specialStars[i].binaryStarTemp);
                        }
                        break;
                    }
                }
                break;
            }
        }
        TerraRings = new Vector2[FilteredSeed[2]];
        for (int i = 0; i < TerraRings.Length; i++)
        {
            if (i == 0)
            {

                int Size = RandGen.Next(minRingSize, FilteredSeed[4]);
                TerraRings[i].x = _startDist + FilteredSeed[8];
                TerraRings[i].y = _startDist + FilteredSeed[8] + Size;
            }
            else
            {
                int Size = RandGen.Next(minRingSize, FilteredSeed[4]);

                TerraRings[i].x = TerraRings[i - 1].y + FilteredSeed[6];
                TerraRings[i].y = TerraRings[i - 1].y + FilteredSeed[6] + Size;

            }
        }
        InstantiateRings(TerraRings, FilteredSeed[0], terraPlanetSize, BasePlanet);
    }
    public void GenerateGasRings()
    {
        GasRings = new Vector2[FilteredSeed[3]];
        for (int i = 0; i < GasRings.Length; i++)
        {
            if (i == 0)
            {
                GasRings[i].x = TerraRings[TerraRings.Length - 1].y + FilteredSeed[6] * 2;
                GasRings[i].y = TerraRings[TerraRings.Length - 1].y + FilteredSeed[6] * 2 + RandGen.Next(minRingSize, FilteredSeed[4]);
            }
            else
            {
                GasRings[i].x = GasRings[i - 1].y + FilteredSeed[6];
                GasRings[i].y = GasRings[i - 1].y + FilteredSeed[6] + RandGen.Next(minRingSize, FilteredSeed[4]);
            }
        }
        InstantiateRings(GasRings, FilteredSeed[1], gasPlanetSize, BasePlanet);
    }
    public void InstantiateRings(Vector2[] _rings, int _quantity, Vector2 _size, GameObject _prefab)
    {
        for (int x = 0; x < _rings.Length; x++)
        {
            for (int y = 0; y < _quantity; y++)
            {
                Generate(UnityEngine.Random.Range(_size.x, _size.y), UnityEngine.Random.Range(_rings[x].x, _rings[x].y), defaultMass, Vector2.zero, 1, _prefab);
            }
        }
    }
    public void InstantiateBinaryStar(float _dist, int _quantity, float _size, GameObject _prefab)
    {
        Generate(_size, _dist, defaultMass, Vector2.zero, 1, _prefab);
    }
    public void generateAsteroidBelt()
    {

        if (FilteredSeed[11] < 2)
        {
            return;
        }
        else if (FilteredSeed[11] > 7)
        {
            AsteroidBelts = new Vector2[TerraRings.Length + GasRings.Length - 1];
        }
        else
        {
            AsteroidBelts = new Vector2[TerraRings.Length + GasRings.Length - 2];

        }
        int numberofTerraBelts = 0;
        for (int i = 0; i < AsteroidBelts.Length; i++)
        {
            int ib = i - numberofTerraBelts;
            if (i < TerraRings.Length - 1)
            {
                numberofTerraBelts++;
                AsteroidBelts[i].x = TerraRings[i].y;
                AsteroidBelts[i].y = TerraRings[i + 1].x;

            }
            else if (ib < GasRings.Length - 1)
            {

                AsteroidBelts[i].x = GasRings[ib].y;
                AsteroidBelts[i].y = GasRings[ib + 1].x;

            }
            else
            {
                if (FilteredSeed[11] > 5)
                {
                    AsteroidBelts[AsteroidBelts.Length - 1].x = TerraRings[TerraRings.Length - 1].y;
                    AsteroidBelts[AsteroidBelts.Length - 1].y = GasRings[0].x;
                }

            }


        }
        for (int b = 0; b < AsteroidBelts.Length; b++)
        {
            float meanDist = (AsteroidBelts[b].x + AsteroidBelts[b].y) / 2;
            float factor = Mathf.Abs(meanDist - AsteroidBelts[b].y);
            for (int d = 0; d < FilteredSeed[7]; d++)
            {
                if (b == AsteroidBelts.Length - 1)
                {
                    for (int x = 0; x < asteroidBeltDensity * 2; x++)
                    {
                        float dist = UnityEngine.Random.Range(meanDist - (factor * asteroidBeltSize), meanDist + (factor * asteroidBeltSize));
                        float rand = UnityEngine.Random.Range(0, factor) * asteriodFalloff;
                        if (rand > Mathf.Abs(meanDist - dist))
                        {
                            Generate(UnityEngine.Random.Range(asteroidSize.x, asteroidSize.y), dist, defaultMass, Vector2.zero, 1, BaseAsteroid);
                        }
                    }
                }
                else
                {
                    for (int x = 0; x < asteroidBeltDensity; x++)
                    {
                        float dist = UnityEngine.Random.Range(meanDist - (factor * asteroidBeltSize), meanDist + (factor * asteroidBeltSize));
                        float rand = UnityEngine.Random.Range(0, factor) * asteriodFalloff;
                        if (rand > Mathf.Abs(meanDist - dist))
                        {
                            Generate(UnityEngine.Random.Range(asteroidSize.x, asteroidSize.y), dist, defaultMass, Vector2.zero, 1, BaseAsteroid);
                        }
                    }
                }
            }
        }
    }





   






    public void Generate(float _Volume, float _Distance, float _Mass, Vector2 _AxisTilt, float _OrbitalSpeed, GameObject _Prefab)
    {
        float angle = RandGen.Next(0, 360) / (Mathf.PI / 180);
        Vector3 SpawnPos = new Vector3(Mathf.Cos(angle), Mathf.Cos(angle) * _AxisTilt.x, Mathf.Sin(angle)).normalized * _Distance;
        GameObject _Instance = Instantiate(_Prefab, SpawnPos, Quaternion.identity, SystemObjectRef.transform);
        AllPlanets.Add(_Instance);
        //==============================================================
        //Set Mass
        //==============================================================
        _Mass = Mathf.Clamp(_Mass, minMass, Mathf.Infinity);
        _Instance.GetComponent<Rigidbody>().mass = _Mass;
        //==============================================================
        //Set Volume
        //==============================================================
        _Volume = Mathf.Clamp(_Volume, minVolume, Mathf.Infinity);
        _Instance.transform.localScale = new Vector3(_Volume, _Volume, _Volume);
        //==============================================================
        //Set Color and Star Reference
        //==============================================================
        PlanetLighting pltemp;
        if (_Instance.TryGetComponent<PlanetLighting>(out pltemp))
        {
            pltemp.Star = star;
            if (_Prefab == BasePlanet)
                pltemp.BaseColor = PlanetColors.Evaluate(UnityEngine.Random.Range(0.0f, 1.0f));
            else if (_Prefab == BaseAsteroid)
                pltemp.BaseColor = AsteroidColor.Evaluate(UnityEngine.Random.Range(0.0f, 1.0f));
        }
        //==============================================================
        //Set Start Velocity
        //==============================================================
        Gravity XGravRef = _Instance.GetComponent<Gravity>();
        //--------------------------------------------------------------
        Vector3 ParalellDir = (_Instance.transform.position - star.transform.position).normalized;
        Vector3 ProductVector = new Vector3(-ParalellDir.z, -ParalellDir.z * _AxisTilt.x, ParalellDir.x).normalized;
        //--------------------------------------------------------------
        XGravRef.StartVelocityType = Gravity.StartVelType.Auto;
        XGravRef.StartVelocity = ProductVector;
    }
}

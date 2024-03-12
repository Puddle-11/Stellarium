using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEngine.Rendering.Universal;
using System.Linq;



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
    [UnityEngine.Range(0, 100)]
    [SerializeField] private int Quasarfrequency; //out of 100
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
    private Vector2[] TerraRings;
    private Vector2[] GasRings;
    private Vector2[] AsteroidBelts;
    private List<GameObject> AllPlanets = new List<GameObject>();
    private bool Explore;
    private Color CurrentStarColor = Color.black;
    private Color CurrentStarTint = Color.black;
    public List<Constelation> constellationList;
    private bool abletoGenerate = true;
    public string[] greekLettes;
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
        GenerateSystem(seed);
    }
    public void RunExplore()
    {
        Explore = true;
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
        //Digit 5: Unused
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
            int[] res = new int[12];
        //if exploring set the main seed to random
        if (Explore)
        {
            res = GenerateSeed();
            seed = string.Join("", res);
        }

        #region Generate Custom Star Variables
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
        #endregion

        //Generate a subseed with the mainseed, if the main seed has already been initialized then subseed will = seed. otherwise subseed will generate a new seed in proper form, from the old one
        SeedDigits = GenerateSeed(seed);
        subSeed = string.Join("", SeedDigits);

       
        SeedText.text = subSeed;//Set the seed text UI element to the subseed
        if (seed == null || seed == string.Empty) seed = subSeed; //if Main seed hasnt been generated set it to the subseed
        SeedDigits = FilterDigits(SeedDigits);

        //==============================================================
        //Generate Quasars
        //==============================================================

        #region Generate Quasars
        int r = RandGen.Next(0, 100);//Roll a 100 sided dice

        //If the role was below the threshold, the current star will be a Quasar
        if (r > Quasarfrequency) Quasar = false; 
        else Quasar = true;

        BlackHole.SetActive(Quasar); //Set the black hole to be active or inactive depending on Quasar State
        Vector3 StarScale = Vector3.zero;
        if (!Quasar)
        {
            StarScale = Vector3.one * (SeedDigits[8] * starSize + minStarSize);
        }
        else
        {
            BlackHole.transform.localScale = (Vector3.one * (SeedDigits[8] * starSize + minBlackHoleSize)) / 2;
            StarScale = new Vector3(SeedDigits[8] * starSize + minBlackHoleSize, 0.2f, SeedDigits[8] * starSize + minBlackHoleSize);
        }
        #endregion

        //Start Generation Process
        GenerateStar(StarScale, StarColor.Evaluate((float)(SeedDigits[9] * 10 + SeedDigits[10]) / 100), SP);
    }
    #region Seed Generation and Filtration
    public int[] GenerateSeed()
    {
        int[] res = new int[12];
        for (int i = 0; i < 12; i++)
        {
            res[i] = UnityEngine.Random.Range(1, 10);
        }
        return res;
    }
    public int[] GenerateSeed(string _seed)
    {
        int[] res = new int[12];
        for (int x = 0; x < res.Length; x++)
        {
            if (x < seed.Length) int.TryParse(seed[x].ToString(), out res[x]);
            else res[x] = RandGen.Next(0, 10);
        }
        return res;
    }
    private int[] FilterDigits(int[] _input)
    {
        //==============================================================
        //Initialize System
        //==============================================================
        //if input is to short to be a seed return an error
        if (_input.Length < 12)
        {
            Debug.LogError("Invalid input passed to FilterDigits");
            return null;
        }
        int[] res = new int[_input.Length]; //set res to the correct length
 
        //==============================================================
        //Manual Filter
        //==============================================================

        //--------------------------------------------------------------
        //Manually filter the digits based on the generation parameters of the system
        res[0] = (int)Mathf.Round(Mathf.Clamp(_input[0] * terraPlanetDensity, 1, 10));
        res[1] = (int)Mathf.Round(Mathf.Clamp(_input[1] * gasPlanetDensity, 1, 10));
        res[2] = (int)Mathf.Round(Mathf.Clamp(_input[2] / terraRingClamp, 1, 10));
        res[3] = (int)Mathf.Round(Mathf.Clamp(_input[3] / gasRingClamp, 1, 10));
        res[4] = _input[4] + minRingSize;
        res[5] = _input[5];
        res[6] = (int)Mathf.Clamp((_input[6] * asteroidBeltSize), asteroidBeltRange.x, asteroidBeltRange.y);
        res[8] = _input[8];
        res[7] = _input[7];
        res[9] = _input[9];
        res[10] = _input[10];
        res[11] = _input[11];
        //--------------------------------------------------------------
        return res;
    }


    #endregion
    public void GenerateStar(Vector3 _starSize, Color _starCol, SpecialStar _CustomStar)
    {
        //==============================================================
        //Initialize Star Generator
        //==============================================================
        string starName;
        Color _starTint = _starCol;
        if (_CustomStar != null)
        {
            if(_CustomStar.useCustomTint) _starTint = _CustomStar.customTint;

            //If the star has a custom color, set the star color to that
            if (_CustomStar.useCustomStarColor)
            {
                float ATemp = _starCol.a;
                _starCol = _CustomStar.customStarColor;
                _starCol.a = ATemp;
            }

            starName = _CustomStar.name; 
            constellationNamefield.text = _CustomStar.constellationName; //Set UI element to custom Constelation name
            descriptionField.text = _CustomStar.description; //Set UI element to custom description 
            constelationImage.sprite = constellationList.Find((x) => x.constelatioName == _CustomStar.constellationName).ConstelationImage; //find the constelation that matches the custom star name, and grab the constelation image

        }
        else
        {
            int starIndex = RandGen.Next(0, constellationList.Count); //get random star index
            descriptionField.text = ""; //Reset descriptor field
            starName = greekLettes[RandGen.Next(0, constellationList[starIndex].starNum)] + " " + constellationList[starIndex].suffixName + "-" + ConvertB24(RandGen.Next(0, 24), 24) + RandGen.Next(0, 10000); //Generate star name
            constellationNamefield.text = constellationList[starIndex].constelatioName;
            constelationImage.sprite = constellationList[starIndex].ConstelationImage;
        }


        starNamefield.text = starName;
        StopCoroutine("LerpStarColor");
        StopCoroutine("LerpStarSize");
        StartCoroutine(LerpStarSize(_starSize, starChangeTime));
        StartCoroutine(LerpStarColor(_starCol, _starTint, starChangeTime));
    }
    public IEnumerator LerpStarSize(Vector3 _End, float _Speed)
    {
        //==============================================================
        //Initialize Star Generator
        //==============================================================
        float stimer = 0;
        Vector3 _Start = star.transform.localScale;
        //Start delay
        yield return new WaitForSeconds(starChangeDelay);

        while (star.transform.localScale != _End)
        {
            star.transform.localScale = Vector3.Lerp(_Start, _End, stimer / _Speed);
            yield return 0;
            stimer += Time.deltaTime;
        }
        yield return new WaitForSeconds(starChangeDelay);
    }
    public IEnumerator LerpStarColor(Color _Color, Color _Tint, float _Speed)
    {
        //==============================================================
        //Initialize Star Generator
        //==============================================================
        Color _startColor = CurrentStarColor;
        Color _startTint = CurrentStarTint;
        float stimer = 0;
        MeshRenderer[] m = star.GetComponentsInChildren<MeshRenderer>();

        yield return new WaitForSeconds(starChangeDelay);
            m[0].material.color = Color.white;
        while (CurrentStarColor != _Color)
        {
            //==============================================================
            //Color Lerp
            CurrentStarTint = Color.Lerp(_startTint, _Tint, stimer / _Speed);
            CurrentStarColor = Color.Lerp(_startColor, _Color, stimer / _Speed);
            for (int i = 1; i < m.Length; i++)
            {
                m[i].material.color = CurrentStarColor;
            }
            //==============================================================

            //--------------------------------------------------------------
            //Set Bloom
            //--------------------------------------------------------------
            CurrentStarTint.a = 255;
            ColorParameter CP = new ColorParameter(CurrentStarTint, true);
            _bloom.tint.SetValue(CP);


            yield return 0;
            stimer += Time.deltaTime;
        }
        yield return new WaitForSeconds(starChangeDelay);

        GenerateTerraRings();
        GenerateGasRings();
        generateAsteroidBelt();

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
        TerraRings = new Vector2[SeedDigits[2]];
        for (int i = 0; i < TerraRings.Length; i++)
        {
            if (i == 0)
            {

                int Size = RandGen.Next(minRingSize, SeedDigits[4]);
                TerraRings[i].x = _startDist + SeedDigits[8];
                TerraRings[i].y = _startDist + SeedDigits[8] + Size;
            }
            else
            {
                int Size = RandGen.Next(minRingSize, SeedDigits[4]);

                TerraRings[i].x = TerraRings[i - 1].y + SeedDigits[6];
                TerraRings[i].y = TerraRings[i - 1].y + SeedDigits[6] + Size;

            }
        }
        InstantiateRings(TerraRings, SeedDigits[0], terraPlanetSize, BasePlanet);
    }
    public void GenerateGasRings()
    {
        GasRings = new Vector2[SeedDigits[3]];
        for (int i = 0; i < GasRings.Length; i++)
        {
            if (i == 0)
            {
                GasRings[i].x = TerraRings[TerraRings.Length - 1].y + SeedDigits[6] * 2;
                GasRings[i].y = TerraRings[TerraRings.Length - 1].y + SeedDigits[6] * 2 + RandGen.Next(minRingSize, SeedDigits[4]);
            }
            else
            {
                GasRings[i].x = GasRings[i - 1].y + SeedDigits[6];
                GasRings[i].y = GasRings[i - 1].y + SeedDigits[6] + RandGen.Next(minRingSize, SeedDigits[4]);
            }
        }
        InstantiateRings(GasRings, SeedDigits[1], gasPlanetSize, BasePlanet);
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

        if (SeedDigits[11] < 2)
        {
            return;
        }
        else if (SeedDigits[11] > 7)
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
                if (SeedDigits[11] > 5)
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
            for (int d = 0; d < SeedDigits[7]; d++)
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

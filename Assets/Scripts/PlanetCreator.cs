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

public class PlanetCreator : MonoBehaviour
{
    [Space]
    [Space]
    [Header("General")]
    [SerializeField] public string seed;
    public string subSeed;
    [SerializeField] private TextMeshProUGUI SeedText;
    [SerializeField] private GameObject SystemObjectRef;
    [SerializeField] private Volume PostProfile;
    [SerializeField] private GameObject BasePlanet;
    [SerializeField] private GameObject BaseAsteroid;
    [SerializeField] private GameObject star;
    [SerializeField] private TextMeshProUGUI starNamefield;
    [SerializeField] private UnityEngine.UI.Image constelationImage;

    [Space]
    [Space]
    [Header("Generation Parameters")]
    [SerializeField] private int minRingSize;
    [SerializeField] private float DefaultMass;
    [SerializeField] private float DefaultVolume;
    [Space]
    [Space]
    [Header("Star Parameters")]
    [SerializeField] private float starSize;
    [SerializeField] private float minStarSize;
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
    public string[] PlanetNames;

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
        if (!abletoGenerate)
        {
            return;
        }
        abletoGenerate = false;


        RandGen = new System.Random(seed.GetHashCode());

        for (int i = 0; i < AllPlanets.Count; i++)
        {
            Destroy(AllPlanets[i]);
        }
        AllPlanets = new List<GameObject>();
        //=============================================
        //Get seed
        //=============================================
        subSeed = "";
        if (Explore)
        {

            SeedDigits = GenerateSeed(12);
           
        }
        else
        {
            SeedDigits = new int[12];

            for (int x = 0; x < SeedDigits.Length; x++)
            {
                int temp;
                if (x < _seed.Length && int.TryParse(_seed[x].ToString(), out temp))
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
        string tempseed = "";
        for (int i = 0; i < 5; i++)
        {
            tempseed = tempseed + SeedDigits[i].ToString();
        }
        int _seedint = int.Parse(tempseed);
       if(seed == null || seed == string.Empty)
        {
            seed = subSeed;
        }
        //=============================================
        //Digit 0: number of Terestrial planets (per ring)
        //Digit 1: number of gasious planets (per ring)
        //Digit 2: number of Rings Terestrial
        //Digit 3: number of Rings Gassious
        //Digit 4: Size of rings Range
        //Digit 6: Distance of Between rings (this effects how big the asteroid belts are)
        //Digit 7: density of asteroid belts
        //Digit 8: star size;
        //Digit 9-10: star Tempeture;
        //Digit 11: terra-gas belt

        FilteredSeed = new int[SeedDigits.Length];
        FilteredSeed[0] = (int)Mathf.Round(Mathf.Clamp(SeedDigits[0] * terraPlanetDensity,1, 10));
        FilteredSeed[1] = (int)Mathf.Round(Mathf.Clamp(SeedDigits[1] * gasPlanetDensity, 1, 10));
        FilteredSeed[2] = (int)Mathf.Round( Mathf.Clamp(SeedDigits[2]/ terraRingClamp, 1, 10));
        FilteredSeed[3] = (int)Mathf.Round(Mathf.Clamp(SeedDigits[3] / gasRingClamp, 1, 10));
        FilteredSeed[4] = SeedDigits[4] + minRingSize;
        FilteredSeed[5] = SeedDigits[5];
        FilteredSeed[6] = (int)Mathf.Clamp((SeedDigits[6] * asteroidBeltSize), asteroidBeltRange.x, asteroidBeltRange.y);
        FilteredSeed[7] = SeedDigits[7];
        FilteredSeed[8] = SeedDigits[8];
        FilteredSeed[9] = SeedDigits[9];
        FilteredSeed[10] = SeedDigits[10];
        FilteredSeed[11] = SeedDigits[11];


      
        generateStar(FilteredSeed[8], StarColor.Evaluate(   (float)(FilteredSeed[9] * 10 + FilteredSeed[10])/100));



    }
    public void generateStar(float _starSize, Color _starCol)
    {
     
        int starInex = RandGen.Next(0, ConstelationList.Length);
        StarName = greekLettes[RandGen.Next(0, ConstelationList[starInex].starNum)] + " " + ConstelationList[starInex].suffixName + "-" + ConvertB24(RandGen.Next(0, 24), 24) + RandGen.Next(0, 10000);

        starNamefield.text = StarName.ToUpper();
        if (ConstelationList[starInex].ConstelationImage != null)
        {
            constelationImage.sprite = ConstelationList[starInex].ConstelationImage;
        }
        else
        {
            constelationImage.sprite = null;

        }
        StopCoroutine("LerpStarColor");
        StopCoroutine("LerpStarSize");
        StartCoroutine(LerpStarSize(star.transform.localScale.x, _starSize * starSize + minStarSize, starChangeTime));
        StartCoroutine(LerpStarColor(CurrentStarColor, _starCol, starChangeTime));
    }
    public IEnumerator LerpStarSize(float _Start, float _End, float _Speed)
    {
        yield return new WaitForSeconds(starChangeDelay);
        float stimer = 0;
        float current = _Start;
        while (current != _End)
        {

            current = Mathf.Lerp(_Start, _End, stimer / _Speed);
            star.transform.localScale = new Vector3(current, current, current);
            yield return 0;
            stimer += Time.deltaTime;
        }
        yield return new WaitForSeconds(starChangeDelay);

    }
    public IEnumerator LerpStarColor(Color _Start, Color _End, float _Speed)
    {
        yield return new WaitForSeconds(starChangeDelay);

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
           
            if(AllPlanets[i].gameObject.tag == "Planet")
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
        TerraRings = new Vector2[FilteredSeed[2]];
        for (int i = 0; i < TerraRings.Length; i++)
        {
            if (i == 0)
            {
                int Size = RandGen.Next(minRingSize, FilteredSeed[4]);
                TerraRings[i].x = startDistance + FilteredSeed[8];
                TerraRings[i].y = startDistance + FilteredSeed[8] + Size;
            }
            else
            {
                int Size = RandGen.Next(minRingSize, FilteredSeed[4]);

                TerraRings[i].x = TerraRings[i-1].y + FilteredSeed[6];
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
                GasRings[i].x = TerraRings[TerraRings.Length - 1].y + FilteredSeed[6]*2;
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
    public void InstantiateRings(Vector2[] _rings,int _quantity,Vector2 _size, GameObject _prefab)
    {
        for (int x = 0; x < _rings.Length; x++)
        {
            for (int y = 0; y < _quantity; y++)
            {
                Generate(UnityEngine.Random.Range(_size.x, _size.y), UnityEngine.Random.Range(_rings[x].x, _rings[x].y), DefaultMass, Vector2.zero, 1, _prefab);
            }
        }
    }
    public void generateAsteroidBelt()
    {

        if(FilteredSeed[11] < 2)
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
                            Generate(UnityEngine.Random.Range(asteroidSize.x, asteroidSize.y), dist, DefaultMass, Vector2.zero, 1, BaseAsteroid);
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
                            Generate(UnityEngine.Random.Range(asteroidSize.x, asteroidSize.y), dist, DefaultMass, Vector2.zero, 1, BaseAsteroid);
                        }
                    }
                }
            }
        }
    }

  


    
    public int[] GenerateSeed(int Size)
    {
        seed = "";
        int[] res = new int[Size];
        for (int i = 0; i < Size; i++)
        {
            res[i] = UnityEngine.Random.Range(1, 10);
            seed += res[i].ToString();
        }
        return res;
    }
   





    public void Generate(float _Volume, float _Distance, float _Mass, Vector2 _AxisTilt, float _OrbitalSpeed, GameObject _Prefab)
    {
        float angle = RandGen.Next(0, 360) / (Mathf.PI / 180);
        Vector3 SpawnPos = new Vector3(Mathf.Cos(angle), Mathf.Cos(angle) * _AxisTilt.x, Mathf.Sin(angle)).normalized;
        SpawnPos = SpawnPos * _Distance;

   
                GameObject x = Instantiate(_Prefab, SpawnPos, Quaternion.identity, SystemObjectRef.transform);
        AllPlanets.Add(x);
                if (_Mass == 0)
                {
                    x.GetComponent<Rigidbody>().mass = DefaultMass;
                }
                else
                {
                    x.GetComponent<Rigidbody>().mass = _Mass;
                }


                if (_Volume == 0)
                {
                    float Volume = DefaultVolume;
                    x.transform.localScale = new Vector3(Volume, Volume, Volume);

                }
                else
                {
                    x.transform.localScale = new Vector3(_Volume, _Volume, _Volume);

                }
                Gravity XGravRef = x.GetComponent<Gravity>();

                Vector3 Axis = Vector3.zero;
                    Axis = Vector3.Normalize(new Vector3(0, _AxisTilt.x, 1));

                



                XGravRef.StartVelocityType = Gravity.StartVelType.Auto;
              
        if(_Prefab == BasePlanet)
            x.GetComponent<PlanetLighting>().BaseColor = PlanetColors.Evaluate(UnityEngine.Random.Range(0.0f, 1.0f));
        else
            x.GetComponent<PlanetLighting>().BaseColor = AsteroidColor.Evaluate(UnityEngine.Random.Range(0.0f, 1.0f));

        x.GetComponent<PlanetLighting>().Star = star;



        Vector3 ProductVector = (x.transform.position - star.transform.position).normalized;
        ProductVector = new Vector3(-ProductVector.z, -ProductVector.z * _AxisTilt.x, ProductVector.x).normalized;



        XGravRef.StartVelocity = ProductVector;
    }
  

}

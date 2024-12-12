using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeatShip : MonoBehaviour {
    private Dictionary<int, Dictionary<int, float>> NodeSimilarities = new Dictionary<int, Dictionary<int, float>>();
    private Dictionary<int, float> NodeAdjustedFitnessMultiplies = new Dictionary<int, float>();
    [SerializeField]
    private float LifeTime = 2f;
    private RocketNeat[] AllRocketNeats;
    [SerializeField]
    private float MutationRate = 0.1f;
    [SerializeField]
    private int PopulationCount = 250;
    [SerializeField]
    private GameObject Species;
    [SerializeField]
    private Transform StartPoint;
    [SerializeField]
    private float TurnSpeed;
    [SerializeField]
    [Range(0, 100)]
    private float Speed = 1;
    [SerializeField]
    private float ObstacleDistance = 5f;
    private float TotalFitness = 0;
    private float TotalAdjustedFitness = 0;
    private int Generation = 1;
    private float AverageFitness = 0;
    private float MaxFitness;
    private RocketNeat BestController;
    private int OverMaxDistance;
    private int CurrentMaxdistance;
    [SerializeField]
    private AsteroidBelt AB;
    private NeatViualizer NW;
    public Transform Furthest;
    private Transform CurrentMaxRocket;

    [Header("Neat Values")]
    [SerializeField]
    private float c1 = 1;
    [SerializeField]
    private float c2 = 1;
    [SerializeField]
    private float c3 = 1;
    [SerializeField]
    private float DeltaThreshold = 3;
    // Use this for initialization
    void Start()
    {
        AllRocketNeats = new RocketNeat[PopulationCount];
        NW = GetComponent<NeatViualizer>();

        for (int i = 0; i < AllRocketNeats.Length; i++)
        {
            GameObject CurrentSpecies = Instantiate(Species, StartPoint.position, Quaternion.Euler(new Vector3(0, 0, 270)));
            RocketNeat c = CurrentSpecies.GetComponent<RocketNeat>();
            c.Speed = Speed;
            c.TurnSpeed = TurnSpeed;
            c.LifeTime = LifeTime;
            c.MotherShip = this;
            c.MutationRate = MutationRate;
            c.ObstacleDistance = ObstacleDistance;
            c.CreateNewBrain(4, 2);
            c.color = Random.ColorHSV();
            AllRocketNeats[i] = c;
        }
    }

    public string result = "";

    void CreateNewPopulation()
    {
        RocketNeat[] NewControllers = new RocketNeat[PopulationCount];

        GetSimiliraties();
        SumFitness();

        RocketNeat prevparent = new RocketNeat();
        for (int i = 0; i < NewControllers.Length; i++)
        {

            RocketNeat Parent = GetParent();
            RocketNeat Parent2 = GetSimilarRocket(Parent);
            GameObject CurrentSpecies = Instantiate(Species, StartPoint.position, Quaternion.Euler(new Vector3(0, 0, 270)));
            RocketNeat c = CurrentSpecies.GetComponent<RocketNeat>();

            c.CreateNewBrain(Parent.Fitness > Parent2.Fitness ? Parent.Brain : Parent2.Brain, Parent.Fitness > Parent2.Fitness ? Parent2.Brain : Parent.Brain);
            
            c.Speed = Speed;
            c.TurnSpeed = TurnSpeed;
            c.MotherShip = this;
            c.MutationRate = MutationRate;
            c.ObstacleDistance = ObstacleDistance;
            c.LifeTime = LifeTime;
            c.color = Parent.color;
            NewControllers[i] = c;
        }

        for (int i = 0; i < AllRocketNeats.Length; i++)
        {
            Destroy(AllRocketNeats[i].gameObject);
        }

        NewControllers.CopyTo(AllRocketNeats, 0);
        Generation++;
        AB.CreateNewAsteroids();
        CurrentMaxdistance = 0;
        result += Generation.ToString() + " | " + AverageFitness + " | " + OverMaxDistance + "\n";
        Debug.Log(result);
    }

    void SumFitness()
    {
        TotalFitness = 0;
        TotalAdjustedFitness = 0;

        for (int i = 0; i < AllRocketNeats.Length; i++)
        {

            RocketNeat c = AllRocketNeats[i];
            TotalAdjustedFitness += c.Fitness / NodeAdjustedFitnessMultiplies[i];
            TotalFitness += c.Fitness;

            if (c.Fitness > MaxFitness)
            {
                MaxFitness = c.Fitness;
                BestController = c;

            }
        }
        
        AverageFitness = TotalFitness / PopulationCount;

        NW.Display(BestController.Brain, null);

    }

    float GetAdjustedFitness(int index)
    {
        Dictionary<int, float> tempDict = NodeSimilarities[index];
        float output = 0;
        foreach (float f in tempDict.Values)
        {
            if (f > DeltaThreshold)
            {
                output += 1;
            }
        }
        return Mathf.Max(1, output);
    }

    RocketNeat GetSimilarRocket(RocketNeat B1)
    {
        int index = -1;
        for (int i = 0; i < AllRocketNeats.Length; i++)
        {
            if (AllRocketNeats[i] == B1)
            {
                index = i;
                break;
            }
        }
        Dictionary<int, float> BirdSimiliraties = NodeSimilarities[index];
        float total = 0;
        foreach (float f in BirdSimiliraties.Values)
        {
            total += f;
        }
        float rand = Random.Range(0, total);
        float current = 0;
        foreach (int i in BirdSimiliraties.Keys)
        {
            current += BirdSimiliraties[i];
            if (current > rand)
            {
                return AllRocketNeats[i];
            }
        }
        return null;
    }


    void GetSimiliraties()
    {
        NodeSimilarities = new Dictionary<int, Dictionary<int, float>>();
        NodeAdjustedFitnessMultiplies = new Dictionary<int, float>();
        for (int i = 0; i < AllRocketNeats.Length; i++)
        {
            RocketNeat B1 = AllRocketNeats[i];
            NodeSimilarities[i] = new Dictionary<int, float>();
            for (int j = 0; j < AllRocketNeats.Length; j++)
            {
                if (j != i)
                {
                    RocketNeat B2 = AllRocketNeats[j];
                    float Difference = B1.Brain.CalculateDifference(B2.Brain, c1, c2, c3);
                    NodeSimilarities[i][j] = Difference;


                }
            }
        }

        for (int i = 0; i < AllRocketNeats.Length; i++)
        {
            NodeAdjustedFitnessMultiplies[i] = GetAdjustedFitness(i);
        }
    }

    RocketNeat GetParent()
    {
        float CurrentFitness = 0;
        float RandFitness = Random.Range(0, TotalAdjustedFitness);
        RocketNeat parent = AllRocketNeats[0];
        for (int i = 0; i < AllRocketNeats.Length; i++)
        {
            RocketNeat c = AllRocketNeats[i];
            CurrentFitness += c.Fitness / NodeAdjustedFitnessMultiplies[i];
            if (CurrentFitness > RandFitness)
            {

                parent = c;
                break;
            }
        }

        return parent;
    }

    public void CheckAllDead()
    {
        foreach (RocketNeat c in AllRocketNeats)
        {
            if (!c.Dead)
                return;
        }

        CreateNewPopulation();
    }

    private void OnGUI()
    {
        GUI.skin.label.fontSize = 75;
        GUI.color = Color.white;
        GUI.Label(new Rect(0, 0, 500, 100), "Gen: " + Generation.ToString());
        GUI.Label(new Rect(0, 100, 5000, 100), "Best: " + OverMaxDistance.ToString());
        GUI.Label(new Rect(0, 200, 5000, 100), "Current Best: " + CurrentMaxdistance.ToString());
    }


    public Transform FurthestRocket()
    {
        float MaxX = 0;
        Transform MaxRocketNeat = AllRocketNeats[0].transform;
        foreach (RocketNeat r in AllRocketNeats)
        {
            if (!r.Dead)
            {
                if (r.transform.position.x > MaxX)
                {
                    MaxX = r.transform.position.x;
                    MaxRocketNeat = r.transform;
                }

                if (CurrentMaxRocket == null || r.transform.position.x > CurrentMaxRocket.position.x + 1)
                {
                    CurrentMaxRocket = r.transform;
                    NW.Display(r.Brain, null);
                }

                if (r.transform.position.x > OverMaxDistance)
                {
                    OverMaxDistance = (int)r.transform.position.x;
                }
            }
        }
        if (CurrentMaxRocket != null)
            CurrentMaxdistance = (int)CurrentMaxRocket.transform.position.x;

        return MaxRocketNeat;
    }
    
}

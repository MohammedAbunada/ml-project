using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseNeatController : MonoBehaviour
{
    protected Dictionary<int, Dictionary<int, float>> NodeSimilarities = new Dictionary<int, Dictionary<int, float>>();
    protected Dictionary<int, float> NodeAdjustedFitnessMultiplies = new Dictionary<int, float>();
    protected NeatViualizer NW;
    protected BaseNeat[] AllBaseNeats;
    [SerializeField]
    protected float MutationRate = 0.1f;
    [SerializeField]
    protected int PopulationCount = 100;
    [SerializeField]
    protected GameObject Species;
    [SerializeField]
    protected Transform StartPoint;
    protected float TotalFitness = 0;
    protected float TotalAdjustedFitness = 0;
    protected int Generation = 1;
    protected float AverageFitness = 0;
    protected float MaxFitness;
    protected BaseNeat BestController;
    protected float OverMaxDistance;
    protected Transform CurrentBestTransform;
    protected int CurrentMaxedistance;

    [Header("Neat Values")]
    [SerializeField]
    protected float c1 = 1;
    [SerializeField]
    protected float c2 = 1;
    [SerializeField]
    protected float c3 = 1;
    [SerializeField]
    protected float DeltaThreshold = 3;

    public virtual void SumFitness()
    {
        TotalFitness = 0;
        TotalAdjustedFitness = 0;
        for (int i = 0; i < AllBaseNeats.Length; i++)
        {
            BaseNeat c = AllBaseNeats[i];

            TotalFitness += c.Fitness;
            TotalAdjustedFitness += c.Fitness / NodeAdjustedFitnessMultiplies[i];
            if (c.Fitness > MaxFitness)
            {
                MaxFitness = c.Fitness;
                BestController = c;

            }

        }
        NW.Display(BestController.Brain, BestController);
    }

    public  float GetAdjustedFitness(int index)
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

    public BaseNeat GetParent()
    {
        float CurrentFitness = 0;
        float RandFitness = Random.Range(0, TotalAdjustedFitness);
        BaseNeat parent = AllBaseNeats[0];
        for (int i = 0; i < AllBaseNeats.Length; i++)
        {
            BaseNeat c = AllBaseNeats[i];
            CurrentFitness += c.Fitness / NodeAdjustedFitnessMultiplies[i];
            if (CurrentFitness > RandFitness)
            {

                parent = c;
                break;
            }
        }


        return parent;
    }
    public virtual void CreateNewPopulation()
    {

    }
    public BaseNeat GetSimilarBird(BaseNeat B1)
    {
        int index = -1;
        for (int i = 0; i < AllBaseNeats.Length; i++)
        {
            if (AllBaseNeats[i] == B1)
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
                return AllBaseNeats[i];
            }
        }
        return null;
    }

    public void GetSimiliraties()
    {
        NodeSimilarities = new Dictionary<int, Dictionary<int, float>>();
        NodeAdjustedFitnessMultiplies = new Dictionary<int, float>();
        for (int i = 0; i < AllBaseNeats.Length; i++)
        {
            BaseNeat B1 = AllBaseNeats[i];
            NodeSimilarities[i] = new Dictionary<int, float>();
            for (int j = 0; j < AllBaseNeats.Length; j++)
            {
                if (j != i)
                {
                    BaseNeat B2 = AllBaseNeats[j];
                    float Difference = B1.Brain.CalculateDifference(B2.Brain, c1, c2, c3);
                    NodeSimilarities[i][j] = Difference;


                }
            }
        }

        for (int i = 0; i < AllBaseNeats.Length; i++)
        {
            NodeAdjustedFitnessMultiplies[i] = GetAdjustedFitness(i);
        }
    }



    public void CheckAllDead()
    {
        foreach (BaseNeat c in AllBaseNeats)
        {
            if (!c.Dead)
                return;
        }

        CreateNewPopulation();
    }

    

    public virtual Transform FurthestSpecies()
    {
        return null;
    }


}

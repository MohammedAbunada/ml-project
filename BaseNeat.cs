using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseNeat : MonoBehaviour
{
    [HideInInspector]
    public Genome Brain;
    [HideInInspector]
    public float MutationRate;
    [HideInInspector]
    public bool Dead;
    [HideInInspector]
    public float[] Features;
    [HideInInspector]
    public float Fitness;
    [HideInInspector]
    public BaseNeatController myNest;
    [HideInInspector]
    public Color color;


    public void CreateNewBrain(int InputCount, int OutputCount)
    {
        Brain = new Genome(InputCount, OutputCount);
        Brain.Mutate();
    }

    public void CreateNewBrain(Genome P1, Genome P2)
    {
        Brain = new Genome(P1, P2);
        Brain.Mutate();
    }

    public void CreateNewBrain(Genome P1)
    {
        Brain = new Genome(P1);
    }

    public float Sigmoid(float x)
    {
        return 1 / (1 + Mathf.Exp(-2f * x));
    }

    public float NegativeSigmoid(float x)
    {
        return 2 * Sigmoid(x) - 1;
    }

    public float DotProduct(float[] A, float[] B)
    {
        float sum = 0;
        for (int i = 0; i < A.Length; i++)
        {
            sum += A[i] * B[i];
        }
        return sum;
    }

    public virtual void CalculateFitness()
    {
        Fitness = transform.position.x;
    }


    public virtual void Die()
    {
        Dead = true;
        CalculateFitness();
        myNest.CheckAllDead();
    }

}

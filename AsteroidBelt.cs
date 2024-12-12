using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidBelt : MonoBehaviour {
    [SerializeField]
    private float MinimumDistance = 2f;

    [SerializeField]
    private float MaximumDistance = 5f;

    [SerializeField]
    private float MinSize = 1f;

    [SerializeField]
    private float MaxSize = 2f;

    [SerializeField]
    private GameObject AsteroidGameobject;

    [SerializeField]
    private int AsteroidCount = 1000;
    private GameObject[] AllAsteroids;

    [SerializeField]
    private float YOffset;
    private float LastX;
    private float Min;
    private float Max;
	// Use this for initialization
	void Start () {
        AllAsteroids = new GameObject[1000];
        LastX = 15;
        Min = MinimumDistance;
        Max = MaximumDistance;
        for (int i = 0; i < AllAsteroids.Length; i++)
        {
            GameObject g = Instantiate(AsteroidGameobject, new Vector3(LastX, Random.Range(-YOffset, YOffset), 0), Quaternion.identity);
            LastX += Random.Range(Min, Max);
            g.transform.localScale *= Random.Range(MinSize, MaxSize);
            AllAsteroids[i] = g;
            //if (Max > 2f)
            //    Max -= 0.003f;
            //if (Min > 0f)
            //    Min -= 0.002f;
        }
    }
	
    public void CreateNewAsteroids()
    {
        LastX = 15;
        Min = MinimumDistance;
        Max = MaximumDistance;
        for (int i = 0; i < AllAsteroids.Length; i++)
        {
            if (AllAsteroids[i]!=null)
                Destroy(AllAsteroids[i]);
            
        }
        AllAsteroids = new GameObject[1000];

        for (int i = 0; i < AllAsteroids.Length; i++)
        {
            GameObject g = Instantiate(AsteroidGameobject, new Vector3(LastX, Random.Range(-YOffset, YOffset), 0), Quaternion.identity);
            LastX += Random.Range(Min, Max);
            g.transform.localScale *= Random.Range(MinSize, MaxSize);
            AllAsteroids[i] = g;
            //if (Max > 2f)
            //    Max -= 0.003f;
            //if (Min > 0f)
            //    Min -= 0.002f;

        }
    }
}

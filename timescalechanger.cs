using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timescalechanger : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 1; i < 10; i++)
        {
            if (Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + i.ToString())))
            {
                Time.timeScale = i;
            }
        }
    }
}
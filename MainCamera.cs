using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {
    [SerializeField]
    private float Smoothness = 20f;

    [SerializeField]
    private NeatShip RM;

    private Transform FurthestRocket;
	// Update is called once per frame
	void FixedUpdate () {
        FurthestRocket = RM.FurthestRocket();

        transform.position = new Vector3(Mathf.Lerp(transform.position.x, FurthestRocket.position.x, Time.deltaTime * Smoothness), transform.position.y, transform.position.z);
	}
}

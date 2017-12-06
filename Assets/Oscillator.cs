using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour {
    [SerializeField] Vector3 movementVector = new Vector3 (10f,10f,10f);
    [SerializeField] float period = 2f;
    float movementFactor;
    Vector3 startingPos;
	// Use this for initialization
	void Start ()
    {
        startingPos = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        /*floats can be never the same and Mathf.Epsilon is the smallest value that a float can be.
         so instead of period == 0, we use period <= Math.Epsilon*/
        if (period <= Mathf.Epsilon) { return; }
        float cycles = Time.time / period;
        const float tau = Mathf.PI;
        float rawSinWave = Mathf.Sin(cycles * tau);
        movementFactor = rawSinWave / 2f + .5f;
        transform.position = startingPos + movementVector * movementFactor;
    }
}

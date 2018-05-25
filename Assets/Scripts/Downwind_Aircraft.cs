using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {

    public GameObject aircraft;
    public GameObject downwind;
    public Rigidbody rb;

    public Aircraft ac;

    public float smoothTime = .3f;
    public Vector3 velocity = Vector3.zero;

    // Use this for initialization
    void Start () {
        downwind = new GameObject();
        aircraft = new GameObject();
        downwind = GameObject.Find("downwind");
        aircraft = GameObject.Find("C130");
        rb = aircraft.GetComponent<Rigidbody>();
        ac = new Aircraft();

	}

    public void Initialize_Aircraft(Aircraft ac)
    {

    }
	
	// Update is called once per frame
	void Update () {
		if(aircraft.transform.position == downwind.transform.position)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, 10f);
            Vector3 targetPosition = rb.transform.TransformPoint(new Vector3(ac.xForce, ac.yForce, ac.zForce));
            rb.transform.position = Vector3.SmoothDamp(rb.transform.position, targetPosition, ref velocity, smoothTime);
        }
	}
}

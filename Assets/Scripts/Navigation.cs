using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation : MonoBehaviour {

    public Transform destination_point;
    //public GameObject nav;

    public GameObject aircraft;
    public GameObject downwind;
    public GameObject base_leg;
    public Rigidbody rb;

    public Aircraft ac;

    public float smoothTime = .3f;
    public Vector3 velocity = Vector3.zero;
    public int entered_downwind = 0;

    public NavMeshAgent nav;

    // Use this for initialization
    void Start () {
        //nav = new GameObject();
        //nav = GameObject.Find("downwind");
        nav = GetComponent<NavMeshAgent>();
        
        downwind = new GameObject();
        aircraft = new GameObject();
        downwind = GameObject.Find("downwind");
        base_leg = GameObject.Find("base_leg");

        aircraft = GameObject.Find("C130");
        rb = aircraft.GetComponent<Rigidbody>();
        ac = new Aircraft();
        Initialize_Aircraft(ac);
    }


    public void Initialize_Aircraft(Aircraft ac)
    {
        ac.xForce = 8f;
        ac.yForce = 0;
        ac.zForce = 0f;

        ac.rx = -90;
        ac.ry = 110;
        ac.rz = 0f;

    }

    public void entering_downwind()
    {
        //Destroy(transform.GetComponent<NavMeshAgent>());
        //Destroy(transform.GetComponent<MeshRenderer>());

        //ac.px = ac.transform.position.x;
        //ac.py = ac.transform.position.y;
        //ac.pz = ac.transform.position.z;
        ac.ry += 70f;


        //rb.AddForce(ac.xForce, ac.yForce, ac.zForce);
        entered_downwind = 1;
    }
	
	// Update is called once per frame
	void Update () {
        if(entered_downwind == 0)
        {
            print("Nav Start");
            transform.localEulerAngles = new Vector3(ac.rx, ac.ry, ac.rz);
            nav.SetDestination(downwind.transform.position);
            //transform.GetComponent<NavMeshAgent>().destination = downwind.transform.position;

        }
     
        else if(aircraft.transform.position.z <= 35 && entered_downwind == 0)
        {
            print("entering downind");
            entering_downwind();

            //Vector3 targetPosition = rb.transform.TransformPoint(new Vector3(ac.xForce, ac.yForce, ac.zForce));
            //rb.transform.position = Vector3.SmoothDamp(rb.transform.position, targetPosition, ref velocity, smoothTime);
            //rb.velocity = Vector3.ClampMagnitude(rb.velocity, 10f);
        }

        if(entered_downwind == 1)
        {
            this.transform.localEulerAngles = new Vector3(ac.rx, ac.ry, ac.rz);
            transform.GetComponent<NavMeshAgent>().destination = base_leg.transform.position;
            //Vector3 targetPosition = rb.transform.TransformPoint(new Vector3(ac.xForce, ac.yForce, ac.zForce));
            //rb.transform.position = Vector3.SmoothDamp(rb.transform.position, targetPosition, ref velocity, smoothTime);
            //rb.velocity = Vector3.ClampMagnitude(rb.velocity, 10f);
        }
    }
}

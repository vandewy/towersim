using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation : MonoBehaviour {

    //public Transform destination_point;
    //public GameObject nav;

    public GameObject aircraft;
    public GameObject downwind;
    public GameObject perch;
    public GameObject short_final;
    public Rigidbody rb;

    public Aircraft ac;

    public float smoothTime = .3f;
    public Vector3 velocity = Vector3.zero;
    public int entered_downwind = 0;

    public NavMeshAgent nav;

    // Use this for initialization
    void Start () {
        //Get aircrafts mesh, enables navigation
        nav = GetComponent<NavMeshAgent>();
        
        //Location in the rectangular pattern
        downwind = GameObject.Find("Downwind");
        perch = GameObject.Find("Perch");
        short_final = GameObject.Find("Short_Final");


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

        ac.initial_spawn = true;
        nav.SetDestination(downwind.transform.position);

    }

    public void entering_downwind(Aircraft ac)
    {
        float z_rotation = ac.rz;

        while(ac.rz != -1f)
        {
            ac.rz += .00001f;
        }
        //Destroy(transform.GetComponent<NavMeshAgent>());
        //Destroy(transform.GetComponent<MeshRenderer>());

        //ac.px = ac.transform.position.x;
        //ac.py = ac.transform.position.y;
        //ac.pz = ac.transform.position.z;
        ac.ry += 70f;


        //rb.AddForce(ac.xForce, ac.yForce, ac.zForce);
        entered_downwind = 1;
    }

    public void enter_downwind_controller()
    {
        System.Threading.Thread mThread = new System.Threading.Thread(() => entering_downwind(ac));
        mThread.Start();
    }

    // Update is called once per frame
    void Update () {
        if(entered_downwind == 0)
        {
            print("Nav Start");
            transform.localEulerAngles = new Vector3(ac.rx, ac.ry, ac.rz);
            //nav.SetDestination(downwind.transform.position);
        }
        else if(entered_downwind == 0)
        {
            print("entering downind");
            //entering_downwind();

            //Vector3 targetPosition = rb.transform.TransformPoint(new Vector3(ac.xForce, ac.yForce, ac.zForce));
            //rb.transform.position = Vector3.SmoothDamp(rb.transform.position, targetPosition, ref velocity, smoothTime);
            //rb.velocity = Vector3.ClampMagnitude(rb.velocity, 10f);
        }

        if(entered_downwind == 1)
        {
            this.transform.localEulerAngles = new Vector3(ac.rx, ac.ry, ac.rz);
            transform.GetComponent<NavMeshAgent>().destination = perch.transform.position;
            //Vector3 targetPosition = rb.transform.TransformPoint(new Vector3(ac.xForce, ac.yForce, ac.zForce));
            //rb.transform.position = Vector3.SmoothDamp(rb.transform.position, targetPosition, ref velocity, smoothTime);
            //rb.velocity = Vector3.ClampMagnitude(rb.velocity, 10f);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        print(other.gameObject.name);
        if(other.gameObject.name == "Downwind")
        {
            enter_downwind_controller();
            nav.SetDestination(perch.transform.position);

        }else if(other.gameObject.name == "Perch")
        {
            nav.SetDestination(short_final.transform.position);
        }
    }
}

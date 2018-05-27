using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation : MonoBehaviour {

    public GameObject aircraft;
    public GameObject downwind;
    public GameObject perch;
    public GameObject short_final;

    public Rigidbody rb;

    
    public Animator anime;
    public Animation animate;
    public AnimationClip anim_clip;
 
    public Aircraft ac;

    public float smoothTime = .3f;
    
    public int entered_downwind = 0;

    public NavMeshAgent nav;

    public GameObject goal;
    public float movement_speed = 10f;
    public Transform target;

    // Use this for initialization
    void Start () {
        //Get aircrafts mesh, enables navigation
        nav = GetComponent<NavMeshAgent>();
        
        //Locations in the traffic pattern
        downwind = GameObject.Find("Downwind");
        perch = GameObject.Find("Perch");
        short_final = GameObject.Find("Short_Final");

        anime = gameObject.GetComponent<Animator>();
        animate = gameObject.GetComponent<Animation>();

        aircraft = GameObject.Find("C130");
        rb = aircraft.GetComponent<Rigidbody>();
        ac = new Aircraft();
        Initialize_Aircraft(ac);

        anime.enabled = true;
        anime.Play("Fly_wheels_hidden");

        load_animations();
        target = GameObject.Find("Location").transform;
    }

    private void load_animations()
    {
        anim_clip = Resources.Load("enter_downwind") as AnimationClip;
        animate.AddClip(anim_clip, "enter_downwind");
    }


    public void Initialize_Aircraft(Aircraft ac)
    {
        //ac.xForce = 8f;
        //ac.yForce = 0;
        //ac.zForce = 0f;

        ac.rx = -90;
        ac.ry = 110;
        ac.rz = 0f;

        ac.initial_spawn = true;
        nav.SetDestination(downwind.transform.position);
        
    }

    public void entering_downwind()
    {
        animate.PlayQueued("enter_downwind");
        
        entered_downwind = 1;

        //Downwind
        ac.rx = -90;
        ac.ry = 180;
        ac.rz = 0f;
    }

    // Update is called once per frame
    void Update () {

        if (!animate.isPlaying && gameObject.GetComponent<NavMeshAgent>() != null)
        {
            transform.localEulerAngles = new Vector3(ac.rx, ac.ry, ac.rz);
        }

        if(gameObject.GetComponent<NavMeshAgent>() == null)
        {
            print("herep");
            find_location();
        }
                
    }

    public void find_location()
    {
        rb.AddRelativeForce(Vector3.forward * movement_speed);
    }

    public void OnTriggerEnter(Collider other)
    {
       
        if(other.gameObject.name == "Downwind")
        {
            
            entering_downwind();
            nav.SetDestination(perch.transform.position);

        }
        else if(other.gameObject.name == "Perch")
        {
            nav.SetDestination(short_final.transform.position);
            GameObject.Destroy(nav);
            GameObject.Destroy(gameObject.GetComponent<NavMeshAgent>());
            find_location();

        }
    }
}

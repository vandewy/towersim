using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class Spawn : MonoBehaviour
{
    public int ac_count;
    public GameObject Aircraft;
    public List<string> ac_callsigns; //[0] is type, [1] is callsign
    public List<object> characteristics;
    public Aircraft ac;
    public Database db;

    //if departure is false a/c will be an arrival
    public bool departure;

    public void Start()
    {
        db = new Database();
        ac = new Aircraft();
        var ac_callsigns = db.get_unused_callsigns();

        departure = true;
        //will need to be reset after gameobject is destroyed
        db.update_as_departure(ac_callsigns[0].call_sign);
        spawn_aircraft(ac_callsigns[0], departure);

    }

    //if departure is false a/c will spawn airborne inbound to field
    public void spawn_aircraft(Database.Call_Sign_Data aircraft, bool departure)
    {
        characteristics = db.get_ac_characteristics(aircraft.type);

        if(departure == true)
        {
            
            switch ((string)characteristics[8])//departure point
            {
                case "D":
                    Aircraft = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/" + aircraft.type + ".prefab", typeof(GameObject));
                    GameObject clone = GameObject.Instantiate(Aircraft, new Vector3(249f, 4f, -148f), Quaternion.Euler(0f, 0f, 0f));
                    clone.name = aircraft.call_sign;
                    break;
                case "E":
                    Aircraft = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/" + aircraft.type + ".prefab", typeof(GameObject));
                    GameObject clone2 = GameObject.Instantiate(Aircraft, new Vector3(361.5f, 4f, -150f), Quaternion.Euler(0f, 0f, 0f));
                    clone2.name = aircraft.call_sign;
                    break;
                case "G":
                    Aircraft = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/" + aircraft.type + ".prefab", typeof(GameObject));
                    GameObject clone3 = GameObject.Instantiate(Aircraft, new Vector3(461.4f, 4f, -148f), Quaternion.Euler(0f, 0f, 0f));
                    clone3.name = aircraft.call_sign;
                    break;

            }
        }
        else
        {
            Aircraft = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/" + aircraft.type + ".prefab", typeof(GameObject));
            GameObject clone = GameObject.Instantiate(Aircraft, new Vector3(249f, .2f, -148f), Quaternion.Euler(0f, 0f, 0f));
            clone.name = aircraft.call_sign;
        }

    }

    void Fixed_Update()
    {

    }

}
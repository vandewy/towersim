using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class Spawn : MonoBehaviour
{
    public int ac_count;
    public GameObject Aircraft;
    public string[] ac_callsigns;
    public List<object> characteristics;

    public void Start()
    {
        spawn_aircraft("C172");
        ac_count++;
    }

    public void spawn_aircraft(string type)
    {
        Aircraft = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/"+type+".prefab", typeof(GameObject));
        GameObject clone = GameObject.Instantiate(Aircraft);
        clone.name = type;

    }

    void Fixed_Update()
    {

    }

}
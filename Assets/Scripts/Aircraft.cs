using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aircraft : MonoBehaviour {

    public int current_altitude;
    public int new_altitude;
    public string call_sign;
    public int heading;
    public string departure_point;
    public string type;
    public float xVelocity;
    public float yVelocity;
    public float rotation;
    public int climb_rate;
    public int descent_rate;
    public int ground_speed;
    public float turn_rate;
    public bool overhead_break;
    public bool initial_spawn;
    // How quickly an aircraft banks
    public float roll_rate;
    public string weight_class;
    public int category;

    public float px, py, pz;
    public float rx, ry, rz;
    public float xForce, yForce, zForce;

}

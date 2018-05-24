using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Flight : MonoBehaviour {

    public GameObject aircraft;
    public Rigidbody rb;
    public Aircraft ac;

    public float smoothTime = .3f;
    public Vector3 velocity = Vector3.zero;

	// Use this for initialization
	void Start () {
        aircraft = new GameObject();
        rb = GetComponent<Rigidbody>();
        ac = new Aircraft();
        initialize_aircraft(ac);
        velocity.x = 1f;
        
    }

    public void initialize_aircraft(Aircraft a)
    {
        a.overhead_break = true;
        a.xForce = 10f;
        a.yForce = 0f;
        a.zForce = 0f;
        a.rx = -90;// rb.transform.rotation.x;
        a.ry = rb.transform.rotation.y;
        a.rz = rb.transform.rotation.z;
        a.pz = rb.transform.position.z;
        a.py = rb.transform.position.y;
        

    }

    public void break_controller()
    {
        System.Threading.Thread mThread = new System.Threading.Thread(() => overhead_break(ac));
        mThread.Start();
    }

    private void overhead_break(Aircraft ac)
    {
        print("break controller");
        int count = 0;
        float xx, yy, zz;
        xx = ac.rx;
        yy = ac.ry;
        zz = ac.rz;

        int yyTally = 0, xxTally = 0, zzTally = 0;

        while (true)
        {
            //initial break
            System.Threading.Thread.Sleep(120);
            if (xx > -180)
            {
                xx = xx - 5f;
                ac.rx = xx;
            }
            else
                xxTally = 1;

            if (yy < 180)
            {
                yy = yy + 5;
                ac.ry = yy;
            }
            else
                yyTally = 1;



            if (yyTally == 1 && xxTally == 1)
            {
                xxTally = 0;
                yyTally = 0;
                break;
            }
            count++;
            if (count > 1000)
                break;
        }

        //straighten out on break
        while (true)
        {
            System.Threading.Thread.Sleep(70);
            if (xx < -90)
            {

                xx = xx + 3f;
                ac.rx = xx;
            }
            else
            {
                ac.zForce = -5f;
                break;
            }
        }
        
        //force to descend aircraft
        //ac.yForce = 3f;
        System.Threading.Thread.Sleep(5000);
        ac.zForce = -6f;
        //base back to runway
        while (true)
        {
            ac.zForce = 0;
            System.Threading.Thread.Sleep(90);
            if (yy < 360)
            {
                yy = yy + 3.5f;
                ac.ry = yy;
            }
            else
                yyTally = 1;

            if (xx > -180)
            {
                xx = xx - 4f;
                ac.rx = xx;
            }
            else
                xxTally = 1;

            

            if (yyTally == 1 && xxTally == 1)
            {
                xxTally = 0;
                yyTally = 0;
                break;
            }

        }

        //straighten out on short final
       
        while (true)
        {
            System.Threading.Thread.Sleep(100);
            if (xx < -90)
            {
                xx = xx + 4;
                ac.rx = xx;
            }
            else
                break;
        }




    }

    public void rotate()
    {
        //rb.transform.Rotate(Vector3.left * 90);
        //rb.transform.localEulerAngles = new Vector3(ac.rx, ac.ry, ac.rz);
        //rb.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), 5f);
        ac.rx = -180;
        ac.ry = 90;
        ac.rz = -5;
        

        //ac.xForce = -1;
    }

    // Update is called once per frame
    void Update () {

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, 10f);
        Vector3 targetPosition = rb.transform.TransformPoint(new Vector3(ac.xForce, ac.yForce, ac.zForce));
        rb.transform.position = Vector3.SmoothDamp(rb.transform.position, targetPosition, ref velocity, smoothTime);
        rb.transform.localEulerAngles = new Vector3(ac.rx, ac.ry, ac.rz);
        //rb.AddForce(ac.xForce, ac.yForce, ac.zForce);
        //rb.transform.Rotate(ac.rx, ac.ry, ac.rz);

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, 10f);
        if (rb.transform.position.x >= 275 && ac.overhead_break == true)
        {

            break_controller();
            //rotate();
            ac.overhead_break = false;
        }
	}
}

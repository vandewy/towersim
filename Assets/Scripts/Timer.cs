using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Timer : MonoBehaviour
{
    private string time_to_string;
    private int time_num;
    public float count_down;
    public int minutes_count;
    public bool time_up;
    public float countdown_speed;
    Text txt;

    // Use this for initialization
    void Start()
    {
        minutes_count = 0;
        count_down = 0;
        time_up = false;
        time_num = 0;
        time_to_string = "";
        countdown_speed = 0.02f;
        txt = GetComponent<Text>();
        txt.color = Color.black;
    }

    // Update is called once per frame
    void Update()
    {
        if (time_up == false)
        {
            Countdown();
        }
        else
        {
            //Destroy(GameObject.Find("Canvas"));
        }

    }

    void Countdown()
    {
        print(count_down + " " + minutes_count + " " + time_num);
        count_down += countdown_speed;
        if (minutes_count >= 60 && count_down >= 0)
        {
            //
        }
        else if (count_down < time_num && minutes_count > 0)
        {
            if (count_down < 9 && minutes_count > 0)
            {
                time_to_string = "0" + minutes_count.ToString() + ":0" + time_num.ToString();
            }
            else
            {
                time_to_string = "0" + minutes_count.ToString() + ":" + time_num.ToString();
            }

            time_num++;
            if (count_down <= 0 && minutes_count != 0)
            {
                time_num = 60;
                count_down = 59;
                minutes_count += 1;
            }
            txt.text = time_to_string;
        }
        else if (count_down < time_num && minutes_count == 0)
        {
            if (count_down < 9)
            {
                time_to_string = "0" + minutes_count.ToString() + ":0" + time_num.ToString();
                txt.color = Color.black;
            }
            else
            {
                time_to_string = "0" + minutes_count.ToString() + ":" + time_num.ToString();
                txt.color = Color.black;
            }

            time_num++;
            txt.text = time_to_string;
        }
    }
}


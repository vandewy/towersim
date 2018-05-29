//File is used for all database functions on Simulator.sqlite
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using Mono.Data.Sqlite;

public class Database : MonoBehaviour {

    public string conn;
    public IDbConnection dbconn;
    public string query;
    public List<object> characteristics;

    public List<object> get_ac_characteristics(string type)
    {
        characteristics = new List<object>();
        conn = "URI=file:" + Application.dataPath + "/Simulator.sqlite";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();

        IDbCommand dbcmd = dbconn.CreateCommand();
        query = "SELECT * FROM aircraft WHERE type=" + "'" + type + "'";
        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            string ac_type = reader.GetString(0);//type aircraft
            int climb_rate = reader.GetInt32(1);
            int descent_rate = reader.GetInt32(2);
            float turn_rate = reader.GetFloat(3);
            float roll_rate = reader.GetFloat(4);//related to ac banking
            int ground_speed = reader.GetInt32(5);
            int category = reader.GetInt32(6);
            string weight_class = reader.GetString(7);

            characteristics.Add(ac_type);
            characteristics.Add(climb_rate);
            characteristics.Add(descent_rate);
            characteristics.Add(turn_rate);
            characteristics.Add(roll_rate);
            characteristics.Add(ground_speed);
            characteristics.Add(category);
            characteristics.Add(weight_class);

        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return characteristics;
    }
}

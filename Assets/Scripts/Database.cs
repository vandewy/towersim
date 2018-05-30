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

    public struct Call_Sign_Data
    {
        public Call_Sign_Data(string ac_type, string ac_call_sign)
        {
            type = ac_type;
            call_sign = ac_call_sign;
        }

        public string type { get; private set; }
        public string call_sign { get; private set; }
    }

    public List<Call_Sign_Data> call_sign_data;

    public bool is_departure(string callsign)
    {
        int dep = 0;

        conn = "URI=file:" + Application.dataPath + "/Simulator.sqlite";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();

        IDbCommand dbcmd = dbconn.CreateCommand();
        query = "SELECT departure FROM call_signs WHERE call_sign=" + "'" + callsign + "'";
        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            dep = reader.GetInt32(0);
        }

        if (dep == 1)
            return true;
        else
            return false;
    }

    public void update_as_departure(string callsign)
    {
        conn = "URI=file:" + Application.dataPath + "/Simulator.sqlite";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();

        IDbCommand dbcmd = dbconn.CreateCommand();
        query = "UPDATE call_signs SET departure=1 WHERE call_sign=" + "'" + callsign + "'";
        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();
    }

    //[0] is type a/c, [1] is call_sign
    public List<Call_Sign_Data> get_unused_callsigns()
    {
        call_sign_data = new List<Call_Sign_Data>();
        conn = "URI=file:" + Application.dataPath + "/Simulator.sqlite";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();

        IDbCommand dbcmd = dbconn.CreateCommand();
        query = "SELECT type, call_sign FROM call_signs WHERE used=0";
        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            call_sign_data.Add(new Call_Sign_Data(reader.GetString(0), reader.GetString(1)));
        }

        return call_sign_data;
    }

    //return type aircraft
    public string get_type(string callsign)
    {
        string type = "";
        conn = "URI=file:" + Application.dataPath + "/Simulator.sqlite";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();

        IDbCommand dbcmd = dbconn.CreateCommand();
        query = "SELECT type FROM call_signs WHERE call_sign=" +"'" + callsign + "'";
        dbcmd.CommandText = query;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            type = reader.GetString(0);
        }

        return type;
    }

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
            string departure_point = reader.GetString(8);

            characteristics.Add(ac_type);
            characteristics.Add(climb_rate);
            characteristics.Add(descent_rate);
            characteristics.Add(turn_rate);
            characteristics.Add(roll_rate);
            characteristics.Add(ground_speed);
            characteristics.Add(category);
            characteristics.Add(weight_class);
            characteristics.Add(departure_point);

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

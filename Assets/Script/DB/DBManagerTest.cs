using MySql.Data.MySqlClient;
using System.Data;
using System;
using UnityEngine;
using System.Collections.Generic;

public class DBManagerTest : MonoBehaviour
{
    private MySqlConnection SqlConn;
    private MySqlCommand cmd;

    static string ipAddress = "localhost";
    static string db_id = "root";
    static string db_pw = "";
    static string db_name = "mydb";
    static string strConn = new MySqlConnectionStringBuilder
    {
        Server = ipAddress,
        UserID = db_id,
        Password = db_pw,
        Database = db_name,
        CharacterSet = "utf8",
        SslMode = MySqlSslMode.None
    }.ToString();

    public static DBManagerTest instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        try
        {
            SqlConn = new MySqlConnection(strConn);
            cmd = new MySqlCommand();
            cmd.Connection = SqlConn;
            Debug.Log("DBManagerTest initialized.");
        }
        catch (Exception e)
        {
            Debug.LogError("Error initializing DBManagerTest: " + e.ToString());
        }
    }

    public bool SetRecordHighScore(HighScore score)
    {
        if (SqlConn == null || cmd == null)
        {
            Debug.LogError("SetRecordHighScore method called with SqlConn or cmd being null.");
            return false;
        }

        try
        {
            if (GetRecordHighScore(out HighScore dbScore, score.userID) == false)
                return false;

            if (score.money < dbScore.money)
                score.money = dbScore.money;
            if (score.stars < dbScore.stars)
                score.stars = dbScore.stars;
            if (score.lifeTime < dbScore.lifeTime)
                score.lifeTime = dbScore.lifeTime;

            SqlConn.Open();

            cmd.CommandText = $"Update User_Record Set Money = {score.money}, Stars = {score.stars}, Life_Time = '{score.lifeTime}' Where User_Id = {score.userID}";

            int result = cmd.ExecuteNonQuery();

            if (result < 0)
            {
                Debug.Log("Database operation failed.");
                SqlConn.Close();
                return false;
            }

            SqlConn.Close();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Database operation failed: " + e.ToString());
            return false;
        }
    }

    public bool GetRecordHighScore(out HighScore highScore, int userID = 1)
    {
        highScore = new HighScore();

        if (SqlConn == null || cmd == null)
        {
            Debug.LogError("GetRecordHighScore method called with SqlConn or cmd being null.");
            return false;
        }

        try
        {
            SqlConn.Open();

            cmd.CommandText = $"Select * From User_Record Where User_Id = {userID}";
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                highScore.userID = reader.GetInt32(0);
                highScore.stars = reader.GetInt32(1);
                highScore.money = reader.GetInt32(2);
                highScore.lifeTime = reader.GetInt32(3);

                SqlConn.Close();
                return true;
            }
            else
            {
                SqlConn.Close();
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Database operation failed: " + e.ToString());
            return false;
        }
    }

    public List<SkillDataStruct> GetIngameSkillData()
    {
        if (SqlConn == null || cmd == null)
        {
            Debug.LogError("GetIngameSkillData method called with SqlConn or cmd being null.");
            return null;
        }

        try
        {
            SqlConn.Open();

            cmd.CommandText = "Select * from ingameskill";
            MySqlDataReader reader = cmd.ExecuteReader();

            List<SkillDataStruct> skillDataList = new List<SkillDataStruct>();
            while (reader.Read())
            {
                SkillDataStruct skillData = new SkillDataStruct
                {
                    skillName = reader.GetString(0),
                    skillInfo = reader.GetString(1),
                    increase = reader.GetInt32(2),
                    price = 0
                };
                skillDataList.Add(skillData);
            }

            SqlConn.Close();
            return skillDataList;
        }
        catch (Exception e)
        {
            Debug.LogError("Database operation failed: " + e.ToString());
            return null;
        }
    }
}

using MySql.Data.MySqlClient;
using System.Data;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Drawing;

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
            if (GetRecordHighScore(out HighScore dbScore, score.charactorName, score.userID) == false)
                return false;

            if (score.money < dbScore.money)
                score.money = dbScore.money;
            if (score.stars < dbScore.stars)
                score.stars = dbScore.stars;
            if (score.lifeTime < dbScore.lifeTime)
                score.lifeTime = dbScore.lifeTime;

            SqlConn.Open();

            cmd.CommandText = $"Update users_charactor Set Money = {score.money}, Stars_Record = {score.stars}, Life_Time_Record = '{score.lifeTime}' Where User_Id = {score.userID} AND Charactor_Name = '{score.charactorName}'";

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

    public bool GetRecordHighScore(out HighScore highScore, string character, int userID = 1)
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

            cmd.CommandText = $"Select * From users_charactor Where User_Id = {userID} And Charactor_Name = '{character}'";
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                highScore.charactorName = reader.GetString(0);
                highScore.userID = reader.GetInt32(1);
                highScore.stars = reader.GetInt32(3);
                highScore.money = reader.GetInt32(4);
                highScore.lifeTime = reader.GetInt32(5);

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
    public List<CharactorData> GetCharactorData()
    {
        if (SqlConn == null)
        {
            Debug.LogError("GetCharactorData 메서드에서 SqlConn이 null입니다.");
            return null;
        }
        try
        {
            SqlConn.Open();   //DB 연결

            cmd.CommandText = "Select Charactor_Name, Move_Speed, Health from Charactor";
            MySqlDataReader reader = cmd.ExecuteReader();

            List<CharactorData> charactorDataList = new List<CharactorData>();
            CharactorData charactorData;
            while (reader.Read())
            {
                charactorData.name = reader.GetString(0);
                charactorData.moveSpeed = reader.GetFloat(1);
                charactorData.health = reader.GetFloat(2);
                charactorDataList.Add(charactorData);
            }

            SqlConn.Close();  //DB 연결 해제

            return charactorDataList;
        }
        catch (Exception e)
        {
            Debug.LogError("데이터베이스 작업 실패: " + e.ToString());
            SqlConn.Close();
            return null;
        }
    }
    public int GetMoney(string charactor, int userID = 1)
    {
        if (SqlConn == null)
        {
            Debug.LogError("GetMoney 메서드에서 SqlConn이 null입니다.");
            return -1;
        }
        try
        {
            SqlConn.Open();   // DB 연결

            cmd.CommandText = $"Select Money From Users_Charactor Where User_Id = {userID} and Charactor_Name = '{charactor}'";
            MySqlDataReader reader = cmd.ExecuteReader();

            int money = -1;

            if (reader.Read())
            {
                money = reader.GetInt32(0);
            }

            SqlConn.Close();
            return money;
        }
        catch (Exception e)
        {
            Debug.LogError("예외 발생: " + e.ToString());
            SqlConn.Close();
            return -1;
        }
    }

    public bool SetMoney(int money, string charactor, int userID = 1)
    {
        if (SqlConn == null)
        {
            Debug.LogError("SetMoney 메서드에서 SqlConn이 null입니다.");
            return false;
        }
        try
        {
            if (SqlConn.State == System.Data.ConnectionState.Closed)
            {
                SqlConn.Open();   // DB 연결
            }

            cmd.CommandText = $"Update Users_Charactor set money = {money} Where User_Id = {userID} AND Charactor_Name = '{charactor}'";
            int result = cmd.ExecuteNonQuery();

            if (result < 0)
            {
                Debug.Log("데이터 업데이트에 실패했습니다.");
                SqlConn.Close();  // DB 연결 해제
                return false;
            }

            SqlConn.Close();  // DB 연결 해제
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("예외 발생: " + e.ToString());
            SqlConn.Close();  // DB 연결 해제
            return false;
        }
    }

}

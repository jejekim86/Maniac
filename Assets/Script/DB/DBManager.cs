using MySql.Data.MySqlClient;
using System.Data;
using System;
using UnityEngine;
using System.Data.SqlClient;
using System.Collections.Generic;

public class DBManager : MonoBehaviour
{
    //static readonly string ipAddress = "localhost";
    //static readonly string db_id = "root";
    //static readonly string db_pw = "";
    //static readonly string db_name = "mydb";

    private string strConn;
    public static DBManager Instance { get; private set; }

    private MySqlConnection SqlConn;

    private MySqlCommand cmd = new MySqlCommand();

    public DBManager(string ipAddress = "localhost", string User = "root", string pw = "", string dbName = "mydb")
    {
        strConn = new MySqlConnectionStringBuilder
        {
            Server = ipAddress,
            UserID = User,
            Password = pw,
            Database = dbName,
            CharacterSet = "utf8",
            SslMode = MySqlSslMode.None
        }.ToString();

        try
        {
            SqlConn = new MySqlConnection(strConn);
            cmd.Connection = SqlConn;
            Debug.Log("데이터베이스 연결 문자열이 초기화되었습니다.");
        }
        catch (Exception e)
        {
            Debug.LogError("데이터베이스 연결 문자열 초기화 실패: " + e.ToString());
        }
    }

    /*
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        try
        {
            SqlConn = new MySqlConnection(strConn);
            Debug.Log("데이터베이스 연결 문자열이 초기화되었습니다.");
        }
        catch (Exception e)
        {
            Debug.LogError("데이터베이스 연결 문자열 초기화 실패: " + e.ToString());
        }
    }

    void Start()
    {
        //DataSet ds = GetSkillData();
        //string sjon = JsonConvert.SerializeXmlNode(ds.GetXml());
        //Debug.Log(ds.GetXml());
        //Debug.Log(InsertTestData());
        //UpdateGold(1000);
        //Debug.Log(UpdateSkillLevelData("자석", 1));
        //Debug.Log(SetMoney(1000));
        //Debug.Log(GetMoney());
        //SetRecord(123, 2000, "6000");
        //DataSet ds = GetRecord();
        //Debug.Log(ds.GetXml());
        List<SkillDataStruct> list = GetSkillData();
    }
    */

    public bool SetRecordHighScore(HighScore score)
    {
        if (SqlConn == null)
        {
            Debug.LogError("SetRecord 메서드에서 SqlConn이 null입니다.");
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

            SqlConn.Open();   //DB 연결

            cmd.CommandText = $"Update User_Record Set Money = {score.money}, Stars = {score.stars}, Life_Time = '{score.lifeTime}' Where User_Id = {score.userID}";

            int result = cmd.ExecuteNonQuery();

            if (result < 0)
            {
                Debug.Log("데이터베이스 작업 실패: ");
                SqlConn.Close();  //DB 연결 해제
                return false;
            }

            SqlConn.Close();  //DB 연결 해제

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("데이터베이스 작업 실패: " + e.ToString());
            return false;
        }
    }

    public bool GetRecordHighScore(out HighScore highScore, int userID = 1)
    {
        highScore = new HighScore();

        if (SqlConn == null)
        {
            Debug.LogError("GetRecord 메서드에서 SqlConn이 null입니다.");
            return false;
        }

        try
        {
            SqlConn.Open();   //DB 연결

            cmd.CommandText = $"Select * From User_Record Where User_Id = {userID}";
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                highScore.userID = reader.GetInt32(0);
                highScore.stars = reader.GetInt32(1);
                highScore.money = reader.GetInt32(2);
                highScore.lifeTime = reader.GetInt32(3);

                SqlConn.Close();  //DB 연결 해제
                return true;
            }
            else
            {
                SqlConn.Close();  //DB 연결 해제
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("데이터베이스 작업 실패: " + e.ToString());
            return false;
        }
    }

    /// <summary>
    /// 데이터 베이스에서 돈을 가져옴
    /// </summary>
    /// <returns></returns>
    public int GetMoney(int userID = 1)
    {
        if (SqlConn == null)
        {
            Debug.LogError("GetMoney 메서드에서 SqlConn이 null입니다.");
            return -1;
        }
        try
        {
            SqlConn.Open();   // DB 연결

            cmd.CommandText = $"Select Money From User Where User_Id = {userID};";
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


    public bool AddMoney(int money, int userID = 1)
    {
        if (SqlConn == null)
        {
            Debug.LogError("UpdateGold 메서드에서 SqlConn이 null입니다.");
            return false;
        }
        try
        {
            int getMoney = GetMoney(userID);
            if (getMoney < 0)
                return false;
            money += getMoney;
            if (money < 0)
                return false;

            if (SqlConn.State == System.Data.ConnectionState.Closed)
            {
                SqlConn.Open();   // DB 연결
            }

            cmd.CommandText = $"Update User set money = {money} Where User_Id = {userID};";
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

    public List<SkillDataStruct> GetSkillData()
    {
        if (SqlConn == null)
        {
            Debug.LogError("GetSkillData 메서드에서 SqlConn이 null입니다.");
            return null;
        }
        try
        {
            SqlConn.Open();   //DB 연결

            cmd.CommandText = "Select * from skill";
            MySqlDataReader reader = cmd.ExecuteReader();

            List<SkillDataStruct> skillDataList = new List<SkillDataStruct>();
            SkillDataStruct skillData;
            while (reader.Read())
            {
                skillData.skillName = reader.GetString(0);
                skillData.skillInfo = reader.GetString(1);
                skillData.increase = reader.GetInt32(2);
                skillDataList.Add(skillData);
            }

            SqlConn.Close();  //DB 연결 해제

            return skillDataList;
        }
        catch (Exception e)
        {
            Debug.LogError("데이터베이스 작업 실패: " + e.ToString());
            SqlConn.Close();
            return null;
        }
    }

    public int? GetSkillLevel(string skillName, int userID = 1)
    {
        if (SqlConn == null)
        {
            Debug.LogError("GetSkillLevel 메서드에서 SqlConn이 null입니다.");
            return null;
        }

        try
        {
            SqlConn.Open();   //DB 연결

            cmd.CommandText = $"Select Level From Upgrade_Skill Where User_Id = {userID} AND Skill_Name = '{skillName}'";
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                int level = reader.GetInt32(0);
                reader.Close();
                SqlConn.Close();
                return level;
            }
            else
            {
                reader.Close();
                SqlConn.Close();
                return -1;
            }

        }
        catch (Exception e)
        {
            Debug.LogError("데이터베이스 작업 실패: " + e.ToString());
            SqlConn.Close();
            return null;
        }
    }

    public bool UpdateSkillLevelData(string skillName, int changeAmount, int userID = 1)
    {
        if (SqlConn == null)
        {
            Debug.LogError("UpdateSkillLevelData 메서드에서 SqlConn이 null입니다.");
            return false;
        }

        try
        {
            int? nowLevel = GetSkillLevel(skillName);

            if (nowLevel == null || nowLevel < 0)
                return false;

            SqlConn.Open();   //DB 연결

            cmd.CommandText = $"Update Upgrade_Skill Set Level = {nowLevel + changeAmount} Where User_Id = {userID} AND Skill_Name = '{skillName}'";
            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                Debug.Log("데이터가 성공적으로 업데이트했습니다.");
                SqlConn.Close();  //DB 연결 해제
                return true;
            }
            else
            {
                Debug.Log("데이터 업데이트에 실패했습니다.");
                SqlConn.Close();  //DB 연결 해제
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("데이터베이스 작업 실패: " + e.ToString());
            SqlConn.Close();  //DB 연결 해제
            return false;
        }
    }
}

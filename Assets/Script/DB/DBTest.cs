using MySql.Data.MySqlClient;
using System.Data;
using System;
using UnityEngine;
using System.Data.SqlClient;

public class DBTest : MonoBehaviour
{
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

    static string selectAllSkillData = "Select * from skill";

    public static MySqlConnection SqlConn;

    private void Awake()
    {
        try
        {
            SqlConn = new MySqlConnection(strConn);
            Debug.Log("�����ͺ��̽� ���� ���ڿ��� �ʱ�ȭ�Ǿ����ϴ�.");
        }
        catch (Exception e)
        {
            Debug.LogError("�����ͺ��̽� ���� ���ڿ� �ʱ�ȭ ����: " + e.ToString());
        }
    }

    void Start()
    {
        //DataSet ds = GetSkillData();
        //string sjon = JsonConvert.SerializeXmlNode(ds.GetXml());
        //Debug.Log(ds.GetXml());
        //Debug.Log(InsertTestData());
        //UpdateGold(1000);
        //Debug.Log(UpdateSkillLevelData("�ڼ�", 1));
        //Debug.Log(SetMoney(1000));
        //Debug.Log(GetMoney());
        SetRecord(123, 2000, "6000");
        DataSet ds = GetRecord();
        Debug.Log(ds.GetXml());
    }

    public bool SetRecord(int stars, int money, string time)
    {
        if (SqlConn == null)
        {
            Debug.LogError("SetRecord �޼��忡�� SqlConn�� null�Դϴ�.");
            return false;
        }

        try
        {
            SqlConn.Open();   //DB ����

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = SqlConn;
            cmd.CommandText = $"Update User_Record Set Money = {money}, Stars = {stars}, Life_Time = '{time}' Where User_Id = 1";

            MySqlDataAdapter sd = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sd.Fill(ds, "Skill");

            SqlConn.Close();  //DB ���� ����

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("�����ͺ��̽� �۾� ����: " + e.ToString());
            return false;
        }
    }

    public DataSet GetRecord()
    {
        if (SqlConn == null)
        {
            Debug.LogError("GetRecord �޼��忡�� SqlConn�� null�Դϴ�.");
            return null;
        }

        try
        {
            SqlConn.Open();   //DB ����

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = SqlConn;
            cmd.CommandText = "Select * From User_Record Where User_Id = 1";

            MySqlDataAdapter sd = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sd.Fill(ds, "User_Record");


            SqlConn.Close();  //DB ���� ����

            return ds;
        }
        catch (Exception e)
        {
            Debug.LogError("�����ͺ��̽� �۾� ����: " + e.ToString());
            return null;
        }
    }

    /// <summary>
    /// ������ ���̽����� ���� ������
    /// </summary>
    /// <returns></returns>
    public int GetMoney()
    {
        if (SqlConn == null)
        {
            Debug.LogError("GetMoney �޼��忡�� SqlConn�� null�Դϴ�.");
            return -1;
        }
        try
        {
            if (SqlConn.State == System.Data.ConnectionState.Closed)
            {
                SqlConn.Open();   // DB ����
            }
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = SqlConn;
            cmd.CommandText = "Select Money From User Where User_Id = 1;";
            MySqlDataReader reader = cmd.ExecuteReader();

            int money = -1;

            if (reader.Read())
            {
                money = reader.GetInt32(0);
            }
            return money;
        }
        catch (Exception e)
        {   
            Debug.LogError("���� �߻�: " + e.ToString());
            return -1;
        }
        finally
        {
            if (SqlConn.State == System.Data.ConnectionState.Open)
            {
                SqlConn.Close();  // DB ���� ����
            }
        }
    }


    public bool SetMoney(int money)
    {
        if (SqlConn == null)
        {
            Debug.LogError("UpdateGold �޼��忡�� SqlConn�� null�Դϴ�.");
            return false;
        }
        try
        {
            int getMoney = GetMoney();
            if( getMoney < 0 )
                return false;
            money += getMoney;
            if(money < 0)
                return false;

            if (SqlConn.State == System.Data.ConnectionState.Closed)
            {
                SqlConn.Open();   // DB ����
            }

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = SqlConn;
            cmd.CommandText = $"Update User set money = {money} Where User_Id = 1;";
            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                Debug.Log("�����Ͱ� ���������� ������Ʈ�߽��ϴ�.");
            }
            else
            {
                Debug.Log("������ ������Ʈ�� �����߽��ϴ�.");
            }
            if (SqlConn.State == System.Data.ConnectionState.Open)
            {
                SqlConn.Close();  // DB ���� ����
            }
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("���� �߻�: " + e.ToString());
            if (SqlConn.State == System.Data.ConnectionState.Open)
            {
                SqlConn.Close();  // DB ���� ����
            }
            return false;
        }
        finally
        {
            if (SqlConn.State == System.Data.ConnectionState.Open)
            {
                SqlConn.Close();  // DB ���� ����
            }
        }
    }

    public DataSet GetSkillData()
    {
        if (SqlConn == null)
        {
            Debug.LogError("GetSkillData �޼��忡�� SqlConn�� null�Դϴ�.");
            return null;
        }

        try
        {
            SqlConn.Open();   //DB ����

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = SqlConn;
            cmd.CommandText = selectAllSkillData;

            MySqlDataAdapter sd = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sd.Fill(ds, "Skill");

            SqlConn.Close();  //DB ���� ����

            return ds;
        }
        catch (Exception e)
        {
            Debug.LogError("�����ͺ��̽� �۾� ����: " + e.ToString());
            return null;
        }
    }

    public int? GetSkillLevel(string skillName)
    {
        if (SqlConn == null)
        {
            Debug.LogError("GetSkillLevel �޼��忡�� SqlConn�� null�Դϴ�.");
            return null;
        }

        try
        {
            SqlConn.Open();   //DB ����

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = SqlConn;
            cmd.CommandText = $"Select Level From Upgrade_Skill Where User_Id = 1 AND Skill_Name = '{skillName}'";
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
            Debug.LogError("�����ͺ��̽� �۾� ����: " + e.ToString());
            return null;
        }
    }

    public bool UpdateSkillLevelData(string skillName, int changeAmount)
    {
        if (SqlConn == null)
        {
            Debug.LogError("UpdateSkillLevelData �޼��忡�� SqlConn�� null�Դϴ�.");
            return false;
        }

        try
        {
            int? nowLevel = GetSkillLevel(skillName);

            if (nowLevel == null || nowLevel < 0)
                return false;

            SqlConn.Open();   //DB ����

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = SqlConn;
            cmd.CommandText = $"Update Upgrade_Skill Set Level = {nowLevel + changeAmount} Where User_Id = 1 AND Skill_Name = '{skillName}'";
            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                Debug.Log("�����Ͱ� ���������� ������Ʈ�߽��ϴ�.");
                SqlConn.Close();  //DB ���� ����
                return true;
            }
            else
            {
                Debug.Log("������ ������Ʈ�� �����߽��ϴ�.");
                SqlConn.Close();  //DB ���� ����
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("�����ͺ��̽� �۾� ����: " + e.ToString());
            return false;
        }
    }

    
}

using MySql.Data.MySqlClient;
using System.Data;
using System;
using UnityEngine;
using System.Data.SqlClient;
using static UnityEngine.GraphicsBuffer;

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
        /*
        if (SqlConn == null)
        {
            Debug.LogError("Start �޼��忡�� SqlConn�� null�Դϴ�.");
            return;
        }

        //string sql = "SELECT * FROM user;";
        string sql = "Select * from skill";
        DataSet ds = OnSelectRequest(sql, "skill");

        if (ds != null)
        {
            Debug.Log(ds.GetXml());
            //Debug.Log(ds.)
        }
        else
        {
            Debug.LogError("�����ͺ��̽����� �����͸� �������� �� �����߽��ϴ�.");
        }
        */
        DataSet ds = GetSkillData();
        //string sjon = JsonConvert.SerializeXmlNode(ds.GetXml());
        Debug.Log(ds.GetXml());
    }

    public static DataSet GetSkillData()
    {
        if (SqlConn == null)
        {
            Debug.LogError("OnSelectRequest �޼��忡�� SqlConn�� null�Դϴ�.");
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

    public static DataSet OnSelectRequest(string p_query, string table_name)
    {
        if (SqlConn == null)
        {
            Debug.LogError("OnSelectRequest �޼��忡�� SqlConn�� null�Դϴ�.");
            return null;
        }

        try
        {
            SqlConn.Open();   //DB ����

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = SqlConn;
            cmd.CommandText = p_query;
            
            /*
            MySqlDataReader reader = cmd.ExecuteReader();
            
            while (reader.Read())
            {
                Debug.Log(reader.GetInt32(0) + "\n" + reader.GetString(1));
            }
            */

            MySqlDataAdapter sd = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sd.Fill(ds, table_name);

            SqlConn.Close();  //DB ���� ����

            return ds;
        }
        catch (Exception e)
        {
            Debug.LogError("�����ͺ��̽� �۾� ����: " + e.ToString());
            return null;
        }
    }
}

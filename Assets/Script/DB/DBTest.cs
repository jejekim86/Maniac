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
            Debug.Log("데이터베이스 연결 문자열이 초기화되었습니다.");
        }
        catch (Exception e)
        {
            Debug.LogError("데이터베이스 연결 문자열 초기화 실패: " + e.ToString());
        }
    }

    void Start()
    {
        /*
        if (SqlConn == null)
        {
            Debug.LogError("Start 메서드에서 SqlConn이 null입니다.");
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
            Debug.LogError("데이터베이스에서 데이터를 가져오는 데 실패했습니다.");
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
            Debug.LogError("OnSelectRequest 메서드에서 SqlConn이 null입니다.");
            return null;
        }

        try
        {
            SqlConn.Open();   //DB 연결

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = SqlConn;
            cmd.CommandText = selectAllSkillData;

            MySqlDataAdapter sd = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sd.Fill(ds, "Skill");

            SqlConn.Close();  //DB 연결 해제

            return ds;
        }
        catch (Exception e)
        {
            Debug.LogError("데이터베이스 작업 실패: " + e.ToString());
            return null;
        }
    }

    public static DataSet OnSelectRequest(string p_query, string table_name)
    {
        if (SqlConn == null)
        {
            Debug.LogError("OnSelectRequest 메서드에서 SqlConn이 null입니다.");
            return null;
        }

        try
        {
            SqlConn.Open();   //DB 연결

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

            SqlConn.Close();  //DB 연결 해제

            return ds;
        }
        catch (Exception e)
        {
            Debug.LogError("데이터베이스 작업 실패: " + e.ToString());
            return null;
        }
    }
}

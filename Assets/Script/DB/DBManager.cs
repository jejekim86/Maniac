using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DBManager
{
    private string strConn;
    private MySqlConnection SqlConn;
    private MySqlCommand cmd = new MySqlCommand();

    public DBManager(DBConnectionInfo connectionInfo)
    {
        strConn = new MySqlConnectionStringBuilder
        {
            Server = connectionInfo.ipAddress,
            UserID = connectionInfo.user,
            Password = connectionInfo.password,
            Database = connectionInfo.dbName,
            CharacterSet = "utf8",
            SslMode = MySqlSslMode.None
        }.ToString();

        try
        {
            SqlConn = new MySqlConnection(strConn);
            cmd.Connection = SqlConn;
            Debug.Log("�����ͺ��̽� ���� ���ڿ��� �ʱ�ȭ�Ǿ����ϴ�.");
        }
        catch (Exception e)
        {
            Debug.LogError("�����ͺ��̽� ���� ���ڿ� �ʱ�ȭ ����: " + e.ToString());
        }
    }

    public bool SetRecordHighScore(HighScore score)
    {
        if (SqlConn == null)
        {
            Debug.LogError("SetRecord �޼��忡�� SqlConn�� null�Դϴ�.");
            return false;
        }

        try
        {
            if (!GetRecordHighScore(out HighScore dbScore, score.userID))
                return false;

            if (score.money < dbScore.money)
                score.money = dbScore.money;
            if (score.stars < dbScore.stars)
                score.stars = dbScore.stars;
            if (score.lifeTime < dbScore.lifeTime)
                score.lifeTime = dbScore.lifeTime;

            SqlConn.Open();   //DB ����

            cmd.CommandText = $"Update User_Record Set Money = {score.money}, Stars = {score.stars}, Life_Time = '{score.lifeTime}' Where User_Id = {score.userID}";

            int result = cmd.ExecuteNonQuery();

            if (result < 0)
            {
                Debug.Log("�����ͺ��̽� �۾� ����: ");
                SqlConn.Close();  //DB ���� ����
                return false;
            }

            SqlConn.Close();  //DB ���� ����

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("�����ͺ��̽� �۾� ����: " + e.ToString());
            return false;
        }
    }

    public bool GetRecordHighScore(out HighScore highScore, int userID = 1)
    {
        highScore = new HighScore();

        if (SqlConn == null)
        {
            Debug.LogError("GetRecord �޼��忡�� SqlConn�� null�Դϴ�.");
            return false;
        }

        try
        {
            SqlConn.Open();   //DB ����

            cmd.CommandText = $"Select * From User_Record Where User_Id = {userID}";
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                highScore.userID = reader.GetInt32(0);
                highScore.stars = reader.GetInt32(1);
                highScore.money = reader.GetInt32(2);
                highScore.lifeTime = reader.GetInt32(3);

                SqlConn.Close();  //DB ���� ����
                return true;
            }
            else
            {
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

    /// <summary>
    /// ������ ���̽����� ���� ������
    /// </summary>
    /// <returns></returns>
    public int GetMoney(string charactor, int userID = 1)
    {
        if (SqlConn == null)
        {
            Debug.LogError("GetMoney �޼��忡�� SqlConn�� null�Դϴ�.");
            return -1;
        }
        try
        {
            SqlConn.Open();   // DB ����

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
            Debug.LogError("���� �߻�: " + e.ToString());
            SqlConn.Close();
            return -1;
        }
    }

    public bool SetMoney(int money, string charactor, int userID = 1)
    {
        if (SqlConn == null)
        {
            Debug.LogError("SetMoney �޼��忡�� SqlConn�� null�Դϴ�.");
            return false;
        }
        try
        {
            if (SqlConn.State == System.Data.ConnectionState.Closed)
            {
                SqlConn.Open();   // DB ����
            }

            cmd.CommandText = $"Update Users_Charactor set money = {money} Where User_Id = {userID} AND Charactor_Name = '{charactor}'";
            int result = cmd.ExecuteNonQuery();

            if (result < 0)
            {
                Debug.Log("������ ������Ʈ�� �����߽��ϴ�.");
                SqlConn.Close();  // DB ���� ����
                return false;
            }

            SqlConn.Close();  // DB ���� ����
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("���� �߻�: " + e.ToString());
            SqlConn.Close();  // DB ���� ����
            return false;
        }
    }

    public List<SkillDataStruct> GetSkillData()
    {
        if (SqlConn == null)
        {
            Debug.LogError("GetSkillData �޼��忡�� SqlConn�� null�Դϴ�.");
            return null;
        }
        try
        {
            SqlConn.Open();   //DB ����

            cmd.CommandText = "Select * from skill";
            MySqlDataReader reader = cmd.ExecuteReader();

            List<SkillDataStruct> skillDataList = new List<SkillDataStruct>();
            while (reader.Read())
            {
                SkillDataStruct skillData = new SkillDataStruct();
                skillData.skillName = reader.GetString(0);
                skillData.skillInfo = reader.GetString(1);
                skillData.increase = reader.GetInt32(2);
                skillData.price = reader.GetInt32(3);
                skillDataList.Add(skillData);
            }

            SqlConn.Close();  //DB ���� ����

            return skillDataList;
        }
        catch (Exception e)
        {
            Debug.LogError("�����ͺ��̽� �۾� ����: " + e.ToString());
            SqlConn.Close();
            return null;
        }
    }

    public int? GetSkillLevel(string skillName, string charactor, int userID = 1)
    {
        if (SqlConn == null)
        {
            Debug.LogError("GetSkillLevel �޼��忡�� SqlConn�� null�Դϴ�.");
            return null;
        }

        try
        {
            SqlConn.Open();   //DB ����

            cmd.CommandText = $"Select Level From Upgrade_Skill Where User_Id = {userID} AND Skill_Name = '{skillName}' AND Charactor_Name = '{charactor}'";
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
            SqlConn.Close();
            return null;
        }
    }

    public bool UpdateSkillLevelData(string skillName, string charactor, int changeAmount, int userID = 1)
    {
        if (SqlConn == null)
        {
            Debug.LogError("UpdateSkillLevelData �޼��忡�� SqlConn�� null�Դϴ�.");
            return false;
        }

        try
        {
            int? nowLevel = GetSkillLevel(skillName, charactor, userID);

            if (nowLevel == null || nowLevel < 0)
                return false;

            SqlConn.Open();   //DB ����

            cmd.CommandText = $"Update Upgrade_Skill Set Level = {nowLevel + changeAmount} Where User_Id = {userID} AND Skill_Name = '{skillName}' AND Charactor_Name = '{charactor}'";
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
            SqlConn.Close();  //DB ���� ����
            return false;
        }
    }

    public bool GetIdentitySkillData(out SkillDataStruct identity, string charactor)
    {
        identity = new SkillDataStruct();

        if (SqlConn == null)
        {
            Debug.LogError("GetIdentitySkillData �޼��忡�� SqlConn�� null�Դϴ�.");
            return false;
        }

        try
        {
            SqlConn.Open();   //DB ����

            cmd.CommandText = $"Select Identityskill_Name, Identityskill_Info, Identityskill_Increase, Identityskill_Price, Identityskill_Level From Charactor Where Charactor_Name = '{charactor}'";
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                identity.skillName = reader.GetString(0);
                identity.skillInfo = reader.GetString(1);
                identity.increase = reader.GetInt32(2);
                identity.price = reader.GetInt32(3);
                identity.level = reader.GetInt32(4);

                SqlConn.Close();  //DB ���� ����
                return true;
            }
            else
            {
                SqlConn.Close();  //DB ���� ����
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("�����ͺ��̽� �۾� ����: " + e.ToString());
            SqlConn.Close();
            return false;
        }
    }

    public bool UpdateIdentitySkillLevelData(string charactor, int changeAmount, int userID = 1)
    {
        if (SqlConn == null)
        {
            Debug.LogError("UpdateIdentitySkillLevelData �޼��忡�� SqlConn�� null�Դϴ�.");
            return false;
        }

        try
        {
            int? nowLevel = GetIdentitySkillLevel(charactor, userID);

            if (nowLevel == null || nowLevel < 0)
                return false;

            SqlConn.Open();   //DB ����

            cmd.CommandText = $"Update Charactor Set Identityskill_Level = {nowLevel + changeAmount} Where Charactor_Name = '{charactor}'";
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
            SqlConn.Close();  //DB ���� ����
            return false;
        }
    }

    public int? GetIdentitySkillLevel(string charactor, int userID = 1)
    {
        if (SqlConn == null)
        {
            Debug.LogError("GetIdentitySkillLevel �޼��忡�� SqlConn�� null�Դϴ�.");
            return null;
        }

        try
        {
            SqlConn.Open();   //DB ����

            cmd.CommandText = $"Select Identityskill_Level From Charactor Where Charactor_Name = '{charactor}'";
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
            SqlConn.Close();
            return null;
        }
    }

    /// <summary>
    /// ĳ���� ���� ��������
    /// </summary>
    /// <returns></returns>
    public List<CharactorData> GetCharactorData()
    {
        if (SqlConn == null)
        {
            Debug.LogError("GetCharactorData �޼��忡�� SqlConn�� null�Դϴ�.");
            return null;
        }
        try
        {
            SqlConn.Open();   //DB ����

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

            SqlConn.Close();  //DB ���� ����

            return charactorDataList;
        }
        catch (Exception e)
        {
            Debug.LogError("�����ͺ��̽� �۾� ����: " + e.ToString());
            SqlConn.Close();
            return null;
        }
    }

    public List<WeaponDataStruct> GetWeaponData(string charactor)
    {
        if (SqlConn == null)
        {
            Debug.LogError("GetWeaponData �޼��忡�� SqlConn�� null�Դϴ�.");
            return null;
        }
        try
        {
            SqlConn.Open();   //DB ����

            cmd.CommandText = $"Select Weapon_Name, Weapon_Info, Price from Weapon Where Charactor_Name = '{charactor}'";
            MySqlDataReader reader = cmd.ExecuteReader();

            List<WeaponDataStruct> weaponDataList = new List<WeaponDataStruct>();
            WeaponDataStruct weapondata;
            while (reader.Read())
            {
                weapondata.name = reader.GetString(0);
                weapondata.info = reader.GetString(1);
                weapondata.price = reader.GetInt32(2);
                weaponDataList.Add(weapondata);
            }

            SqlConn.Close();  //DB ���� ����

            return weaponDataList;
        }
        catch (Exception e)
        {
            Debug.LogError("�����ͺ��̽� �۾� ����: " + e.ToString());
            SqlConn.Close();
            return null;
        }
    }

    public int CharatorIsLock(string charactor, int userID = 1)
    {
        if (SqlConn == null)
        {
            Debug.LogError("CharatorIsLock �޼��忡�� SqlConn�� null�Դϴ�.");
            return -1;
        }
        try
        {
            SqlConn.Open();   // DB ����

            cmd.CommandText = $"Select Lock From Users_Charactor Where User_Id = {userID} and Charactor_Name = '{charactor}'";
            MySqlDataReader reader = cmd.ExecuteReader();

            int islock = -1;

            if (reader.Read())
            {
                islock = reader.GetInt32(0);
            }

            SqlConn.Close();
            return islock;
        }
        catch (Exception e)
        {
            Debug.LogError("���� �߻�: " + e.ToString());
            SqlConn.Close();
            return -1;
        }
    }

    public bool BuyWeapon(string weaponName, string charactor, int userID = 1)
    {
        if (SqlConn == null)
        {
            Debug.LogError("BuyWeapon �޼��忡�� SqlConn�� null�Դϴ�.");
            return false;
        }

        try
        {
            SqlConn.Open();   // DB ����

            cmd.CommandText = $"Update PurchasedWeapon Set WeaponPurchase = 1 Where Weapon_Name = '{weaponName}' AND User_Id = {userID} AND Charactor_Name = '{charactor}'";
            int result = cmd.ExecuteNonQuery();

            SqlConn.Close();

            return result > 0;
        }
        catch (Exception e)
        {
            Debug.LogError("���� ���� �� ���� �߻�: " + e.ToString());
            SqlConn.Close();
            return false;
        }
    }

    public int WeaponIsBuy(string weaponName, string charactor, int userID = 1)
    {
        if (SqlConn == null)
        {
            Debug.LogError("WeaponIsBuy �޼��忡�� SqlConn�� null�Դϴ�.");
            return -1;
        }
        try
        {
            SqlConn.Open();   // DB ����

            cmd.CommandText = $"Select WeaponPurchase From PurchasedWeapon Where Weapon_Name = '{weaponName}' AND User_Id = {userID} AND Charactor_Name = '{charactor}'";
            MySqlDataReader reader = cmd.ExecuteReader();

            int isBuy = -1;

            if (reader.Read())
            {
                isBuy = reader.GetInt32(0);
            }

            SqlConn.Close();
            return isBuy;
        }
        catch (Exception e)
        {
            Debug.LogError("���� �߻�: " + e.ToString());
            SqlConn.Close();
            return -1;
        }
    }

    public bool RefundWeapon(string weaponName, string charactor, int userID)
    {
        if (SqlConn == null)
        {
            Debug.LogError("RefundWeapon �޼��忡�� SqlConn�� null�Դϴ�.");
            return false;
        }
        try
        {
            SqlConn.Open();   // DB ����

            cmd.CommandText = $"Update PurchasedWeapon Set WeaponPurchase = 0 Where Weapon_Name = '{weaponName}' AND User_Id = {userID} AND Charactor_Name = '{charactor}'";
            int result = cmd.ExecuteNonQuery();

            SqlConn.Close();

            return result > 0;
        }
        catch (Exception e)
        {
            Debug.LogError("���� ȯ�� �� ���� �߻�: " + e.ToString());
            SqlConn.Close();
            return false;
        }
    }
}
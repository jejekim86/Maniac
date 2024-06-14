using UnityEngine;

public struct SkillDataStruct
{
    public string skillName;
    public string skillInfo;
    public int increase;
    public int price;
}

public struct HighScore
{
    public string charactorName;
    public int userID;
    public int stars;
    public int money;
    public int lifeTime;
}

public struct DBConnectionInfo
{
    public string ipAddress;
    public string user;
    public string password;
    public string dbName;
}

public struct CharactorData
{
    public string name;
    public float moveSpeed;
    public float health;
}

public struct WeaponDataStruct
{
    public string name;
    public string info;
    public int price;
}
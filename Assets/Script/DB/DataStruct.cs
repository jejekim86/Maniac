using UnityEngine;

public struct SkillDataStruct
{
    public string skillName;
    public string skillInfo;
    public int increase;
}

public struct HighScore
{
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
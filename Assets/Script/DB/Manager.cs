using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{

    static DBConnectionInfo dBConnectionInf = new DBConnectionInfo
    {
        ipAddress = "localhost",
        user = "root",
        password = "",
        dbName = "mydb"
    };
    
    DBManager DBManager = new DBManager(dBConnectionInf);
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(DBManager.GetMoney());
        List<SkillDataStruct> List = DBManager.GetSkillData();
        HighScore highScore;
        DBManager.GetRecordHighScore(out highScore);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

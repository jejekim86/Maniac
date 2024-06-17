using UnityEngine;

public class PlayerUpgradeManager : MonoBehaviour
{
    [SerializeField] private Player player; // Player 스크립트 참조
    [SerializeField] private DBManager dbManager; // DB 매니저

    public void UpgradeStat(string statName, int upgradeValue)
    {
        switch (statName)
        {
            case "walkSpeed":
                player.walkSpeed += upgradeValue;
                dbManager.UpgradePlayerStat("Santa", "walkSpeed", (int)player.walkSpeed);
                break;
            case "dashPower":
                player.dashPower += upgradeValue;
                dbManager.UpgradePlayerStat("Santa", "dashPower", (int)player.dashPower);
                break;
            case "maxHp":
                player.maxHp += upgradeValue;
                player.curHp = player.maxHp;
                dbManager.UpgradePlayerStat("Santa", "maxHp", (int)player.maxHp);
                player.HP_image.fillAmount = player.maxHp;
                break;
                // 필요한 경우 다른 능력치도 추가할 수 있습니다.
        }
    }
}

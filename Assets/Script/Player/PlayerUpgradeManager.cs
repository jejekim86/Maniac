using UnityEngine;

public class PlayerUpgradeManager : MonoBehaviour
{
    [SerializeField] private Player player; // Player ��ũ��Ʈ ����
    [SerializeField] private DBManager dbManager; // DB �Ŵ���

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
                // �ʿ��� ��� �ٸ� �ɷ�ġ�� �߰��� �� �ֽ��ϴ�.
        }
    }
}

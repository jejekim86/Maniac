//using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;

//public class SkillUpgradeManager : MonoBehaviour
//{
//    static DBConnectionInfo dBConnectionInfo = new DBConnectionInfo
//    {
//        ipAddress = "127.0.0.1",
//        user = "root",
//        password = "",
//        dbName = "mydb"
//    };

//    [SerializeField] private DBManager dbManager = new DBManager(dBConnectionInfo);
//    [SerializeField] private GameObject skillTextPrefab;
//    [SerializeField] private GameObject skillButtonPrefab;
//    [SerializeField] private Transform skillTextContainer;
//    [SerializeField] private Transform skillButtonContainer;
//    [SerializeField] private GameObject dashPrefab;
//    [SerializeField] private Transform dashContainer;
//    [SerializeField] private Text playerMoney;

//    private int playerId = 1; // ���� ����� ID�� ���� �������� ����
//    private int currentMoney;
//    private List<SkillDataStruct> skills;
//    private Dictionary<string, GameObject> skillInfoPanels = new Dictionary<string, GameObject>();
//    private Dictionary<string, Text> skillPriceTexts = new Dictionary<string, Text>();
//    private Color PriceColor = new Color32(104, 204, 128, 255); // #68CC80

//    void Start()
//    {
//        LoadPlayerMoney();
//        LoadSkills();
//    }

//    void LoadPlayerMoney()
//    {
//        currentMoney = dbManager.GetMoney(playerId);
//        UpdatePlayerMoneyUI();
//    }

//    void UpdatePlayerMoneyUI()
//    {
//        playerMoney.text = currentMoney.ToString();
//    }

//    void LoadSkills()
//    {
//        skills = dbManager.GetSkillData();

//        GameObject dashText = Instantiate(dashPrefab, dashContainer);
//        Image dash = dashText.transform.GetChild(0).GetComponent<Image>();
//        string imageDashPath = $"Skills/Dash";
//        Sprite dashSprite = Resources.Load<Sprite>(imageDashPath);
//        if (dashSprite != null)
//        {
//            dash.sprite = dashSprite;
//        }
//        else
//        {
//            Debug.LogError($"�̹����� �ε��� �� �����ϴ�: {imageDashPath}");
//        }

//        for (int i = 0; i < skills.Count; i++)
//        {
//            var skill = skills[i];

//            GameObject skillText = Instantiate(skillTextPrefab, skillTextContainer);
//            skillText.SetActive(false); // �ʱ⿡�� ��Ȱ��ȭ

//            // �ؽ�Ʈ ��� ����
//            skillText.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = skill.skillName;
//            skillText.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = skill.skillInfo;
//            skillText.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = skill.increase.ToString();
//            skillText.transform.GetChild(0).GetChild(5).GetChild(0).GetComponent<Text>().text = skill.price.ToString();

//            // ��ų ���� �г��� ��ųʸ��� ����
//            skillInfoPanels[skill.skillName] = skillText;

//            GameObject skillButton = Instantiate(skillButtonPrefab, skillButtonContainer);

//            // ��ų ���� ��������
//            int skillLevel = dbManager.GetSkillLevel(skill.skillName, playerId).GetValueOrDefault();

//            // ��ų ���� UI ����
//            Text skillLevelText = skillText.transform.GetChild(0).GetChild(3).GetComponent<Text>();
//            if (skillLevel > 0)
//            {
//                skillLevelText.text = $"level {skillLevel}";
//                skillLevelText.gameObject.SetActive(true);
//            }
//            else
//            {
//                skillLevelText.gameObject.SetActive(false);
//            }

//            // ��ư ��� ���� �� ��ų �̸� ����
//            Button upgradeButton = skillButton.transform.GetChild(0).GetComponent<Button>();
//            Text skillPriceText = skillButton.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
//            skillPriceText.text = skill.price.ToString();
//            string skillName = skill.skillName; // �ݵ�� ���� ���� ��� -> ��...? (���� ĸó ���� ����)
//            upgradeButton.onClick.AddListener(() => UpgradeSkill(skillName, skill.price, skillLevelText, skillPriceText));

//            // ȯ�� �ȳ�â ����
//            Image skillRefund = skillText.transform.GetChild(0).GetChild(6).GetComponent<Image>();
//            skillRefund.gameObject.SetActive(skillLevel > 0);

//            // �ʱ� ���� ���� ����
//            skillPriceTexts[skill.skillName] = skillPriceText;
//            if (currentMoney < skill.price)
//            {
//                skillPriceText.color = Color.red;
//            }
//            else
//            {
//                skillPriceText.color = PriceColor;
//            }

//            // �̹��� ����
//            Image skillImage = skillButton.transform.GetChild(0).GetChild(0).GetComponent<Image>();
//            string imageSkillPath = $"Skills/Skill_{i + 1}"; // �̹��� �̸� Skill_1, Skill_2, ... ����

//            Sprite skillSprite = Resources.Load<Sprite>(imageSkillPath);
//            if (skillSprite != null)
//            {
//                skillImage.sprite = skillSprite;
//            }
//            else
//            {
//                Debug.LogError($"�̹����� �ε��� �� �����ϴ�: {imageSkillPath}");
//            }

//            // EventTrigger �߰�
//            EventTrigger trigger = skillButton.AddComponent<EventTrigger>();

//            // PointerEnter �̺�Ʈ �߰�
//            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
//            entryEnter.eventID = EventTriggerType.PointerEnter;
//            entryEnter.callback.AddListener((eventData) => { ShowSkillInfo(skillName); });
//            trigger.triggers.Add(entryEnter);

//            // PointerExit �̺�Ʈ �߰�
//            EventTrigger.Entry entryExit = new EventTrigger.Entry();
//            entryExit.eventID = EventTriggerType.PointerExit;
//            entryExit.callback.AddListener((eventData) => { HideSkillInfo(skillName); });
//            trigger.triggers.Add(entryExit);

//            // ��Ŭ�� �̺�Ʈ �߰�
//            EventTrigger.Entry entryRightClick = new EventTrigger.Entry();
//            entryRightClick.eventID = EventTriggerType.PointerClick;
//            entryRightClick.callback.AddListener((eventData) => {
//                if (((PointerEventData)eventData).button == PointerEventData.InputButton.Right)
//                {
//                    RefundSkill(skillName, skill.price, skillLevelText, skillPriceText);
//                }
//            });
//            trigger.triggers.Add(entryRightClick);
//        }
//    }

//    void ShowSkillInfo(string skillName)
//    {
//        if (skillInfoPanels.TryGetValue(skillName, out GameObject skillText))
//        {
//            skillText.SetActive(true);
//        }
//        else
//        {
//            Debug.LogError($"��ų ������ ã�� �� �����ϴ�: {skillName}");
//        }
//    }

//    void HideSkillInfo(string skillName)
//    {
//        if (skillInfoPanels.TryGetValue(skillName, out GameObject skillText))
//        {
//            skillText.SetActive(false);
//        }
//        else
//        {
//            Debug.LogError($"��ų ������ ã�� �� �����ϴ�: {skillName}");
//        }
//    }

//    void UpgradeSkill(string skillName, int skillPrice, Text skillLevelText, Text skillPriceText)
//    {
//        if (currentMoney >= skillPrice)
//        {
//            bool success = dbManager.UpdateSkillLevelData(skillName, 1, playerId);
//            if (success)
//            {
//                currentMoney -= skillPrice;

//                // �����ͺ��̽��� ���� �� ������Ʈ
//                dbManager.SetMoney(currentMoney, playerId);

//                UpdatePlayerMoneyUI();

//                // ��ų ���� ������Ʈ
//                int newLevel = dbManager.GetSkillLevel(skillName, playerId).GetValueOrDefault();
//                skillLevelText.text = $"level {newLevel}";
//                skillLevelText.gameObject.SetActive(newLevel > 0);

//                // ���� ����� ���
//                skillPriceText.color = PriceColor;

//                // ȯ�� â Ȱ��ȭ
//                Transform skillWindow = skillLevelText.transform.parent.parent;
//                Image skillRefund = skillWindow.transform.GetChild(0).GetChild(6).GetComponent<Image>();
//                skillRefund.gameObject.SetActive(true);

//                // ��� ��ų ���� ������Ʈ
//                UpdateSkillPrices();

//                Debug.Log($"��ų {skillName} ���׷��̵忡 �����߽��ϴ�.");
//            }
//            else
//            {
//                Debug.Log($"��ų {skillName} ���׷��̵忡 �����߽��ϴ�.");
//            }
//        }
//        else
//        {
//            // ���� ������ ���, �ؽ�Ʈ ���� ���������� ����
//            skillPriceText.color = Color.red;
//            Debug.Log("���� �����մϴ�.");
//        }
//    }

//    void RefundSkill(string skillName, int skillPrice, Text skillLevelText, Text skillPriceText)
//    {
//        int currentLevel = dbManager.GetSkillLevel(skillName, playerId).GetValueOrDefault();
//        if (currentLevel > 0)
//        {
//            bool success = dbManager.UpdateSkillLevelData(skillName, -1, playerId);
//            if (success)
//            {
//                currentMoney += skillPrice;

//                // �����ͺ��̽��� ���� �� ������Ʈ
//                dbManager.SetMoney(currentMoney, playerId);

//                UpdatePlayerMoneyUI();

//                // ��ų ���� ������Ʈ
//                int newLevel = dbManager.GetSkillLevel(skillName, playerId).GetValueOrDefault();
//                skillLevelText.text = $"level {newLevel}";
//                skillLevelText.gameObject.SetActive(newLevel > 0);

//                // ȯ�� �� ���� ����ϸ� ���� �ؽ�Ʈ ���� ����
//                if (currentMoney >= skillPrice)
//                {
//                    skillPriceText.color = PriceColor;
//                }

//                // ��ų ������ 0�̸� ȯ�� â ��Ȱ��ȭ
//                if (newLevel == 0)
//                {
//                    Transform skillWindow = skillLevelText.transform.parent.parent;
//                    Image skillRefund = skillWindow.transform.GetChild(0).GetChild(6).GetComponent<Image>();
//                    skillRefund.gameObject.SetActive(false);
//                }

//                // ��� ��ų ���� ������Ʈ
//                UpdateSkillPrices();

//                Debug.Log($"��ų {skillName} ȯ�ҿ� �����߽��ϴ�.");
//            }
//            else
//            {
//                Debug.Log($"��ų {skillName} ȯ�ҿ� �����߽��ϴ�.");
//            }
//        }
//        else
//        {
//            Debug.Log($"��ų {skillName}��(��) �� �̻� ȯ���� �� �����ϴ�.");
//        }
//    }

//    void UpdateSkillPrices()
//    {
//        foreach (var skill in skills)
//        {
//            if (skillPriceTexts.TryGetValue(skill.skillName, out Text skillPriceText))
//            {
//                if (currentMoney < skill.price)
//                {
//                    skillPriceText.color = Color.red;
//                }
//                else
//                {
//                    skillPriceText.color = PriceColor;
//                }
//            }
//        }
//    }
//}
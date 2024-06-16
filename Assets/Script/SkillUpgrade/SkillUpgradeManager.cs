using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillUpgradeManager : MonoBehaviour
{
    static DBConnectionInfo dBConnectionInfo = new DBConnectionInfo
    {
        ipAddress = "localhost",
        user = "root",
        password = "",
        dbName = "mydb"
    };

    [SerializeField] private DBManager dbManager = new DBManager(dBConnectionInfo);
    [SerializeField] private GameObject skillTextPrefab;
    [SerializeField] private GameObject skillButtonPrefab;
    [SerializeField] private Transform skillTextContainer;
    [SerializeField] private Transform skillButtonContainer;
    [SerializeField] private GameObject identityPrefab;
    [SerializeField] private Transform identityContainer;
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private Transform weaponContainer;
    [SerializeField] private Text playerMoney;

    private string currentCharactor = "Santa"; // ���� ĳ���� �̸����� ����
    private int playerId = 1; // ���� ����� ID�� ���� �������� ����
    private int currentMoney;
    private List<SkillDataStruct> skills;
    private SkillDataStruct identity;
    private List<WeaponDataStruct> weapons;
    private Dictionary<string, GameObject> skillInfoPanels = new Dictionary<string, GameObject>();
    private Dictionary<string, Text> skillPriceTexts = new Dictionary<string, Text>();
    private Color PriceColor = new Color32(104, 204, 128, 255); // #68CC80

    void Start()
    {
        LoadPlayerMoney();
        LoadSkills();
        LoadWeapon();
        LoadIdentitySkills();
    }

    void LoadPlayerMoney()
    {
        currentMoney = dbManager.GetMoney(currentCharactor, playerId);
        UpdatePlayerMoneyUI();
    }

    void UpdatePlayerMoneyUI()
    {
        playerMoney.text = currentMoney.ToString();
    }

    void LoadSkills()
    {
        skills = dbManager.GetSkillData();

        for (int i = 0; i < skills.Count; i++)
        {
            var skill = skills[i];

            GameObject skillText = Instantiate(skillTextPrefab, skillTextContainer);
            skillText.SetActive(false); // �ʱ⿡�� ��Ȱ��ȭ

            // �ؽ�Ʈ ��� ����
            skillText.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = skill.skillName;
            skillText.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = skill.skillInfo;
            skillText.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = skill.increase.ToString();
            skillText.transform.GetChild(0).GetChild(5).GetChild(0).GetComponent<Text>().text = skill.price.ToString();

            // ��ų ���� �г��� ��ųʸ��� ����
            skillInfoPanels[skill.skillName] = skillText;

            GameObject skillButton = Instantiate(skillButtonPrefab, skillButtonContainer);

            // ��ų ���� ��������
            int skillLevel = dbManager.GetSkillLevel(skill.skillName, currentCharactor, playerId).GetValueOrDefault();

            // ��ų ���� UI ����
            Text skillLevelText = skillText.transform.GetChild(0).GetChild(3).GetComponent<Text>();
            if (skillLevel > 0)
            {
                skillLevelText.text = $"level {skillLevel}";
                skillLevelText.gameObject.SetActive(true);
            }
            else
            {
                skillLevelText.gameObject.SetActive(false);
            }

            // ��ư ��� ���� �� ��ų �̸� ����
            Button upgradeButton = skillButton.transform.GetChild(0).GetComponent<Button>();
            Text skillPriceText = skillButton.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
            skillPriceText.text = skill.price.ToString();
            string skillName = skill.skillName; // �ݵ�� ���� ���� ��� -> ��...? (���� ĸó ���� ����)
            upgradeButton.onClick.AddListener(() => UpgradeSkill(skillName, skill.price, skillLevelText, skillPriceText));

            // ȯ�� �ȳ�â ����
            Image skillRefund = skillText.transform.GetChild(0).GetChild(6).GetComponent<Image>();
            skillRefund.gameObject.SetActive(skillLevel > 0);

            // �ʱ� ���� ���� ����
            skillPriceTexts[skill.skillName] = skillPriceText;
            if (currentMoney < skill.price)
            {
                skillPriceText.color = Color.red;
            }
            else
            {
                skillPriceText.color = PriceColor;
            }

            // �̹��� ����
            Image skillImage = skillButton.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            string imageSkillPath = $"Skills/Skill_{i + 1}"; // �̹��� �̸� Skill_1, Skill_2, ... ����

            Sprite skillSprite = Resources.Load<Sprite>(imageSkillPath);
            if (skillSprite != null)
            {
                skillImage.sprite = skillSprite;
            }
            else
            {
                Debug.LogError($"�̹����� �ε��� �� �����ϴ�: {imageSkillPath}");
            }

            // EventTrigger �߰�
            EventTrigger trigger = skillButton.AddComponent<EventTrigger>();

            // PointerEnter �̺�Ʈ �߰�
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((eventData) => { ShowSkillInfo(skillName); });
            entryEnter.callback.AddListener((eventData) => { StartButtonShake(skillButton); });
            trigger.triggers.Add(entryEnter);

            // PointerExit �̺�Ʈ �߰�
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((eventData) => { HideSkillInfo(skillName); });
            entryExit.callback.AddListener((eventData) => { StopButtonShake(skillButton); });
            trigger.triggers.Add(entryExit);

            // ��Ŭ�� �̺�Ʈ �߰�
            EventTrigger.Entry entryRightClick = new EventTrigger.Entry();
            entryRightClick.eventID = EventTriggerType.PointerClick;
            entryRightClick.callback.AddListener((eventData) =>
            {
                if (((PointerEventData)eventData).button == PointerEventData.InputButton.Right)
                {
                    RefundSkill(skillName, skill.price, skillLevelText, skillPriceText);
                }
            });
            trigger.triggers.Add(entryRightClick);
        }
    }

    void LoadWeapon()
    {
        weapons = dbManager.GetWeaponData(currentCharactor);

        for (int i = 0; i < weapons.Count; i++)
        {
            var weapon = weapons[i];

            GameObject weaponText = Instantiate(skillTextPrefab, skillTextContainer);
            weaponText.SetActive(false); // �ʱ⿡�� ��Ȱ��ȭ

            // �ؽ�Ʈ ��� ����
            weaponText.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = weapon.info;
            weaponText.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = "";
            weaponText.transform.GetChild(0).GetChild(5).GetChild(0).GetComponent<Text>().text = weapon.price.ToString();
            weaponText.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = weapon.name;
            weaponText.transform.GetChild(0).GetChild(4).gameObject.SetActive(false);

            // ���� ���� �г��� ��ųʸ��� ����
            skillInfoPanels[weapon.name] = weaponText;

            GameObject weaponButton = Instantiate(weaponPrefab, weaponContainer);

            Text weaponLevelText = weaponText.transform.GetChild(0).GetChild(3).GetComponent<Text>();
            weaponLevelText.gameObject.SetActive(false);

            // ���� ���� ����
            bool isBuy = dbManager.WeaponIsBuy(weapon.name, currentCharactor, playerId) > 0;

            // ���� ���Ž� UI Ȱ��ȭ
            if (isBuy)
            {
                // UI â �̹��� Ȱ��ȭ ��Ŵ
                // ���⿡ ���� �̹��� ��� ���� �� �����
                /*// �̹��� ����
                Image weaponImage = weaponButton.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
                string imageWeaponPath = $"Weapons/{weapon.name}";*/
                weaponButton.transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(true);
            }

            // ��ư ��� ���� �� ��ų �̸� ����
            Button upgradeButton = weaponButton.transform.GetChild(0).GetChild(0).GetComponent<Button>();
            Text weaponPriceText = weaponButton.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
            weaponPriceText.text = weapon.price.ToString();
            string weaponName = weapon.name;
            upgradeButton.onClick.AddListener(() => BuyWeapon(weaponName, weapon.price, isBuy, weaponPriceText));

            // ȯ�� �ȳ�â ����
            Image weaponRefund = weaponText.transform.GetChild(0).GetChild(6).GetComponent<Image>();
            weaponRefund.gameObject.SetActive(isBuy);

            // �ʱ� ���� ���� ����
            skillPriceTexts[weapon.name] = weaponPriceText;
            if (currentMoney < weapon.price)
            {
                weaponPriceText.color = Color.red;
            }
            else
            {
                weaponPriceText.color = PriceColor;
            }

            // �̹��� ����
            Image weaponImage = weaponButton.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
            string imageWeaponPath = $"Weapons/{weapon.name}";

            Sprite weaponSprite = Resources.Load<Sprite>(imageWeaponPath);
            if (weaponSprite != null)
            {
                weaponImage.sprite = weaponSprite;
            }
            else
            {
                Debug.LogError($"�̹����� �ε��� �� �����ϴ�: {imageWeaponPath}");
            }

            // EventTrigger �߰�
            EventTrigger trigger = weaponButton.AddComponent<EventTrigger>();

            // PointerEnter �̺�Ʈ �߰�
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((eventData) => { ShowSkillInfo(weaponName); });
            entryEnter.callback.AddListener((eventData) => { StartButtonShake(weaponButton); });
            trigger.triggers.Add(entryEnter);

            // PointerExit �̺�Ʈ �߰�
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((eventData) => { HideSkillInfo(weaponName); });
            entryExit.callback.AddListener((eventData) => { StopButtonShake(weaponButton); });
            trigger.triggers.Add(entryExit);

            // ��Ŭ�� �̺�Ʈ �߰�
            EventTrigger.Entry entryRightClick = new EventTrigger.Entry();
            entryRightClick.eventID = EventTriggerType.PointerClick;
            entryRightClick.callback.AddListener((eventData) =>
            {
                if (((PointerEventData)eventData).button == PointerEventData.InputButton.Right)
                {
                    RefundSkill(weaponName, weapon.price, null, weaponPriceText);
                }
            });
            trigger.triggers.Add(entryRightClick);
        }
    }

    void LoadIdentitySkills()
    {
        dbManager.GetIdentitySkillData(out identity, currentCharactor);

        GameObject identityText = Instantiate(skillTextPrefab, skillTextContainer);
        identityText.SetActive(false); // �ʱ⿡�� ��Ȱ��ȭ

        // �ؽ�Ʈ ��� ����
        identityText.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = identity.skillInfo;
        identityText.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = identity.increase.ToString();
        identityText.transform.GetChild(0).GetChild(5).GetChild(0).GetComponent<Text>().text = identity.price.ToString();
        identityText.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = identity.skillName;

        GameObject identityButton = Instantiate(identityPrefab, identityContainer);

        // ����ȭ ���� �г��� ��ųʸ��� ����
        skillInfoPanels[identity.skillName] = identityText;

        int identityLevel = dbManager.GetIdentitySkillLevel(currentCharactor, playerId).GetValueOrDefault();

        // ����ȭ ���� UI ����
        Text identityLevelText = identityText.transform.GetChild(0).GetChild(3).GetComponent<Text>();
        if (identityLevel > 0)
        {
            identityLevelText.text = $"level {identityLevel}";
            identityLevelText.gameObject.SetActive(true);
        }
        else
        {
            identityLevelText.gameObject.SetActive(false);
        }

        // ��ư ��� ���� �� ����ȭ �̸� ����
        Button upgradeButton = identityButton.transform.GetChild(0).GetChild(0).GetComponent<Button>();
        Text identityPriceText = identityButton.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
        identityPriceText.text = identity.price.ToString();
        string identityName = identity.skillName; // �ݵ�� ���� ���� ��� -> ��...? (���� ĸó ���� ����)
        upgradeButton.onClick.AddListener(() => UpgradeSkill(identityName, identity.price, identityLevelText, identityPriceText, true));

        // ȯ�� �ȳ�â ����
        Image identityRefund = identityText.transform.GetChild(0).GetChild(6).GetComponent<Image>();
        identityRefund.gameObject.SetActive(identityLevel > 0);

        // �ʱ� ���� ���� ����
        skillPriceTexts[identity.skillName] = identityPriceText;
        if (currentMoney < identity.price)
        {
            identityPriceText.color = Color.red;
        }
        else
        {
            identityPriceText.color = PriceColor;
        }

        // �̹��� ����
        Image identityImage = identityButton.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        string imageIdentityPath = $"Identites/Dash"; // ���� �ٸ� ĳ���Ͱ� �߰��ȴٸ� identites/{charactor}/identity -> �̷� ������ ��� �����ϸ� �ɵ�

        Sprite skillSprite = Resources.Load<Sprite>(imageIdentityPath);
        if (skillSprite != null)
        {
            identityImage.sprite = skillSprite;
        }
        else
        {
            Debug.LogError($"�̹����� �ε��� �� �����ϴ�: {imageIdentityPath}");
        }

        // EventTrigger �߰�
        EventTrigger trigger = identityButton.AddComponent<EventTrigger>();

        // PointerEnter �̺�Ʈ �߰�
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((eventData) => { ShowSkillInfo(identityName); });
        entryEnter.callback.AddListener((eventData) => { StartButtonShake(identityButton); });
        trigger.triggers.Add(entryEnter);

        // PointerExit �̺�Ʈ �߰�
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((eventData) => { HideSkillInfo(identityName); });
        entryExit.callback.AddListener((eventData) => { StopButtonShake(identityButton); });
        trigger.triggers.Add(entryExit);

        // ��Ŭ�� �̺�Ʈ �߰�
        EventTrigger.Entry entryRightClick = new EventTrigger.Entry();
        entryRightClick.eventID = EventTriggerType.PointerClick;
        entryRightClick.callback.AddListener((eventData) =>
        {
            if (((PointerEventData)eventData).button == PointerEventData.InputButton.Right)
            {
                RefundSkill(identityName, identity.price, identityLevelText, identityPriceText);
            }
        });
        trigger.triggers.Add(entryRightClick);
    }

    void ShowSkillInfo(string skillName)
    {
        if (skillInfoPanels.TryGetValue(skillName, out GameObject skillText))
        {
            skillText.SetActive(true);
        }
        else
        {
            Debug.LogError($"��ų ������ ã�� �� �����ϴ�: {skillName}");
        }
    }

    void HideSkillInfo(string skillName)
    {
        if (skillInfoPanels.TryGetValue(skillName, out GameObject skillText))
        {
            skillText.SetActive(false);
        }
        else
        {
            Debug.LogError($"��ų ������ ã�� �� �����ϴ�: {skillName}");
        }
    }

    void UpgradeSkill(string skillName, int skillPrice, Text skillLevelText, Text skillPriceText, bool isIdentity = false)
    {
        if (currentMoney >= skillPrice)
        {
            bool success = false;
            if (isIdentity)
            {
                success = dbManager.UpdateIdentitySkillLevelData(currentCharactor, 1, playerId);
            }
            else
            {
                success = dbManager.UpdateSkillLevelData(skillName, currentCharactor, 1, playerId);
            }

            if (success)
            {
                currentMoney -= skillPrice;

                // �����ͺ��̽��� ���� �� ������Ʈ
                dbManager.SetMoney(currentMoney, currentCharactor, playerId);

                UpdatePlayerMoneyUI();

                // ��ų ���� ������Ʈ
                int newLevel = isIdentity
                    ? dbManager.GetIdentitySkillLevel(currentCharactor, playerId).GetValueOrDefault()
                    : dbManager.GetSkillLevel(skillName, currentCharactor, playerId).GetValueOrDefault();

                skillLevelText.text = $"level {newLevel}";
                skillLevelText.gameObject.SetActive(newLevel > 0);

                // ���� ����� ���
                skillPriceText.color = PriceColor;

                // ȯ�� â Ȱ��ȭ
                Transform skillWindow = skillLevelText.transform.parent.parent;
                Image skillRefund = skillWindow.transform.GetChild(0).GetChild(6).GetComponent<Image>();
                skillRefund.gameObject.SetActive(true);

                // ��� ��ų ���� ������Ʈ
                UpdateSkillPrices();

                Debug.Log($"��ų {skillName} ���׷��̵忡 �����߽��ϴ�.");
            }
            else
            {
                Debug.Log($"��ų {skillName} ���׷��̵忡 �����߽��ϴ�.");
            }
        }
        else
        {
            // ���� ������ ���, �ؽ�Ʈ ���� ���������� ����
            skillPriceText.color = Color.red;
            Debug.Log("���� �����մϴ�.");
        }
    }

    void BuyWeapon(string weaponName, int weaponPrice, bool isBuy, Text weaponPriceText)
    {
        if (isBuy)
        {
            Debug.Log("�̹� ������");
            return;
        }

        if (currentMoney >= weaponPrice)
        {
            currentMoney -= weaponPrice;

            // �����ͺ��̽��� ���� �� ������Ʈ
            dbManager.SetMoney(currentMoney, currentCharactor, playerId);

            UpdatePlayerMoneyUI();

            // ���� ���� ó��
            bool success = dbManager.BuyWeapon(weaponName, currentCharactor, playerId);
            if (success)
            {
                isBuy = true;
                weaponPriceText.transform.parent.parent.GetChild(1).GetComponent<Image>().enabled = true;

                // ���� ����� ���
                weaponPriceText.color = PriceColor;

                // ��� ��ų ���� ������Ʈ
                UpdateSkillPrices();

                Debug.Log($"���� {weaponName} ���ſ� �����߽��ϴ�.");
            }
            else
            {
                Debug.LogError("���� ���ſ� �����߽��ϴ�.");
            }
        }
        else
        {
            // ���� ������ ���, �ؽ�Ʈ ���� ���������� ����
            weaponPriceText.color = Color.red;
            Debug.Log("���� �����մϴ�.");
        }
    }

    void RefundSkill(string skillName, int skillPrice, Text skillLevelText, Text skillPriceText)
    {
        int currentLevel = dbManager.GetSkillLevel(skillName, currentCharactor, playerId).GetValueOrDefault();
        if (currentLevel > 0)
        {
            bool success = dbManager.UpdateSkillLevelData(skillName, currentCharactor, -1, playerId);
            if (success)
            {
                currentMoney += skillPrice;

                // �����ͺ��̽��� ���� �� ������Ʈ
                dbManager.SetMoney(currentMoney, currentCharactor, playerId);

                UpdatePlayerMoneyUI();

                // ��ų ���� ������Ʈ
                int newLevel = dbManager.GetSkillLevel(skillName, currentCharactor, playerId).GetValueOrDefault();
                skillLevelText.text = $"level {newLevel}";
                skillLevelText.gameObject.SetActive(newLevel > 0);

                // ȯ�� �� ���� ����ϸ� ���� �ؽ�Ʈ ���� ����
                if (currentMoney >= skillPrice)
                {
                    skillPriceText.color = PriceColor;
                }

                // ��ų ������ 0�̸� ȯ�� â ��Ȱ��ȭ
                if (newLevel == 0)
                {
                    Transform skillWindow = skillLevelText.transform.parent.parent;
                    Image skillRefund = skillWindow.transform.GetChild(0).GetChild(6).GetComponent<Image>();
                    skillRefund.gameObject.SetActive(false);
                }

                // ��� ��ų ���� ������Ʈ
                UpdateSkillPrices();

                Debug.Log($"��ų {skillName} ȯ�ҿ� �����߽��ϴ�.");
            }
            else
            {
                Debug.Log($"��ų {skillName} ȯ�ҿ� �����߽��ϴ�.");
            }
        }
        else
        {
            Debug.Log($"��ų {skillName}��(��) �� �̻� ȯ���� �� �����ϴ�.");
        }
    }

    void UpdateSkillPrices()
    {
        foreach (var skill in skills)
        {
            if (skillPriceTexts.TryGetValue(skill.skillName, out Text skillPriceText))
            {
                if (currentMoney < skill.price)
                {
                    skillPriceText.color = Color.red;
                }
                else
                {
                    skillPriceText.color = PriceColor;
                }
            }
        }
    }

    private Coroutine buttonShakeCoroutine;


    void StartButtonShake(GameObject button)
    {
        if (buttonShakeCoroutine != null)
        {
            StopCoroutine(buttonShakeCoroutine);
        }
        buttonShakeCoroutine = StartCoroutine(ShakeButton(button));
    }
    void StopButtonShake(GameObject button)
    {
        if (buttonShakeCoroutine != null)
        {
            StopCoroutine(buttonShakeCoroutine);
            button.transform.rotation = Quaternion.identity;
        }
    }

    IEnumerator ShakeButton(GameObject button)
    {
        RectTransform rt = button.GetComponent<RectTransform>();
        float timeCount = 0;
        float angle;
        while (true)
        {
            angle = 5 * Mathf.Cos(timeCount * 10);
            rt.rotation = Quaternion.Euler(0, 0, angle);
            timeCount += Time.deltaTime;
            yield return null;
        }
    }

}
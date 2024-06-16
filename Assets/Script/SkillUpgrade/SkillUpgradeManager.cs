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

    private string currentCharactor = "Santa";
    private int playerId = 1;
    private int currentMoney;
    private List<SkillDataStruct> skills;
    private SkillDataStruct identity;
    private List<WeaponDataStruct> weapons;
    private Dictionary<string, GameObject> skillInfoPanels = new Dictionary<string, GameObject>();
    private Dictionary<string, Text> skillPriceTexts = new Dictionary<string, Text>();
    private Color PriceColor = new Color32(104, 204, 128, 255);

    void Start()
    {
        LoadPlayerMoney();
        LoadSkills();
        LoadWeapon();
        LoadIdentitySkills();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };

                List<RaycastResult> raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerEventData, raycastResults);

                foreach (RaycastResult result in raycastResults)
                {
                    if (result.gameObject.GetComponent<Button>() != null)
                    {
                        Button button = result.gameObject.GetComponent<Button>();
                        string buttonName = button.transform.parent.name;

                        if (buttonName.EndsWith("_Button"))
                        {
                            string itemName = buttonName.Replace("_Button", "");

                            if (skillInfoPanels.TryGetValue(itemName, out GameObject skillText))
                            {
                                Text skillPriceText = button.transform.GetChild(1).GetChild(0).GetComponent<Text>();
                                Text skillLevelText = skillText.transform.GetChild(0).GetChild(3).GetComponent<Text>();

                                if (itemName == identity.skillName)
                                {
                                    RefundIdentity(itemName, int.Parse(skillPriceText.text), skillLevelText, skillPriceText);
                                }
                                else if (dbManager.WeaponIsBuy(itemName, currentCharactor, playerId) > 0)
                                {
                                    RefundWeapon(itemName, int.Parse(skillPriceText.text), skillLevelText, skillPriceText);
                                }
                                else
                                {
                                    RefundSkill(itemName, int.Parse(skillPriceText.text), skillLevelText, skillPriceText);
                                }
                            }
                        }
                    }
                }
            }
        }
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
            skillText.SetActive(false);

            skillText.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = skill.skillName;
            skillText.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = skill.skillInfo;
            skillText.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = skill.increase.ToString();
            skillText.transform.GetChild(0).GetChild(5).GetChild(0).GetComponent<Text>().text = skill.price.ToString();

            skillInfoPanels[skill.skillName] = skillText;

            GameObject skillButton = Instantiate(skillButtonPrefab, skillButtonContainer);

            skillButton.name = skill.skillName + "_Button";

            int skillLevel = dbManager.GetSkillLevel(skill.skillName, currentCharactor, playerId).GetValueOrDefault();

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

            Button upgradeButton = skillButton.transform.GetChild(0).GetComponent<Button>();
            Text skillPriceText = skillButton.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
            skillPriceText.text = skill.price.ToString();
            string skillName = skill.skillName;
            upgradeButton.onClick.AddListener(() => UpgradeSkill(skillName, skill.price, skillLevelText, skillPriceText));

            Image skillRefund = skillText.transform.GetChild(0).GetChild(6).GetComponent<Image>();
            skillRefund.gameObject.SetActive(skillLevel > 0);

            skillPriceTexts[skill.skillName] = skillPriceText;
            if (currentMoney < skill.price)
            {
                skillPriceText.color = Color.red;
            }
            else
            {
                skillPriceText.color = PriceColor;
            }

            Image skillImage = skillButton.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            string imageSkillPath = $"Skills/Skill_{i + 1}";

            Sprite skillSprite = Resources.Load<Sprite>(imageSkillPath);
            if (skillSprite != null)
            {
                skillImage.sprite = skillSprite;
            }
            else
            {
                Debug.LogError($"이미지를 로드할 수 없습니다: {imageSkillPath}");
            }

            EventTrigger trigger = skillButton.AddComponent<EventTrigger>();

            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((eventData) => { ShowSkillInfo(skillName); });
            entryEnter.callback.AddListener((eventData) => { StartButtonShake(skillButton); });
            trigger.triggers.Add(entryEnter);

            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((eventData) => { HideSkillInfo(skillName); });
            entryExit.callback.AddListener((eventData) => { StopButtonShake(skillButton); });
            trigger.triggers.Add(entryExit);
        }
    }

    void LoadWeapon()
    {
        weapons = dbManager.GetWeaponData(currentCharactor);

        for (int i = 0; i < weapons.Count; i++)
        {
            var weapon = weapons[i];

            GameObject weaponText = Instantiate(skillTextPrefab, skillTextContainer);
            weaponText.SetActive(false);

            weaponText.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = weapon.info;
            weaponText.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = "";
            weaponText.transform.GetChild(0).GetChild(5).GetChild(0).GetComponent<Text>().text = weapon.price.ToString();
            weaponText.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = weapon.name;
            weaponText.transform.GetChild(0).GetChild(4).gameObject.SetActive(false);

            skillInfoPanels[weapon.name] = weaponText;

            GameObject weaponButton = Instantiate(weaponPrefab, weaponContainer);

            Text weaponLevelText = weaponText.transform.GetChild(0).GetChild(3).GetComponent<Text>();
            weaponLevelText.gameObject.SetActive(false);

            bool isBuy = dbManager.WeaponIsBuy(weapon.name, currentCharactor, playerId) > 0;

            if (isBuy)
            {
                weaponButton.transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(true);
                weaponLevelText.gameObject.SetActive(true);
                weaponLevelText.text = "구매됨";
            }

            Button upgradeButton = weaponButton.transform.GetChild(0).GetChild(0).GetComponent<Button>();
            Text weaponPriceText = weaponButton.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
            weaponPriceText.text = weapon.price.ToString();
            string weaponName = weapon.name;
            upgradeButton.onClick.AddListener(() => BuyWeapon(weaponName, weapon.price, isBuy, weaponPriceText));

            Image weaponRefund = weaponText.transform.GetChild(0).GetChild(6).GetComponent<Image>();
            weaponRefund.gameObject.SetActive(isBuy);

            skillPriceTexts[weapon.name] = weaponPriceText;
            if (currentMoney < weapon.price)
            {
                weaponPriceText.color = Color.red;
            }
            else
            {
                weaponPriceText.color = PriceColor;
            }

            Image weaponImage = weaponButton.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
            string imageWeaponPath = $"Weapons/{weapon.name}";

            Sprite weaponSprite = Resources.Load<Sprite>(imageWeaponPath);
            if (weaponSprite != null)
            {
                weaponImage.sprite = weaponSprite;
            }
            else
            {
                Debug.LogError($"이미지를 로드할 수 없습니다: {imageWeaponPath}");
            }

            EventTrigger trigger = weaponButton.AddComponent<EventTrigger>();

            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((eventData) => { ShowSkillInfo(weaponName); });
            entryEnter.callback.AddListener((eventData) => { StartButtonShake(weaponButton); });
            trigger.triggers.Add(entryEnter);

            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((eventData) => { HideSkillInfo(weaponName); });
            entryExit.callback.AddListener((eventData) => { StopButtonShake(weaponButton); });
            trigger.triggers.Add(entryExit);

            EventTrigger.Entry entryRightClick = new EventTrigger.Entry();
            entryRightClick.eventID = EventTriggerType.PointerClick;
            entryRightClick.callback.AddListener((eventData) =>
            {
                if (((PointerEventData)eventData).button == PointerEventData.InputButton.Right)
                {
                    RefundWeapon(weaponName, int.Parse(weaponPriceText.text), weaponLevelText, weaponPriceText);
                }
            });
            trigger.triggers.Add(entryRightClick);
        }
    }

    void LoadIdentitySkills()
    {
        dbManager.GetIdentitySkillData(out identity, currentCharactor);

        GameObject identityText = Instantiate(skillTextPrefab, skillTextContainer);
        identityText.SetActive(false);

        identityText.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = identity.skillInfo;
        identityText.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = identity.increase.ToString();
        identityText.transform.GetChild(0).GetChild(5).GetChild(0).GetComponent<Text>().text = identity.price.ToString();
        identityText.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = identity.skillName;

        GameObject identityButton = Instantiate(identityPrefab, identityContainer);

        skillInfoPanels[identity.skillName] = identityText;

        int identityLevel = dbManager.GetIdentitySkillLevel(currentCharactor, playerId).GetValueOrDefault();

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

        Button upgradeButton = identityButton.transform.GetChild(0).GetChild(0).GetComponent<Button>();
        Text identityPriceText = identityButton.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
        identityPriceText.text = identity.price.ToString();
        string identityName = identity.skillName;
        upgradeButton.onClick.AddListener(() => UpgradeSkill(identityName, identity.price, identityLevelText, identityPriceText, true));

        Image identityRefund = identityText.transform.GetChild(0).GetChild(6).GetComponent<Image>();
        identityRefund.gameObject.SetActive(identityLevel > 0);

        skillPriceTexts[identity.skillName] = identityPriceText;
        if (currentMoney < identity.price)
        {
            identityPriceText.color = Color.red;
        }
        else
        {
            identityPriceText.color = PriceColor;
        }

        Image identityImage = identityButton.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        string imageIdentityPath = $"Identites/Dash";

        Sprite skillSprite = Resources.Load<Sprite>(imageIdentityPath);
        if (skillSprite != null)
        {
            identityImage.sprite = skillSprite;
        }
        else
        {
            Debug.LogError($"이미지를 로드할 수 없습니다: {imageIdentityPath}");
        }

        EventTrigger trigger = identityButton.AddComponent<EventTrigger>();

        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((eventData) => { ShowSkillInfo(identityName); });
        entryEnter.callback.AddListener((eventData) => { StartButtonShake(identityButton); });
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((eventData) => { HideSkillInfo(identityName); });
        entryExit.callback.AddListener((eventData) => { StopButtonShake(identityButton); });
        trigger.triggers.Add(entryExit);

        EventTrigger.Entry entryRightClick = new EventTrigger.Entry();
        entryRightClick.eventID = EventTriggerType.PointerClick;
        entryRightClick.callback.AddListener((eventData) =>
        {
            if (((PointerEventData)eventData).button == PointerEventData.InputButton.Right)
            {
                RefundIdentity(identityName, int.Parse(identityPriceText.text), identityLevelText, identityPriceText);
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
            Debug.LogError($"스킬 정보를 찾을 수 없습니다: {skillName}");
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
            Debug.LogError($"스킬 정보를 찾을 수 없습니다: {skillName}");
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

                dbManager.SetMoney(currentMoney, currentCharactor, playerId);

                UpdatePlayerMoneyUI();

                int newLevel = isIdentity
                    ? dbManager.GetIdentitySkillLevel(currentCharactor, playerId).GetValueOrDefault()
                    : dbManager.GetSkillLevel(skillName, currentCharactor, playerId).GetValueOrDefault();

                skillLevelText.text = $"level {newLevel}";
                skillLevelText.gameObject.SetActive(newLevel > 0);

                skillPriceText.color = PriceColor;

                Transform skillWindow = skillLevelText.transform.parent.parent;
                Image skillRefund = skillWindow.transform.GetChild(0).GetChild(6).GetComponent<Image>();
                skillRefund.gameObject.SetActive(true);

                UpdateSkillPrices();

                if (skillInfoPanels.TryGetValue(skillName, out GameObject skillInfoPanel))
                {
                    StartButtonShake(skillInfoPanel);
                    StartCoroutine(StopButtonShakeAfterDelay(skillInfoPanel, 1f));
                }

                Debug.Log($"스킬 {skillName} 업그레이드에 성공했습니다.");
            }
            else
            {
                Debug.Log($"스킬 {skillName} 업그레이드에 실패했습니다.");
            }
        }
        else
        {
            skillPriceText.color = Color.red;
            Debug.Log("돈이 부족합니다.");
        }
    }

    IEnumerator StopButtonShakeAfterDelay(GameObject button, float delay)
    {
        yield return new WaitForSeconds(delay);
        StopButtonShake(button);
    }

    void BuyWeapon(string weaponName, int weaponPrice, bool isBuy, Text weaponPriceText)
    {
        if (isBuy)
        {
            Debug.Log("이미 구매함");
            return;
        }

        if (currentMoney >= weaponPrice)
        {
            currentMoney -= weaponPrice;

            dbManager.SetMoney(currentMoney, currentCharactor, playerId);

            UpdatePlayerMoneyUI();

            bool success = dbManager.BuyWeapon(weaponName, currentCharactor, playerId);
            if (success)
            {
                isBuy = true;
                weaponPriceText.transform.parent.parent.GetChild(1).GetComponent<Image>().enabled = true;

                weaponPriceText.color = PriceColor;

                UpdateSkillPrices();

                Debug.Log($"무기 {weaponName} 구매에 성공했습니다.");
            }
            else
            {
                Debug.LogError("무기 구매에 실패했습니다.");
            }
        }
        else
        {
            weaponPriceText.color = Color.red;
            Debug.Log("돈이 부족합니다.");
        }
    }

    void RefundSkill(string skillName, int price, Text levelText, Text priceText, bool isIdentity = false)
    {
        int currentLevel = dbManager.GetSkillLevel(skillName, currentCharactor, playerId).GetValueOrDefault();

        if (currentLevel > 0)
        {
            bool success = dbManager.UpdateSkillLevelData(skillName, currentCharactor, -1, playerId);

            if (success)
            {
                currentMoney += price;

                dbManager.SetMoney(currentMoney, currentCharactor, playerId);

                UpdatePlayerMoneyUI();

                int newLevel = dbManager.GetSkillLevel(skillName, currentCharactor, playerId).GetValueOrDefault();

                levelText.text = $"level {newLevel}";
                levelText.gameObject.SetActive(newLevel > 0);

                if (currentMoney >= price)
                {
                    priceText.color = PriceColor;
                }

                if (newLevel == 0)
                {
                    Transform skillWindow = levelText.transform.parent.parent;
                    Image refundImage = skillWindow.transform.GetChild(0).GetChild(6).GetComponent<Image>();
                    refundImage.gameObject.SetActive(false);
                }

                UpdateSkillPrices();

                Debug.Log($"스킬 환불에 성공했습니다: {skillName}");
            }
            else
            {
                Debug.Log($"스킬 환불에 실패했습니다: {skillName}");
            }
        }
        else
        {
            Debug.Log($"더 이상 환불할 수 없습니다: {skillName}");
        }
    }

    void RefundWeapon(string weaponName, int price, Text levelText, Text priceText)
    {
        int isBought = dbManager.WeaponIsBuy(weaponName, currentCharactor, playerId);

        if (isBought > 0)
        {
            bool success = dbManager.RefundWeapon(weaponName, currentCharactor, playerId);

            if (success)
            {
                currentMoney += price;

                dbManager.SetMoney(currentMoney, currentCharactor, playerId);

                UpdatePlayerMoneyUI();

                levelText.text = "";
                levelText.gameObject.SetActive(false);

                if (currentMoney >= price)
                {
                    priceText.color = PriceColor;
                }
                else
                {
                    priceText.color = Color.red;
                }

                Transform weaponWindow = levelText.transform.parent.parent;
                Image refundImage = weaponWindow.transform.GetChild(0).GetChild(6).GetComponent<Image>();
                refundImage.gameObject.SetActive(false);

                UpdateSkillPrices();

                Debug.Log($"무기 환불에 성공했습니다: {weaponName}");
            }
            else
            {
                Debug.Log($"무기 환불에 실패했습니다: {weaponName}");
            }
        }
        else
        {
            Debug.Log($"더 이상 환불할 수 없습니다: {weaponName}");
        }
    }

    void RefundIdentity(string identityName, int price, Text levelText, Text priceText)
    {
        int currentLevel = dbManager.GetIdentitySkillLevel(currentCharactor, playerId).GetValueOrDefault();

        if (currentLevel > 0)
        {
            bool success = dbManager.UpdateIdentitySkillLevelData(currentCharactor, -1, playerId);

            if (success)
            {
                currentMoney += price;

                dbManager.SetMoney(currentMoney, currentCharactor, playerId);

                UpdatePlayerMoneyUI();

                int newLevel = dbManager.GetIdentitySkillLevel(currentCharactor, playerId).GetValueOrDefault();

                levelText.text = $"level {newLevel}";
                levelText.gameObject.SetActive(newLevel > 0);

                if (currentMoney >= price)
                {
                    priceText.color = PriceColor;
                }
                else
                {
                    priceText.color = Color.red;
                }

                if (newLevel == 0)
                {
                    Transform skillWindow = levelText.transform.parent.parent;
                    Image refundImage = skillWindow.transform.GetChild(0).GetChild(6).GetComponent<Image>();
                    refundImage.gameObject.SetActive(false);
                }

                UpdateSkillPrices();

                Debug.Log($"전문화 환불에 성공했습니다: {identityName}");
            }
            else
            {
                Debug.Log($"전문화 환불에 실패했습니다: {identityName}");
            }
        }
        else
        {
            Debug.Log($"더 이상 환불할 수 없습니다: {identityName}");
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

using System.Collections;
using System.Collections.Generic;
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
        LoadPlayerMoney(); // 플레이어 돈 로드
        LoadSkills(); // 스킬 로드
        LoadWeapon(); // 무기 로드
        LoadIdentitySkills(); // 정체성 스킬 로드
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) // 마우스 오른쪽 버튼 클릭 감지
        {
            if (EventSystem.current.IsPointerOverGameObject()) // UI 객체 위에 있는지 확인
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
                            string parentName = button.transform.parent.parent.name;

                            if (skillInfoPanels.TryGetValue(itemName, out GameObject skillText))
                            {
                                Text priceText = button.transform.Find("BG/Price").GetComponent<Text>();
                                Text levelText = skillText.transform.GetChild(0).GetChild(3).GetComponent<Text>();

                                if (parentName == "Skill")
                                {
                                    RefundItem(itemName, int.Parse(priceText.text), levelText, priceText, ItemType.Skill); // 스킬 환불 처리
                                }
                                else if (parentName == "Weapon")
                                {
                                    RefundItem(itemName, int.Parse(priceText.text), levelText, priceText, ItemType.Weapon); // 무기 환불 처리
                                }
                                else if (parentName == "Identity")
                                {
                                    RefundItem(itemName, int.Parse(priceText.text), levelText, priceText, ItemType.Identity); // 정체성 스킬 환불 처리
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
        currentMoney = dbManager.GetMoney(currentCharactor, playerId); // DB에서 현재 돈 조회
        UpdatePlayerMoneyUI(); // UI 업데이트
    }

    void UpdatePlayerMoneyUI()
    {
        playerMoney.text = currentMoney.ToString(); // 현재 돈을 UI에 표시
    }

    void LoadSkills()
    {
        skills = dbManager.GetSkillData(); // DB에서 스킬 데이터 로드

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
            upgradeButton.onClick.AddListener(() => UpgradeSkill(skillName, skill.price, skillLevelText, skillPriceText)); // 스킬 업그레이드

            Image skillRefund = skillText.transform.GetChild(0).GetChild(6).GetComponent<Image>();
            skillRefund.gameObject.SetActive(skillLevel > 0);

            skillPriceTexts[skill.skillName] = skillPriceText;
            skillPriceText.color = currentMoney < skill.price ? Color.red : PriceColor;

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

    private void LoadWeapon()
    {
        weapons = dbManager.GetWeaponData(currentCharactor); // DB에서 무기 데이터 로드

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
            weaponButton.name = weapon.name + "_Button";

            Text weaponLevelText = weaponText.transform.GetChild(0).GetChild(3).GetComponent<Text>();
            weaponLevelText.gameObject.SetActive(false);

            bool isBuy = dbManager.WeaponIsBuy(weapon.name, currentCharactor, playerId) > 0;

            if (isBuy)
            {
                weaponButton.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                weaponLevelText.text = "구매됨";
                weaponLevelText.gameObject.SetActive(true);
            }
            else
            {
                weaponLevelText.gameObject.SetActive(false);
            }

            Button upgradeButton = weaponButton.transform.GetChild(0).GetComponent<Button>();
            Text weaponPriceText = weaponButton.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
            weaponPriceText.text = weapon.price.ToString();
            string weaponName = weapon.name;
            upgradeButton.onClick.AddListener(() => BuyWeapon(weaponName, weapon.price, weaponLevelText, weaponPriceText)); // 무기 구매

            Image weaponRefund = weaponText.transform.GetChild(0).GetChild(6).GetComponent<Image>();
            weaponRefund.gameObject.SetActive(isBuy);

            skillPriceTexts[weapon.name] = weaponPriceText;
            weaponPriceText.color = currentMoney < weapon.price ? Color.red : PriceColor;

            Image weaponImage = weaponButton.transform.GetChild(0).GetChild(0).GetComponent<Image>();
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
                    RefundItem(weaponName, int.Parse(weaponPriceText.text), weaponLevelText, weaponPriceText, ItemType.Weapon); // 무기 환불
                }
            });
            trigger.triggers.Add(entryRightClick);
        }
    }

    private void LoadIdentitySkills()
    {
        dbManager.GetIdentitySkillData(out identity, currentCharactor); // DB에서 정체성 스킬 데이터 로드

        GameObject identityText = Instantiate(skillTextPrefab, skillTextContainer);
        identityText.SetActive(false);

        identityText.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = identity.skillInfo;
        identityText.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = identity.increase.ToString();
        identityText.transform.GetChild(0).GetChild(5).GetChild(0).GetComponent<Text>().text = identity.price.ToString();
        identityText.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = identity.skillName;

        skillInfoPanels[identity.skillName] = identityText;

        GameObject identityButton = Instantiate(identityPrefab, identityContainer);
        identityButton.name = identity.skillName + "_Button";

        int identityLevel = dbManager.GetIdentitySkillLevel(currentCharactor, playerId).GetValueOrDefault();

        Text identityLevelText = identityText.transform.GetChild(0).GetChild(3).GetComponent<Text>();
        identityLevelText.text = identityLevel > 0 ? $"level {identityLevel}" : "";
        identityLevelText.gameObject.SetActive(identityLevel > 0);

        Button upgradeButton = identityButton.transform.GetChild(0).GetComponent<Button>();
        Text identityPriceText = identityButton.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
        identityPriceText.text = identity.price.ToString();
        string identityName = identity.skillName;
        upgradeButton.onClick.AddListener(() => UpgradeSkill(identityName, identity.price, identityLevelText, identityPriceText, true)); // 정체성 스킬 업그레이드

        Image identityRefund = identityText.transform.GetChild(0).GetChild(6).GetComponent<Image>();
        identityRefund.gameObject.SetActive(identityLevel > 0);

        skillPriceTexts[identity.skillName] = identityPriceText;
        identityPriceText.color = currentMoney < identity.price ? Color.red : PriceColor;

        Image identityImage = identityButton.transform.GetChild(0).GetChild(0).GetComponent<Image>();
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
                RefundItem(identityName, int.Parse(identityPriceText.text), identityLevelText, identityPriceText, ItemType.Identity); // 정체성 스킬 환불
            }
        });
        trigger.triggers.Add(entryRightClick);
    }

    void ShowSkillInfo(string skillName)
    {
        if (skillInfoPanels.TryGetValue(skillName, out GameObject skillText))
        {
            skillText.SetActive(true); // 스킬 정보 표시
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
            skillText.SetActive(false); // 스킬 정보 숨김
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
                success = dbManager.UpdateIdentitySkillLevelData(currentCharactor, 1, playerId); // 정체성 스킬 업그레이드
            }
            else
            {
                success = dbManager.UpdateSkillLevelData(skillName, currentCharactor, 1, playerId); // 스킬 업그레이드
            }

            if (success)
            {
                currentMoney -= skillPrice; // 돈 차감

                dbManager.SetMoney(currentMoney, currentCharactor, playerId); // DB에 돈 업데이트

                UpdatePlayerMoneyUI(); // UI 업데이트

                int newLevel = isIdentity
                    ? dbManager.GetIdentitySkillLevel(currentCharactor, playerId).GetValueOrDefault()
                    : dbManager.GetSkillLevel(skillName, currentCharactor, playerId).GetValueOrDefault();

                skillLevelText.text = $"level {newLevel}";
                skillLevelText.gameObject.SetActive(newLevel > 0);

                skillPriceText.color = PriceColor;

                Transform skillWindow = skillLevelText.transform.parent.parent;
                Image skillRefund = skillWindow.transform.GetChild(0).GetChild(6).GetComponent<Image>();
                skillRefund.gameObject.SetActive(true);

                // Dash 활성화
                if (isIdentity)
                {
                    Transform dashTransform = GameObject.Find("Inven/Dash/Dash").transform;
                    dashTransform.gameObject.SetActive(true);
                }

                UpdateSkillPrices(); // 스킬 가격 업데이트

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

    void BuyWeapon(string weaponName, int weaponPrice, Text weaponLevelText, Text weaponPriceText)
    {
        bool isBuy = dbManager.WeaponIsBuy(weaponName, currentCharactor, playerId) > 0;

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
                isBuy = true; // 구매 상태 업데이트
                weaponPriceText.transform.parent.parent.GetChild(1).gameObject.SetActive(true);
                weaponPriceText.color = PriceColor;

                // 구매 상태 업데이트
                GameObject weaponText;
                if (skillInfoPanels.TryGetValue(weaponName, out weaponText))
                {
                    weaponLevelText.text = "구매됨";
                    weaponLevelText.gameObject.SetActive(true);

                    Image weaponRefund = weaponText.transform.GetChild(0).GetChild(6).GetComponent<Image>();
                    weaponRefund.gameObject.SetActive(true);
                }

                // Gun 활성화
                Transform gunTransform = GameObject.Find("Inven/Gun/Gun").transform;
                gunTransform.gameObject.SetActive(true);

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

    enum ItemType { Skill, Weapon, Identity }

    void RefundItem(string itemName, int price, Text levelText, Text priceText, ItemType itemType)
    {
        bool success = false;
        int newLevel = 0;

        switch (itemType)
        {
            case ItemType.Skill:
                int skillLevel = dbManager.GetSkillLevel(itemName, currentCharactor, playerId).GetValueOrDefault();
                if (skillLevel > 0)
                {
                    success = dbManager.UpdateSkillLevelData(itemName, currentCharactor, -1, playerId);
                    newLevel = dbManager.GetSkillLevel(itemName, currentCharactor, playerId).GetValueOrDefault();
                }
                break;

            case ItemType.Weapon:
                bool isBought = dbManager.WeaponIsBuy(itemName, currentCharactor, playerId) > 0;
                if (isBought)
                {
                    success = dbManager.RefundWeapon(itemName, currentCharactor, playerId);
                    isBought = dbManager.WeaponIsBuy(itemName, currentCharactor, playerId) > 0;
                    if (!isBought)
                    {
                        // Gun 비활성화
                        Transform gunTransform = GameObject.Find("Inven/Gun/Gun").transform;
                        gunTransform.gameObject.SetActive(false);

                        // 환불 창 비활성화
                        GameObject weaponText;
                        if (skillInfoPanels.TryGetValue(itemName, out weaponText))
                        {
                            Image weaponRefund = weaponText.transform.GetChild(0).GetChild(6).GetComponent<Image>();
                            weaponRefund.gameObject.SetActive(false);

                            Text weaponLevelText = weaponText.transform.GetChild(0).GetChild(3).GetComponent<Text>();
                            weaponLevelText.text = "";
                            weaponLevelText.gameObject.SetActive(false);
                        }
                    }
                }
                break;

            case ItemType.Identity:
                int identityLevel = dbManager.GetIdentitySkillLevel(currentCharactor, playerId).GetValueOrDefault();
                if (identityLevel > 0)
                {
                    success = dbManager.UpdateIdentitySkillLevelData(currentCharactor, -1, playerId);
                    newLevel = dbManager.GetIdentitySkillLevel(currentCharactor, playerId).GetValueOrDefault();

                    // Dash 비활성화
                    Transform dashTransform = GameObject.Find("Inven/Dash/Dash").transform;
                    dashTransform.gameObject.SetActive(false);

                    // 환불 창 비활성화
                    GameObject identityText;
                    if (skillInfoPanels.TryGetValue(itemName, out identityText))
                    {
                        Image identityRefund = identityText.transform.GetChild(0).GetChild(6).GetComponent<Image>();
                        identityRefund.gameObject.SetActive(false);

                        Text weaponLevelText = identityText.transform.GetChild(0).GetChild(3).GetComponent<Text>();
                        weaponLevelText.text = "";
                        weaponLevelText.gameObject.SetActive(false);
                    }
                }
                break;
        }

        if (success)
        {
            currentMoney += price;
            dbManager.SetMoney(currentMoney, currentCharactor, playerId);
            UpdatePlayerMoneyUI();

            if (itemType == ItemType.Skill || itemType == ItemType.Identity)
            {
                levelText.text = $"level {newLevel}";
                levelText.gameObject.SetActive(newLevel > 0);
            }
            else
            {
                levelText.text = "";
                levelText.gameObject.SetActive(false);
            }

            priceText.color = currentMoney >= price ? PriceColor : Color.red;

            Transform itemWindow = levelText.transform.parent.parent;
            Image refundImage = itemWindow.transform.GetChild(0).GetChild(6).GetComponent<Image>();
            refundImage.gameObject.SetActive(newLevel > 0 || (itemType == ItemType.Weapon && dbManager.WeaponIsBuy(itemName, currentCharactor, playerId) > 0));

            UpdateSkillPrices();
            Debug.Log($"{itemType} 환불에 성공했습니다: {itemName}");
        }
        else
        {
            Debug.Log($"{itemType} 환불에 실패했습니다: {itemName}");
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

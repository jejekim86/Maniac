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

    private string currentCharactor = "Santa"; // 실제 캐릭터 이름으로 설정
    private int playerId = 1; // 실제 사용자 ID에 따라 동적으로 설정
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
            skillText.SetActive(false); // 초기에는 비활성화

            // 텍스트 요소 접근
            skillText.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = skill.skillName;
            skillText.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = skill.skillInfo;
            skillText.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = skill.increase.ToString();
            skillText.transform.GetChild(0).GetChild(5).GetChild(0).GetComponent<Text>().text = skill.price.ToString();

            // 스킬 정보 패널을 딕셔너리에 저장
            skillInfoPanels[skill.skillName] = skillText;

            GameObject skillButton = Instantiate(skillButtonPrefab, skillButtonContainer);

            // 스킬 레벨 가져오기
            int skillLevel = dbManager.GetSkillLevel(skill.skillName, currentCharactor, playerId).GetValueOrDefault();

            // 스킬 레벨 UI 설정
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

            // 버튼 요소 접근 및 스킬 이름 전달
            Button upgradeButton = skillButton.transform.GetChild(0).GetComponent<Button>();
            Text skillPriceText = skillButton.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
            skillPriceText.text = skill.price.ToString();
            string skillName = skill.skillName; // 반드시 로컬 변수 사용 -> 왜...? (참조 캡처 문제 방지)
            upgradeButton.onClick.AddListener(() => UpgradeSkill(skillName, skill.price, skillLevelText, skillPriceText));

            // 환불 안내창 설정
            Image skillRefund = skillText.transform.GetChild(0).GetChild(6).GetComponent<Image>();
            skillRefund.gameObject.SetActive(skillLevel > 0);

            // 초기 가격 색상 설정
            skillPriceTexts[skill.skillName] = skillPriceText;
            if (currentMoney < skill.price)
            {
                skillPriceText.color = Color.red;
            }
            else
            {
                skillPriceText.color = PriceColor;
            }

            // 이미지 설정
            Image skillImage = skillButton.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            string imageSkillPath = $"Skills/Skill_{i + 1}"; // 이미지 이름 Skill_1, Skill_2, ... 형태

            Sprite skillSprite = Resources.Load<Sprite>(imageSkillPath);
            if (skillSprite != null)
            {
                skillImage.sprite = skillSprite;
            }
            else
            {
                Debug.LogError($"이미지를 로드할 수 없습니다: {imageSkillPath}");
            }

            // EventTrigger 추가
            EventTrigger trigger = skillButton.AddComponent<EventTrigger>();

            // PointerEnter 이벤트 추가
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((eventData) => { ShowSkillInfo(skillName); });
            entryEnter.callback.AddListener((eventData) => { StartButtonShake(skillButton); });
            trigger.triggers.Add(entryEnter);

            // PointerExit 이벤트 추가
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((eventData) => { HideSkillInfo(skillName); });
            entryExit.callback.AddListener((eventData) => { StopButtonShake(skillButton); });
            trigger.triggers.Add(entryExit);

            // 우클릭 이벤트 추가
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
            weaponText.SetActive(false); // 초기에는 비활성화

            // 텍스트 요소 접근
            weaponText.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = weapon.info;
            weaponText.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = "";
            weaponText.transform.GetChild(0).GetChild(5).GetChild(0).GetComponent<Text>().text = weapon.price.ToString();
            weaponText.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = weapon.name;
            weaponText.transform.GetChild(0).GetChild(4).gameObject.SetActive(false);

            // 무기 정보 패널을 딕셔너리에 저장
            skillInfoPanels[weapon.name] = weaponText;

            GameObject weaponButton = Instantiate(weaponPrefab, weaponContainer);

            Text weaponLevelText = weaponText.transform.GetChild(0).GetChild(3).GetComponent<Text>();
            weaponLevelText.gameObject.SetActive(false);

            // 무기 구매 여부
            bool isBuy = dbManager.WeaponIsBuy(weapon.name, currentCharactor, playerId) > 0;

            // 무기 구매시 UI 활성화
            if (isBuy)
            {
                // UI 창 이미지 활성화 시킴
                // 여기에 넣을 이미지 경로 삽입 해 줘야함
                /*// 이미지 설정
                Image weaponImage = weaponButton.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
                string imageWeaponPath = $"Weapons/{weapon.name}";*/
                weaponButton.transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(true);
            }

            // 버튼 요소 접근 및 스킬 이름 전달
            Button upgradeButton = weaponButton.transform.GetChild(0).GetChild(0).GetComponent<Button>();
            Text weaponPriceText = weaponButton.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
            weaponPriceText.text = weapon.price.ToString();
            string weaponName = weapon.name;
            upgradeButton.onClick.AddListener(() => BuyWeapon(weaponName, weapon.price, isBuy, weaponPriceText));

            // 환불 안내창 설정
            Image weaponRefund = weaponText.transform.GetChild(0).GetChild(6).GetComponent<Image>();
            weaponRefund.gameObject.SetActive(isBuy);

            // 초기 가격 색상 설정
            skillPriceTexts[weapon.name] = weaponPriceText;
            if (currentMoney < weapon.price)
            {
                weaponPriceText.color = Color.red;
            }
            else
            {
                weaponPriceText.color = PriceColor;
            }

            // 이미지 설정
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

            // EventTrigger 추가
            EventTrigger trigger = weaponButton.AddComponent<EventTrigger>();

            // PointerEnter 이벤트 추가
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((eventData) => { ShowSkillInfo(weaponName); });
            entryEnter.callback.AddListener((eventData) => { StartButtonShake(weaponButton); });
            trigger.triggers.Add(entryEnter);

            // PointerExit 이벤트 추가
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((eventData) => { HideSkillInfo(weaponName); });
            entryExit.callback.AddListener((eventData) => { StopButtonShake(weaponButton); });
            trigger.triggers.Add(entryExit);

            // 우클릭 이벤트 추가
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
        identityText.SetActive(false); // 초기에는 비활성화

        // 텍스트 요소 접근
        identityText.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = identity.skillInfo;
        identityText.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = identity.increase.ToString();
        identityText.transform.GetChild(0).GetChild(5).GetChild(0).GetComponent<Text>().text = identity.price.ToString();
        identityText.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = identity.skillName;

        GameObject identityButton = Instantiate(identityPrefab, identityContainer);

        // 전문화 정보 패널을 딕셔너리에 저장
        skillInfoPanels[identity.skillName] = identityText;

        int identityLevel = dbManager.GetIdentitySkillLevel(currentCharactor, playerId).GetValueOrDefault();

        // 전문화 레벨 UI 설정
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

        // 버튼 요소 접근 및 전문화 이름 전달
        Button upgradeButton = identityButton.transform.GetChild(0).GetChild(0).GetComponent<Button>();
        Text identityPriceText = identityButton.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
        identityPriceText.text = identity.price.ToString();
        string identityName = identity.skillName; // 반드시 로컬 변수 사용 -> 왜...? (참조 캡처 문제 방지)
        upgradeButton.onClick.AddListener(() => UpgradeSkill(identityName, identity.price, identityLevelText, identityPriceText, true));

        // 환불 안내창 설정
        Image identityRefund = identityText.transform.GetChild(0).GetChild(6).GetComponent<Image>();
        identityRefund.gameObject.SetActive(identityLevel > 0);

        // 초기 가격 색상 설정
        skillPriceTexts[identity.skillName] = identityPriceText;
        if (currentMoney < identity.price)
        {
            identityPriceText.color = Color.red;
        }
        else
        {
            identityPriceText.color = PriceColor;
        }

        // 이미지 설정
        Image identityImage = identityButton.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        string imageIdentityPath = $"Identites/Dash"; // 추후 다른 캐릭터가 추가된다면 identites/{charactor}/identity -> 이런 식으로 경로 수정하면 될듯

        Sprite skillSprite = Resources.Load<Sprite>(imageIdentityPath);
        if (skillSprite != null)
        {
            identityImage.sprite = skillSprite;
        }
        else
        {
            Debug.LogError($"이미지를 로드할 수 없습니다: {imageIdentityPath}");
        }

        // EventTrigger 추가
        EventTrigger trigger = identityButton.AddComponent<EventTrigger>();

        // PointerEnter 이벤트 추가
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((eventData) => { ShowSkillInfo(identityName); });
        entryEnter.callback.AddListener((eventData) => { StartButtonShake(identityButton); });
        trigger.triggers.Add(entryEnter);

        // PointerExit 이벤트 추가
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((eventData) => { HideSkillInfo(identityName); });
        entryExit.callback.AddListener((eventData) => { StopButtonShake(identityButton); });
        trigger.triggers.Add(entryExit);

        // 우클릭 이벤트 추가
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

                // 데이터베이스에 남은 돈 업데이트
                dbManager.SetMoney(currentMoney, currentCharactor, playerId);

                UpdatePlayerMoneyUI();

                // 스킬 레벨 업데이트
                int newLevel = isIdentity
                    ? dbManager.GetIdentitySkillLevel(currentCharactor, playerId).GetValueOrDefault()
                    : dbManager.GetSkillLevel(skillName, currentCharactor, playerId).GetValueOrDefault();

                skillLevelText.text = $"level {newLevel}";
                skillLevelText.gameObject.SetActive(newLevel > 0);

                // 돈이 충분한 경우
                skillPriceText.color = PriceColor;

                // 환불 창 활성화
                Transform skillWindow = skillLevelText.transform.parent.parent;
                Image skillRefund = skillWindow.transform.GetChild(0).GetChild(6).GetComponent<Image>();
                skillRefund.gameObject.SetActive(true);

                // 모든 스킬 가격 업데이트
                UpdateSkillPrices();

                Debug.Log($"스킬 {skillName} 업그레이드에 성공했습니다.");
            }
            else
            {
                Debug.Log($"스킬 {skillName} 업그레이드에 실패했습니다.");
            }
        }
        else
        {
            // 돈이 부족한 경우, 텍스트 색상 빨간색으로 변경
            skillPriceText.color = Color.red;
            Debug.Log("돈이 부족합니다.");
        }
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

            // 데이터베이스에 남은 돈 업데이트
            dbManager.SetMoney(currentMoney, currentCharactor, playerId);

            UpdatePlayerMoneyUI();

            // 무기 구매 처리
            bool success = dbManager.BuyWeapon(weaponName, currentCharactor, playerId);
            if (success)
            {
                isBuy = true;
                weaponPriceText.transform.parent.parent.GetChild(1).GetComponent<Image>().enabled = true;

                // 돈이 충분한 경우
                weaponPriceText.color = PriceColor;

                // 모든 스킬 가격 업데이트
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
            // 돈이 부족한 경우, 텍스트 색상 빨간색으로 변경
            weaponPriceText.color = Color.red;
            Debug.Log("돈이 부족합니다.");
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

                // 데이터베이스에 남은 돈 업데이트
                dbManager.SetMoney(currentMoney, currentCharactor, playerId);

                UpdatePlayerMoneyUI();

                // 스킬 레벨 업데이트
                int newLevel = dbManager.GetSkillLevel(skillName, currentCharactor, playerId).GetValueOrDefault();
                skillLevelText.text = $"level {newLevel}";
                skillLevelText.gameObject.SetActive(newLevel > 0);

                // 환불 후 돈이 충분하면 가격 텍스트 색상 변경
                if (currentMoney >= skillPrice)
                {
                    skillPriceText.color = PriceColor;
                }

                // 스킬 레벨이 0이면 환불 창 비활성화
                if (newLevel == 0)
                {
                    Transform skillWindow = skillLevelText.transform.parent.parent;
                    Image skillRefund = skillWindow.transform.GetChild(0).GetChild(6).GetComponent<Image>();
                    skillRefund.gameObject.SetActive(false);
                }

                // 모든 스킬 가격 업데이트
                UpdateSkillPrices();

                Debug.Log($"스킬 {skillName} 환불에 성공했습니다.");
            }
            else
            {
                Debug.Log($"스킬 {skillName} 환불에 실패했습니다.");
            }
        }
        else
        {
            Debug.Log($"스킬 {skillName}은(는) 더 이상 환불할 수 없습니다.");
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
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

//    private int playerId = 1; // 실제 사용자 ID에 따라 동적으로 설정
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
//            Debug.LogError($"이미지를 로드할 수 없습니다: {imageDashPath}");
//        }

//        for (int i = 0; i < skills.Count; i++)
//        {
//            var skill = skills[i];

//            GameObject skillText = Instantiate(skillTextPrefab, skillTextContainer);
//            skillText.SetActive(false); // 초기에는 비활성화

//            // 텍스트 요소 접근
//            skillText.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = skill.skillName;
//            skillText.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = skill.skillInfo;
//            skillText.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = skill.increase.ToString();
//            skillText.transform.GetChild(0).GetChild(5).GetChild(0).GetComponent<Text>().text = skill.price.ToString();

//            // 스킬 정보 패널을 딕셔너리에 저장
//            skillInfoPanels[skill.skillName] = skillText;

//            GameObject skillButton = Instantiate(skillButtonPrefab, skillButtonContainer);

//            // 스킬 레벨 가져오기
//            int skillLevel = dbManager.GetSkillLevel(skill.skillName, playerId).GetValueOrDefault();

//            // 스킬 레벨 UI 설정
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

//            // 버튼 요소 접근 및 스킬 이름 전달
//            Button upgradeButton = skillButton.transform.GetChild(0).GetComponent<Button>();
//            Text skillPriceText = skillButton.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
//            skillPriceText.text = skill.price.ToString();
//            string skillName = skill.skillName; // 반드시 로컬 변수 사용 -> 왜...? (참조 캡처 문제 방지)
//            upgradeButton.onClick.AddListener(() => UpgradeSkill(skillName, skill.price, skillLevelText, skillPriceText));

//            // 환불 안내창 설정
//            Image skillRefund = skillText.transform.GetChild(0).GetChild(6).GetComponent<Image>();
//            skillRefund.gameObject.SetActive(skillLevel > 0);

//            // 초기 가격 색상 설정
//            skillPriceTexts[skill.skillName] = skillPriceText;
//            if (currentMoney < skill.price)
//            {
//                skillPriceText.color = Color.red;
//            }
//            else
//            {
//                skillPriceText.color = PriceColor;
//            }

//            // 이미지 설정
//            Image skillImage = skillButton.transform.GetChild(0).GetChild(0).GetComponent<Image>();
//            string imageSkillPath = $"Skills/Skill_{i + 1}"; // 이미지 이름 Skill_1, Skill_2, ... 형태

//            Sprite skillSprite = Resources.Load<Sprite>(imageSkillPath);
//            if (skillSprite != null)
//            {
//                skillImage.sprite = skillSprite;
//            }
//            else
//            {
//                Debug.LogError($"이미지를 로드할 수 없습니다: {imageSkillPath}");
//            }

//            // EventTrigger 추가
//            EventTrigger trigger = skillButton.AddComponent<EventTrigger>();

//            // PointerEnter 이벤트 추가
//            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
//            entryEnter.eventID = EventTriggerType.PointerEnter;
//            entryEnter.callback.AddListener((eventData) => { ShowSkillInfo(skillName); });
//            trigger.triggers.Add(entryEnter);

//            // PointerExit 이벤트 추가
//            EventTrigger.Entry entryExit = new EventTrigger.Entry();
//            entryExit.eventID = EventTriggerType.PointerExit;
//            entryExit.callback.AddListener((eventData) => { HideSkillInfo(skillName); });
//            trigger.triggers.Add(entryExit);

//            // 우클릭 이벤트 추가
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
//            Debug.LogError($"스킬 정보를 찾을 수 없습니다: {skillName}");
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
//            Debug.LogError($"스킬 정보를 찾을 수 없습니다: {skillName}");
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

//                // 데이터베이스에 남은 돈 업데이트
//                dbManager.SetMoney(currentMoney, playerId);

//                UpdatePlayerMoneyUI();

//                // 스킬 레벨 업데이트
//                int newLevel = dbManager.GetSkillLevel(skillName, playerId).GetValueOrDefault();
//                skillLevelText.text = $"level {newLevel}";
//                skillLevelText.gameObject.SetActive(newLevel > 0);

//                // 돈이 충분한 경우
//                skillPriceText.color = PriceColor;

//                // 환불 창 활성화
//                Transform skillWindow = skillLevelText.transform.parent.parent;
//                Image skillRefund = skillWindow.transform.GetChild(0).GetChild(6).GetComponent<Image>();
//                skillRefund.gameObject.SetActive(true);

//                // 모든 스킬 가격 업데이트
//                UpdateSkillPrices();

//                Debug.Log($"스킬 {skillName} 업그레이드에 성공했습니다.");
//            }
//            else
//            {
//                Debug.Log($"스킬 {skillName} 업그레이드에 실패했습니다.");
//            }
//        }
//        else
//        {
//            // 돈이 부족한 경우, 텍스트 색상 빨간색으로 변경
//            skillPriceText.color = Color.red;
//            Debug.Log("돈이 부족합니다.");
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

//                // 데이터베이스에 남은 돈 업데이트
//                dbManager.SetMoney(currentMoney, playerId);

//                UpdatePlayerMoneyUI();

//                // 스킬 레벨 업데이트
//                int newLevel = dbManager.GetSkillLevel(skillName, playerId).GetValueOrDefault();
//                skillLevelText.text = $"level {newLevel}";
//                skillLevelText.gameObject.SetActive(newLevel > 0);

//                // 환불 후 돈이 충분하면 가격 텍스트 색상 변경
//                if (currentMoney >= skillPrice)
//                {
//                    skillPriceText.color = PriceColor;
//                }

//                // 스킬 레벨이 0이면 환불 창 비활성화
//                if (newLevel == 0)
//                {
//                    Transform skillWindow = skillLevelText.transform.parent.parent;
//                    Image skillRefund = skillWindow.transform.GetChild(0).GetChild(6).GetComponent<Image>();
//                    skillRefund.gameObject.SetActive(false);
//                }

//                // 모든 스킬 가격 업데이트
//                UpdateSkillPrices();

//                Debug.Log($"스킬 {skillName} 환불에 성공했습니다.");
//            }
//            else
//            {
//                Debug.Log($"스킬 {skillName} 환불에 실패했습니다.");
//            }
//        }
//        else
//        {
//            Debug.Log($"스킬 {skillName}은(는) 더 이상 환불할 수 없습니다.");
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
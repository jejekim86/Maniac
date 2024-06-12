using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    [SerializeField] private DBManager dbManager = new DBManager();
    [SerializeField] private GameObject skillTextPrefab;
    [SerializeField] private GameObject skillButtonPrefab;
    [SerializeField] private Transform skillTextContainer;
    [SerializeField] private Transform skillButtonContainer;

    private List<SkillDataStruct> skills;
    private Dictionary<string, GameObject> skillInfoPanels = new Dictionary<string, GameObject>();

    void Start()
    {
        LoadSkills();
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

            // 스킬 정보 패널을 딕셔너리에 저장
            skillInfoPanels[skill.skillName] = skillText;

            GameObject skillButton = Instantiate(skillButtonPrefab, skillButtonContainer);

            // 버튼 요소 접근 및 스킬 이름 전달
            Button upgradeButton = skillButton.transform.GetChild(0).GetComponent<Button>();
            string skillName = skill.skillName; // 반드시 로컬 변수 사용 -> 왜...?
            upgradeButton.onClick.AddListener(() => UpgradeSkill(skillName));

            // 이미지 설정
            Image skillImage = skillButton.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            string imagePath = $"Skills/Skill_{i + 1}"; // 이미지 이름 Skill_1, Skill_2, ... 형태

            Sprite skillSprite = Resources.Load<Sprite>(imagePath);
            if (skillSprite != null)
            {
                skillImage.sprite = skillSprite;
            }
            else
            {
                Debug.LogError($"이미지를 로드할 수 없습니다: {imagePath}");
            }

            // EventTrigger 추가
            EventTrigger trigger = skillButton.AddComponent<EventTrigger>();

            // PointerEnter 이벤트 추가
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((eventData) => { ShowSkillInfo(skillName); });
            trigger.triggers.Add(entryEnter);

            // PointerExit 이벤트 추가
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((eventData) => { HideSkillInfo(skillName); });
            trigger.triggers.Add(entryExit);
        }
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

    void UpgradeSkill(string skillName)
    {
        int userId = 1; // 실제 사용자 ID에 따라 동적으로 설정되어야 합니다
        bool success = dbManager.UpdateSkillLevelData(skillName, 1, userId);

        if (success)
        {
            Debug.Log("스킬 업그레이드에 성공했습니다.");
        }
        else
        {
            Debug.LogError("스킬 업그레이드에 실패했습니다.");
        }
    }
}

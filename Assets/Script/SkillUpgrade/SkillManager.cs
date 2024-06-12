using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public List<SkillData> skills;
    public Transform skillListParent;
    public GameObject skillItemPrefab;

    private int playerMoney = 1000; // 초기 자금

    void Start()
    {
        DisplaySkillsInShop();
    }

    void DisplaySkillsInShop()
    {
        foreach (SkillData skill in skills)
        {
            GameObject skillItem = Instantiate(skillItemPrefab, skillListParent);
            skillItem.transform.Find("Name").GetComponent<UnityEngine.UI.Text>().text = skill.skillName;
            skillItem.transform.Find("Description").GetComponent<UnityEngine.UI.Text>().text = skill.description;
            skillItem.transform.Find("Price").GetComponent<UnityEngine.UI.Text>().text = skill.price.ToString();
            skillItem.transform.Find("UpgradeLevel").GetComponent<UnityEngine.UI.Text>().text = "Level: " + skill.upgradeLevel;
            skillItem.transform.Find("BuyButton").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => BuySkill(skill, skillItem));
        }
    }

    void BuySkill(SkillData skill, GameObject skillItem)
    {
        if (playerMoney >= skill.price)
        {
            playerMoney -= skill.price;
            skill.upgradeLevel++;
            skill.price += 100; // 업그레이드 할 때마다 가격 증가

            skillItem.transform.Find("Price").GetComponent<UnityEngine.UI.Text>().text = skill.price.ToString();
            skillItem.transform.Find("UpgradeLevel").GetComponent<UnityEngine.UI.Text>().text = "Level: " + skill.upgradeLevel;

            Debug.Log("Purchased Skill: " + skill.skillName + ", Level: " + skill.upgradeLevel);
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }
}

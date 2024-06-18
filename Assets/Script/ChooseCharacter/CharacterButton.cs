using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Text characterName;
    [SerializeField] Image characterImage;
    [SerializeField] RectTransform myRectTransform;

    private Text characterInfoText;
    private Text characterUpgradeInfoText;
    private Coroutine onMouserCoroutine;

    private int money;
    private CharactorData characterData;
    private Image upgradeInfoImage;
    private Image characterInfoImage;

    public void OnPointerEnter(PointerEventData eventData)
    {
        characterInfoText.text = $"체력       {characterData.health}\n속도      {characterData.moveSpeed}";
        characterUpgradeInfoText.text = money.ToString();
        onMouserCoroutine = StartCoroutine(MouseOnEvent1());
        StartCoroutine(MouseOnEvent2());
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        StopCoroutine(onMouserCoroutine);
        myRectTransform.rotation = Quaternion.identity;
    }

    public void SetData(CharactorData data, Sprite sprite, Text info, Text upgradeInfo)
    {
        characterData = data;
        characterImage.sprite = sprite;
        characterInfoText = info;
        characterInfoImage = characterInfoText.GetComponentInParent<Image>();
        characterUpgradeInfoText = upgradeInfo;
        upgradeInfoImage = characterUpgradeInfoText.GetComponentInParent<Image>();
        characterName.text = characterData.name;
        money = DBManagerTest.instance.GetMoney(characterData.name);
    }

    public void OnClickChooseCharacter()
    {
        // 해당 케릭터 데이터 넘겨주기

        // 씬 전환
        SceneManager.LoadScene("SkillUpgrade", LoadSceneMode.Single);
    }
    IEnumerator MouseOnEvent1()
    {
        float timeCount = 0;
        float angle;
        while (true)
        {
            angle = 5 * Mathf.Cos(timeCount * 10);
            myRectTransform.rotation = Quaternion.Euler(0, 0, angle);
            timeCount += Time.deltaTime;
            yield return null;
        }
    }   
    IEnumerator MouseOnEvent2()
    {
        float randomAngle = Random.Range(-5f, 5f);
        characterInfoImage.transform.rotation = Quaternion.Euler(0, 0, randomAngle);
        randomAngle = Random.Range(-5f, 5f);
        upgradeInfoImage.transform.rotation = Quaternion.Euler(0, 0, randomAngle);
        Vector3 startPosCharacterInfo = characterInfoImage.transform.position;
        Vector3 startPosUpgradeInfo = upgradeInfoImage.transform.position;
        characterInfoImage.transform.position = startPosCharacterInfo + Vector3.up * 20;
        upgradeInfoImage.transform.position = startPosUpgradeInfo + Vector3.up * 20;
        float timeCount = 0;
        while(timeCount < 0.5)
        {
            characterInfoImage.transform.position = Vector3.Lerp(startPosCharacterInfo + Vector3.up * 20, startPosCharacterInfo, timeCount * 2);
            upgradeInfoImage.transform.position = Vector3.Lerp(startPosUpgradeInfo + Vector3.up * 20, startPosUpgradeInfo, timeCount * 2);
            timeCount += Time.deltaTime;
            yield return null;
        }
        characterInfoImage.transform.position = startPosCharacterInfo;
        upgradeInfoImage.transform.position = startPosUpgradeInfo;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum characters
{
    Santa,
    max
}
public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Image[] backGroundImage;
    [SerializeField] private Transform[] endPos;
    [SerializeField] private Image recordFrame;
    [SerializeField] private Text[] records;
    [SerializeField] private Image characterImage;
    [SerializeField] private Image bustedImage;

    [SerializeField] private Sprite[] characterBustedImages;
    [SerializeField] private Sprite[] characterClearImages;


    private bool isClear = false;
    private HighScore thisGameScore;
    private HighScore highScore;

    void Start()
    {
        // 이번판 게임 기록 가져오기

        // db에서 값 받아오기
        DBManagerTest.instance.GetRecordHighScore(out highScore, "Santa"); // Santa에서 thisGameScore.characterName으로 변경 하기
        for(int i = 0; i < (int)characters.max; i++)
        {
            if(Enum.GetName(typeof(characters), i) == "Santa") // Santa에서 thisGameScore.characterName으로 변경 하기
            {
                if (!isClear)
                    characterImage.sprite = characterBustedImages[i];
                else
                    characterImage.sprite = characterClearImages[i];
                break;
            }
        }
        StartCoroutine(MoveBG());
    }

    public void SetGameScore(HighScore score, bool isClear)
    {
        thisGameScore = score;
        this.isClear = isClear;
    }

    IEnumerator MoveBG()
    {
        float timeCount = 0;
        Vector3[] startPos = new Vector3[backGroundImage.Length];
        for (int i = 0; i < backGroundImage.Length; i++)
        {
            startPos[i] = backGroundImage[i].transform.position;
        }
        while (timeCount < 0.5f)
        {
            timeCount += Time.deltaTime;
            for (int i = 0; i < backGroundImage.Length; i++)
            {
                backGroundImage[i].transform.position = Vector3.Lerp(startPos[i], endPos[i].position, timeCount * 2);
            }
            yield return null;
        }
        StartCoroutine(ShowRecord());
    }

    IEnumerator ShowRecord()
    {
        recordFrame.enabled = true;

        yield return new WaitForSeconds(0.2f);

        records[0].text = $"추구 수준           {thisGameScore.stars}       {highScore.stars}";
        records[1].text = $"생존 시간           {thisGameScore.lifeTime}       {highScore.lifeTime}";
        records[2].text = $"모은 돈           {thisGameScore.money}       {highScore.money}";
        for (int i = 0; i < records.Length; i++)
        {
            records[i].enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.1f);
        characterImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        if(!isClear)
            bustedImage.enabled = true;

        DBManagerTest.instance.SetRecordHighScore(thisGameScore);
    }
}

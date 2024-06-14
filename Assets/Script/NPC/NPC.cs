
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class NPC : MonoBehaviour
{
    [SerializeField] private NPCRandomSpawner spawner;
    [SerializeField] private GameObject upgaradePanel;
    private bool gamePaused = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            // 상점 코드
            upgaradePanel.SetActive(true);

            // 게임 멈추기
            PauseGame();

            // NPC를 다시 풀에 집어넣음
            spawner.objectPool.PutInPool(this);

            // 새로운 NPC 생성 요청
            spawner.SpawnRandomObject();
        }
    }

    private void PauseGame()
    {
        GameManager.Instance.inGame = false;
        // 게임 일시 정지
        //Time.timeScale = 0f;
    }
}
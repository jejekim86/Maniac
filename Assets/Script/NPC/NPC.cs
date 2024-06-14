
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

            // ���� �ڵ�
            upgaradePanel.SetActive(true);

            // ���� ���߱�
            PauseGame();

            // NPC�� �ٽ� Ǯ�� �������
            spawner.objectPool.PutInPool(this);

            // ���ο� NPC ���� ��û
            spawner.SpawnRandomObject();
        }
    }

    private void PauseGame()
    {
        GameManager.Instance.inGame = false;
        // ���� �Ͻ� ����
        //Time.timeScale = 0f;
    }
}
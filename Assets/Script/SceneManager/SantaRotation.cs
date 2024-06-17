using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SantaRotation : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] private float rotationSpeed = 50f; // ȸ�� �ӵ�

    void Update()
    {
        // y���� �߽����� ȸ��
        player.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
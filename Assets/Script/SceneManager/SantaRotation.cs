using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SantaRotation : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] private float rotationSpeed = 50f; // 회전 속도

    void Update()
    {
        // y축을 중심으로 회전
        player.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
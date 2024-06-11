using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    [SerializeField] private Button startButton;

    private void Start()
    {
        startButton.onClick.AddListener(ButtonOnClick);
    }

    private void ButtonOnClick()
    {
        SceneManager.LoadScene("SkillUpgrade", LoadSceneMode.Single);
    }
}

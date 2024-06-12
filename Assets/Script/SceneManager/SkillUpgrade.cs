using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SkillUpgrade : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button checkButton;

    private void Start()
    {
        backButton.onClick.AddListener(BackButtonOnClick);
        checkButton.onClick.AddListener(CheckButtonOnClick);
    }

    private void BackButtonOnClick()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    
    private void CheckButtonOnClick()
    {
        SceneManager.LoadScene("InGame", LoadSceneMode.Single);
    }
}

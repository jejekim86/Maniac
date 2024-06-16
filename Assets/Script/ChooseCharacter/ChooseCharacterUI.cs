using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseCharacterUI : MonoBehaviour
{
    [SerializeField] Text characterInfoText;
    [SerializeField] Text characterUpgradeInfoText;
    [SerializeField] Transform characterPanel;
    [SerializeField] GameObject characterButton;
    [SerializeField] Sprite[] characterImages;
    private List<CharactorData> characters;

    void Start()
    {
        characters = DBManagerTest.instance.GetCharactorData();
        for(int i = 0; i < characters.Count; i++)
        {
            GameObject button = Instantiate(characterButton, characterPanel);
            button.GetComponent<CharacterButton>().SetData(characters[i], characterImages[i], characterInfoText, characterUpgradeInfoText);
        }
    }
}

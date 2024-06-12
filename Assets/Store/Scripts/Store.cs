using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using System.ComponentModel;
#if UNITY_EDITOR
using UnityEditor;
#endif
#region Store
public class Store : MonoBehaviour
{
    #region Variables
    //////////////////
    public GameObject player;
    [Header("Button")]
    Sprite
        coinsImage;
    [HideInInspector]
    public
    Image[] buttonBackground;
    [SerializeField]
    Color
        unlockColor = Color.white,
        lockColor = Color.grey;
    public
Button[] buttons;
    [HideInInspector]
    public
        Text[] buttonsTexts;
    [HideInInspector]
    public
        Image[] buttonsImage;
    //////////////////
    [Header("Scenes  References")]
    Text coinsText;
    Toggle randomToggle;
    [HideInInspector]
    public GameObject container;
    Canvas store;
    //Vaules
    int
        coins,
        index,
        isRandom = 1,
        startedButtonIndex = 0,
        buttonsPerPage = 10;
    [HideInInspector]
    public
        int[] prices;
    string 
        have;
    public string[] 
        data = { "Coins", "PlayerIndex", "isRandom", "Items" };
    //////////////////
    [Header("Items")]
    public Sprite[] icon;

    bool isMove = false;
    Int32 currentIndex()
    {
        return index % 10;
    }
    int GetPageIndex(int x)
    {
        if (x < 11) return 0;
        while (x >= 10) { x /= 10; }
        int d = Convert.ToInt32(string.Format("{0}{1}", x, 0));
        return d;
    }
    [Header("ScriptSettings")]
    [HideInInspector]
    public bool
        hide = true;
    //Instance

#endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
#region Initialize Store
    private void Awake()
    {
        GetRefer();
        CheckSave();
        Initialize();
    }
    void GetRefer()
    {
        store = GetComponentInChildren<Canvas>();
        coinsText = GameObject.FindGameObjectWithTag("coinsText").GetComponent<Text>();
        randomToggle = GameObject.FindGameObjectWithTag("RandomPlayerToggle").GetComponent<Toggle>();
        container = GameObject.FindGameObjectWithTag("storeContainer");
        coinsImage = buttonsImage[0].sprite;
    }
#region Corountines
    public IEnumerator WaitPlayer()
    {
        yield return new WaitUntil(() => player != null);
        SettingPlayer();
        SetPlayer(index);
        RandomizePlayer();

    }
#endregion
    void CheckSave()
    {
        index = PlayerPrefs.GetInt(data[1]);
        coins = PlayerPrefs.GetInt(data[0]);
        coinsText.text = coins.ToString();
        if (PlayerPrefs.HasKey(data[3]))
        {
            have = PlayerPrefs.GetString(data[3]);
        }
        else
        {
            have = icon[0].name;
            PlayerPrefs.SetString(data[3],have);
        }
    }
    void Initialize()
    {
        SetToggle();
        StartCoroutine(WaitPlayer());
        SetLevelButtons(0);
    }
    public void OnOpenLevelMenu()
    {
        int x = GetPageIndex(index);
        if (x == currentIndex()) return;
        SetLevelButtons(x);
    }
    void SetLevelButtons(int current)
    {
        //Set Current Level Page
        //if (current != GetPageIndex((int)this.index)) SetCurrentButton((int)this.index, false);
        //else SetCurrentButton((int)this.index, true);
        startedButtonIndex = current;
        //int unlock = PlayerPrefs.GetInt(data[4]);
        for (int i = 0; i < buttons.Length; i++)
        {
            if (startedButtonIndex + i >= icon.Length) { buttons[i].gameObject.SetActive(false); }
            else if (!buttons[i].gameObject.activeSelf) { buttons[i].gameObject.SetActive(true); }
            int cIndex = i + startedButtonIndex;
            if (cIndex < icon.Length)
            {
                if (!buttons[i].gameObject.activeSelf) buttons[i].gameObject.SetActive(true);
                if (have.Contains(icon[cIndex].name))
                {
                    //enabled
                    //buttons[i].transform.GetChild(0).gameObject.SetActive(false);
                    buttonBackground[i].color = unlockColor;
                    buttonsImage[i].sprite = icon[cIndex];
                    buttonsTexts[i].text = null;
                }
                else
                {
                    //buttons[i].transform.GetChild(0).gameObject.SetActive(true);
                    buttonBackground[i].color = lockColor;
                    buttonsImage[i].sprite = coinsImage;
                    buttonsTexts[i].text = prices[cIndex].ToString();
                }
            }
            else { buttons[i].gameObject.SetActive(false); }
        }
    }
    public void SwipeButtons(int delta)
    {
        if (isMove) return;
        int n;
        if (delta < 0) n = -buttonsPerPage;
        else n = buttonsPerPage;
        n += startedButtonIndex;
        if (n >= icon.Length) n = startedButtonIndex = 0;
        else if (n < 0) n = startedButtonIndex = GetPageIndex(icon.Length - buttonsPerPage);
        SetLevelButtons(n);
    }
    //void SetCurrentButton(int i, bool enabled)
    //{
    //    if (i < icon.Length)
    //    {
    //        Button button;
    //        i = currentIndex();
    //        //print(i);
    //        //if (i >= buttons.Length) return;
    //        //set old button
    //        button = buttons[oldIndex];
    //        Color color = Color.HSVToRGB(0, 0, 100);
    //        //SpriteColor
    //        button.image.color = color;
    //        buttonsTexts[oldIndex].color = color;
    //        //MatColor
    //        button.image.material = themeManager.buttonMat;
    //        buttonsTexts[oldIndex].material = themeManager.buttonMat;
    //        //SetCurrentButton
    //        if (enabled)
    //        {
    //            button = buttons[i];
    //            //MatColor
    //            button.image.material = themeManager.iconMat;
    //            buttonsTexts[i].material = themeManager.iconMat;
    //            //SpriteColor
    //            button.image.color = buttonColor;
    //            buttonsTexts[i].color = buttonColor;
    //        }
    //        oldIndex = i;
    //    }
    //}
#endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
#region Buy/Set Itens
    public void Buy(int i)
    {
        int x = i + startedButtonIndex;
        print(x);
        if (have.Contains(icon[x].name))
        {
            Chance(x);
        }
        else
        {
            GameObject button = EventSystem.current.currentSelectedGameObject;
            int p = int.Parse(buttonsTexts[i].text);
            if (coins >= p)
            {
                //take coins
                coins -= p;
                coinsText.text = coins.ToString();
                //Set Button
                buttonBackground[i].color = unlockColor;
                buttonsImage[i].sprite = icon[x];
                buttonsTexts[i].text = null;
                //Set Vaules
                index = x;
                have += icon[x].name + ",";
                SetPlayer(x);
                //Save
                PlayerPrefs.SetInt(data[1], index);
                PlayerPrefs.SetInt(data[0], coins);
                PlayerPrefs.SetString(data[3], have);
            }
        }
    }
    public void Select()
    {
        GameObject button = EventSystem.current.currentSelectedGameObject;
        int i = int.Parse(button.name);
        Chance(i);
    }
    void Chance(int i)
    {
        SetPlayer(i);
        index = i;
        PlayerPrefs.SetInt(data[1], index);
        //Hide Shopping after set the sprite
        if (hide)
        {
            ShowCanvas(store);
        }
    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Virtual
    public virtual void SetPlayer(int i)
    {
    }
    public virtual void SettingPlayer()
    {

    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region RandomToggle
    void SetToggle()
    {
        isRandom = PlayerPrefs.GetInt(data[2]);
        if (isRandom == 1)
        {
            randomToggle.isOn = true;
        }
        else
        {
            randomToggle.isOn = false;
        }
    }
    public void ToggleRandom()
    {
        bool Enable = randomToggle.isOn;
        if (Enable)
        {
            isRandom = 1;
        }
        else
        {
            isRandom = 0;
            PlayerPrefs.SetInt(data[1], index);
        }
        PlayerPrefs.SetInt(data[2], isRandom);
    }
    public void RandomizePlayer()
    {
        // 0 = false 1 = true
        if (isRandom == 1)
        {
            List<int> AllItens = new List<int>();
            for (int i = 0; i < data.Length; i++)
            {
                if (have.Contains(icon[i].name))
                {
                    AllItens.Add(i);
                }
            }
            index = AllItens[UnityEngine.Random.Range(0, AllItens.Count)];
            SetPlayer(index);
        }
    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Coins
    public void SaveCoins(int c)
    {
        coins += c;
        coinsText.text = coins.ToString();
        PlayerPrefs.SetInt(data[0], coins);
    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Functions
    public void ShowCanvas(Canvas c)
    {
        c.enabled = !c.isActiveAndEnabled;
    }
    public void SetPlayer(GameObject p)
    {
        player = p;
    }
#endregion
}
#endregion
///////////////////////////////////////////////////////////////////////////////////////////////////////////
#region Editor Settings
#if UNITY_EDITOR
[CustomEditor(typeof(Store))]
public class Store_Editor : Editor
{
    //Price
    bool
        increasePrice = true;
    int
        after = 1,
        add = 100,
        price = 100;
    public override void OnInspectorGUI()
    {
        SetSettings();
    }
    public void SetSettings()
    {
        DrawDefaultInspector(); // for non-HideInInspector fields
        Store script = (Store)target;
        EditorGUILayout.LabelField("Script Settings", EditorStyles.boldLabel);
        price = EditorGUILayout.IntField("Default price:", price);
        script.hide = EditorGUILayout.Toggle("Hide Store after buy", script.hide);
        // draw checkbox for the bool
        increasePrice = EditorGUILayout.Toggle("Increase price", increasePrice);
        if (increasePrice)
        {
            after = EditorGUILayout.IntSlider("Every X buttons:", after, 1, script.icon.Length);
            add = EditorGUILayout.IntField("increase price in:", add);
        }
        else
        {
            after = 0;
            add = 0;
        }
        if (script.container == null)
        {
            script.container = GameObject.FindGameObjectWithTag("storeContainer");
        }
        int x = after;
        int p = price;
        if (script.prices.Length != script.icon.Length)
        {
            script.prices = new int[script.icon.Length];
            for (int i = 0; i < script.icon.Length; i++)
            {
                if (i == x)
                {
                    x += after;
                    p += add;
                }
                script.prices[i] = p;
            }
        }
        int n = script.buttons.Length;
        if (script.buttonsTexts.Length != n || script.buttonsImage.Length != n || script.buttonBackground.Length != n || script.buttons.Length == 0)
        {
            script.buttons = script.container.GetComponentsInChildren<Button>();
            script.buttonsTexts = new Text[n];
            script.buttonsImage = new Image[n];
            script.buttonBackground = new Image[n];
            for (int i = 0; i < script.buttons.Length; i++)
            {
                script.buttonsImage[i] = script.buttons[i].transform.GetChild(0).GetComponent<Image>();
                script.buttonsTexts[i] = script.buttons[i].GetComponentInChildren<Text>();
                script.buttonBackground[i] = script.buttons[i].GetComponent<Image>();
            }

        }
    }

}
#endif
#endregion


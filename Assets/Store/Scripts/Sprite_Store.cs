using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class Sprite_Store : Store
{
    SpriteRenderer p;
    public
    Sprite[] items;
    public override void SettingPlayer()
    {
        p = player.GetComponent<SpriteRenderer>();
    }
    public override void SetPlayer(int i)
    {
        p.sprite = items[i];
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(Sprite_Store))]
public class SpriteStore_Editor : Store_Editor
{
    public override void OnInspectorGUI()
    {
        SetSettings();
    }
}
#endif

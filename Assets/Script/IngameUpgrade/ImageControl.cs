using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ImageControl : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Image image;

    private void OnEnable()
    {
        StartCoroutine(SwichingSprite());
    }

    IEnumerator SwichingSprite()
    {
        while (true)
        {
            image.sprite = sprites[0];
            yield return new WaitForSecondsRealtime(0.2f);
            image.sprite = sprites[1];
            yield return new WaitForSecondsRealtime(0.2f);
        }
    }
}

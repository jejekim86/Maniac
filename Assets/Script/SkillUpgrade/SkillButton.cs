using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject infoPanel;

    public void SetSkillInfo(GameObject infoPanel)
    {
        this.infoPanel = infoPanel;
        infoPanel.SetActive(false); // ó������ ��Ȱ��ȭ
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }
}

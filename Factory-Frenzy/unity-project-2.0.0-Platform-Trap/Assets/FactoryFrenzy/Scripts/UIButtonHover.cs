using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string prefabName;
    private Text buttonText;
    private TMPro.TextMeshProUGUI tmpText;

    private string originalText;

    void Awake()
    {
        buttonText = GetComponentInChildren<Text>();
        tmpText = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (buttonText != null) originalText = buttonText.text;
        if (tmpText != null) originalText = tmpText.text;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText != null) buttonText.text = prefabName;
        if (tmpText != null) tmpText.text = prefabName;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null) buttonText.text = originalText;
        if (tmpText != null) tmpText.text = originalText;
    }
}


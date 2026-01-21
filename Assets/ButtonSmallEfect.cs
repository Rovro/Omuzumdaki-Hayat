using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSmallEfect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rect;
    private Vector3 originalScale;



    void Awake()
    {
        rect = GetComponent<RectTransform>();
        originalScale = rect.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rect.localScale = originalScale * 1.1f; // %10 büyüt
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rect.localScale = originalScale; // eski haline dön
    }
}

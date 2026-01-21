using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverTransform : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        rect.localRotation = Quaternion.Euler(0, 0, 5); // 5 derece döndür
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rect.localScale = originalScale; // eski haline dön
        rect.localRotation = Quaternion.Euler(0, 0, 15); ; // eski haline dön
    }
}

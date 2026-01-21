using UnityEngine;
using TMPro;

public class SimpleTextPop : MonoBehaviour
{
    public TextMeshProUGUI myText;   // Inspector’dan ata
    public float growSpeed = 2f;     // Ne kadar hýzlý büyüsün
    public float maxScale = 1.5f;    // Ne kadar büyüsün
    private bool isGrowing = false;

    void Update()
    {
        if (isGrowing)
        {
            // Ölçeði büyüt
            myText.transform.localScale = Vector3.Lerp
            (myText.transform.localScale,
            Vector3.one * maxScale,
            Time.unscaledDeltaTime * growSpeed);


            // Maksimuma çok yaklaþtýysa dur
            if (Vector3.Distance(myText.transform.localScale, Vector3.one * maxScale) < 0.01f)
            {
                isGrowing = false;
            }
        }
    }
    // Senin fonksiyonun içinde burayý çaðýr
    public void ShowText()
    {
        myText.gameObject.SetActive(true);
        myText.transform.localScale = Vector3.one; // Baþtan küçük baþlat
        isGrowing = true;
    }
}

using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeVisual : MonoBehaviour
{
    public Transform soldier;  // Asker referansý (Inspector’dan sürükle býrak)
    public Transform stretcher;  // Çocuk referansý (Inspector’dan sürükle býrak)

    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;

        // Opsiyonel: çizgi stilini ayarlamak
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.brown;
        lr.endColor = Color.sandyBrown;
    }

    void Update()
    {
        if (soldier == null || stretcher == null) return;

        // Ýpin uçlarýný asker ve çocuðun ayak hizasýna baðla
        lr.SetPosition(0, soldier.position + new Vector3(0, -0.5f, 0));
        lr.SetPosition(1, stretcher.position + new Vector3(0, -0.5f, 0));
    }
}

using UnityEngine;

[RequireComponent(typeof(DistanceJoint2D), typeof(Rigidbody2D))]
public class TowAnchorSync : MonoBehaviour
{
    [Header("Flip Thresholds")]
    public float flipDeadZoneX = 0.12f;   // |ropeDir.x| bundan k���kse flip yok
    public float minFlipInterval = 0.15f; // iki flip aras�nda min s�re (opsiyonel)
    private float _lastFlipTime = -999f;

    [Header("References")]
    public Rigidbody2D soldierRb;         // Asker RB (connected body)
    public Transform soldierHookPoint;    // Soldier/HookPoint (child)
    public Transform stretcherFrontPoint; // Stretcher/FrontPoint (child)
    public Transform stretcherVisual;     // Sedyenin g�rsel holder'� (flip i�in)

    [Header("Visual Flip")]
    public bool flipStretcherByRopeDir = true;

    private DistanceJoint2D joint;
    private Rigidbody2D rb;

    private void Awake()
    {
        joint = GetComponent<DistanceJoint2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Reset()
    {
        joint = GetComponent<DistanceJoint2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void LateUpdate()
    {
        if (!joint || !soldierRb || !soldierHookPoint || !stretcherFrontPoint) return;

        // 1) SEDYE taraf� (ANCHOR): d�nya > sedyenin local'i
        Vector2 anchorLocalOnStretcher =
            (Vector2)transform.InverseTransformPoint(stretcherFrontPoint.position);
        joint.anchor = anchorLocalOnStretcher;

        // 2) ASKER taraf� (CONNECTED ANCHOR): d�nya  askerin local'i
        Vector2 connectedLocalOnSoldier =
            (Vector2)soldierRb.transform.InverseTransformPoint(soldierHookPoint.position);
        joint.connectedAnchor = connectedLocalOnSoldier;

        // 3) (Opsiyonel) Sedyeyi ip do�rultusuna g�re flip et
        if (flipStretcherByRopeDir && stretcherVisual != null)
        {
            Vector2 ropeDir = (Vector2)(soldierHookPoint.position - stretcherFrontPoint.position);
            float absX = Mathf.Abs(ropeDir.x);

            // DEAD-ZONE: x yeterince b�y�k de�ilse flip yapma
            if (absX > flipDeadZoneX && (Time.time - _lastFlipTime) > minFlipInterval)
            {
                float sx = Mathf.Sign(ropeDir.x);
                Vector3 s = stretcherVisual.localScale;
                float targetX = Mathf.Abs(s.x) * (sx >= 0 ? 1f : -1f);

                if (!Mathf.Approximately(s.x, targetX)) // ger�ekten y�n de�i�iyorsa uygula
                {
                    s.x = targetX;
                    stretcherVisual.localScale = s;
                    _lastFlipTime = Time.time;
                }
            }
        }

    }
}

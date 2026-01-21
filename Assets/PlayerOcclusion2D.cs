using UnityEngine;
public class PlayerOcclusion2D : MonoBehaviour
{
    [Header("Detection")]
    [Tooltip("Duvar/occluder objelerinin bulunduðu Layer (Inspector'dan seçin).")]
    public LayerMask occluderLayer;
    [Tooltip("Oyuncunun etrafýnda duvar kontrolü yapýlacak yarýçap (küçük tutun).")]
    public float checkRadius = 0.6f;

    [Header("Ordering")]
    [Tooltip("Duvar front sprite'ýnýn sortingOrder'ýna göre oyuncunun önde/arkada olmasýný ayarlar.")]
    public int orderOffset = 1; // duvarýn üzerinde öne çýkmak için +1, arkada kalmak için -1 kullanýlýr

    // Fallback: eðer duvarda SpriteRenderer yoksa bu base order kullanýlýr (opsiyonel)
    public int fallbackOccluderOrder = 100;

    private SpriteRenderer sr;
    private int defaultOrder;

    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        defaultOrder = sr.sortingOrder;
    }

    void Update()
    {
        // çevredeki occluder'larý bul
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, checkRadius, occluderLayer);

        if (hits.Length == 0)
        {
            // Yakýnda hiç duvar yok -> default order'a dön
            if (sr.sortingOrder != defaultOrder) sr.sortingOrder = defaultOrder;
            return;
        }

        // Yakýndaki herhangi bir occluder ile karþýlaþtýk — ilkini kullan (uzaktayken önemli deðil demiþtiniz)
        Collider2D hit = hits[0];
        Transform occluderT = hit.transform;
        float occluderX = occluderT.position.x;

        // occluder'ýn SpriteRenderer'ý varsa onun order'ýný al
        SpriteRenderer occluderSR = occluderT.GetComponent<SpriteRenderer>();
        int occluderOrder = (occluderSR != null) ? occluderSR.sortingOrder : fallbackOccluderOrder;

        // Karakter solundaysa öne çýksýn, saðýndaysa arkada kalsýn
        if (transform.position.x < occluderX)
            sr.sortingOrder = occluderOrder + orderOffset; // önde göster
        else
            sr.sortingOrder = occluderOrder - orderOffset; // arkada göster
    }

    // Görsel yardým: Scene görünümünde radius'u göster
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}

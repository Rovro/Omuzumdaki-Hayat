using System.Collections;
using UnityEngine;

public class MineSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject minePrefab;

    [Header("Spawn Rules")]
    [Tooltip("Sahnedeki eþ zamanlý maksimum mayýn sayýsý")]
    public int maxAlive;

    [Tooltip("Spawn denemeleri arasýndaki bekleme (sn)")]
    public float spawnInterval;

    [Tooltip("Her spawn için uygun pozisyon ararken en fazla kaç deneme yapýlýr")]
    public int maxTriesPerSpawn;

    [Header("Area")]
    [Tooltip("Alaný bir BoxCollider2D ile tanýmlamak istersen ver. Boþsa areaSize kullanýlýr.")]
    public BoxCollider2D areaCollider;

    [Tooltip("Collider vermediysen yerel eksende geniþlik/yükseklik")]
    public Vector2 areaSize = new Vector2(8f, 4f);

    [Header("Overlap (Opsiyonel)")]
    [Tooltip("Spawn noktasýnda çakýþma kontrolü yap")]
    public bool avoidOverlap = true;

    [Tooltip("Çakýþma kontrol yarýçapý")]
    public float overlapRadius = 1f;

    [Tooltip("Çakýþmayý kontrol ederken hangi layer'larý engelle (ör. Ground, Enemy, Mine)")]
    public LayerMask blockLayers;

    private WaitForSeconds _wait;

    private void OnValidate()
    {
        maxAlive = Mathf.Max(0, maxAlive);
        spawnInterval = Mathf.Max(0.0001f, spawnInterval);
        maxTriesPerSpawn = Mathf.Max(1, maxTriesPerSpawn);
        areaSize = new Vector2(Mathf.Max(0.1f, areaSize.x), Mathf.Max(0.1f, areaSize.y));
        overlapRadius = Mathf.Max(0.01f, overlapRadius);
    }

    private void Start()
    {
        if (minePrefab == null)
        {
            Debug.LogWarning("[NightMineSpawner] Mine Prefab atanmadý.");
            enabled = false;
            return;
        }

        _wait = new WaitForSeconds(spawnInterval);
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            // Çocuklar bu spawner'ýn altýna parent edildiði için sayým kolay
            if (transform.childCount < maxAlive)
            {
                TrySpawnMine();
            }

            yield return _wait;
        }
    }

    private void TrySpawnMine()
    {
        for (int i = 0; i < maxTriesPerSpawn; i++)
        {
            Vector2 pos = GetRandomPointInArea();

            if (avoidOverlap)
            {
                // Çakýþma varsa bu pozisyonu geç
                if (Physics2D.OverlapCircle(pos, overlapRadius, blockLayers))
                    continue;
            }

            // Üret ve bu spawner'ýn altýna yerleþtir
            GameObject mine = Instantiate(minePrefab, pos, Quaternion.identity, this.transform);
            return;
        }
        // Uygun yer bulunamazsa sessizce geç—sýradaki döngüde tekrar dener
    }

    private Vector2 GetRandomPointInArea()
    {
        if (areaCollider != null)
        {
            // BoxCollider2D dünya uzayýnda; bounds'tan rastgele nokta seç
            Bounds b = areaCollider.bounds;
            float x = Random.Range(b.min.x, b.max.x);
            float y = Random.Range(b.min.y, b.max.y);
            return new Vector2(x, y);
        }
        else
        {
            // Yerel alan: spawner'ýn merkezinden areaSize kadar
            Vector2 half = areaSize * 0.5f;
            Vector2 local = new Vector2(Random.Range(-half.x, half.x), Random.Range(-half.y, half.y));
            return (Vector2)transform.TransformPoint(local);
        }
    }

    // Sahne içinde alaný görselleþtir (Editor)
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.25f);

        if (areaCollider != null)
        {
            Bounds b = areaCollider.bounds;
            Gizmos.DrawCube(b.center, b.size);
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.9f);
            Gizmos.DrawWireCube(b.center, b.size);
        }
        else
        {
            Matrix4x4 old = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, new Vector3(areaSize.x, areaSize.y, 0.1f));
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.9f);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(areaSize.x, areaSize.y, 0.1f));
            Gizmos.matrix = old;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!avoidOverlap) return;

        Gizmos.color = Color.yellow;
        // Merkezde örnek bir overlap dairesi göster (bilgi amaçlý)
        Vector2 center = areaCollider ? (Vector2)areaCollider.bounds.center : (Vector2)transform.position;
        Gizmos.DrawWireSphere(center, overlapRadius);
    }
}

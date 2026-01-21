using System.Collections;
using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    public enum BombSpawnMode { RandomArea, TargetTransform, PredictiveTarget }
    public enum GroundingMode { GroundRaycast, UseTargetY, UseTargetColliderBottom }

    // Yeni: Hedef kaynağı seçimi
    public enum TargetSource { Player, Stretcher, BetweenPlayerAndStretcher }

    [Header("Spawn Area")]
    [SerializeField] private BoxCollider2D spawnArea; // isTrigger = true önerilir (X sınırı için referans)

    [Header("Prefabs")]
    [SerializeField] private Bomb bombPrefab;

    [Header("Spawn Timing")]
    [Min(0.05f)][SerializeField] private float minSpawnDelay = 1.5f;
    [Min(0.05f)][SerializeField] private float maxSpawnDelay = 3.0f;
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool infiniteLoop = true;
    [SerializeField] private int initialBurstCount = 0;

    [Header("Default Bomb Params (applies to each spawned bomb)")]
    [Min(0.05f)][SerializeField] private float fallTime = 1.5f;
    [Min(0.05f)][SerializeField] private float dropHeight = 6f;
    [Min(0.05f)][SerializeField] private float shadowScale = 1.0f;
    [Min(0.05f)][SerializeField] private float damageRadius = 0.6f;
    [Range(0f, 2f)][SerializeField] private float damageRadiusExtra = 0.2f;
    [SerializeField] private bool radiusFromShadow = true;
    [SerializeField] private LayerMask damageMask;
    [SerializeField] private int damageAmount = 1;

    // [Header("Audio Defaults")] // <- ŞİMDİLİK KAPALI
    // [SerializeField] private AudioClip whistleSFX;
    // [SerializeField] private AudioClip explosionSFX;

    [Header("Targeting")]
    [SerializeField] private BombSpawnMode spawnMode = BombSpawnMode.TargetTransform;
    [SerializeField] private TargetSource targetSource = TargetSource.Player;

    [Tooltip("Karakterin Transform'u ve opsiyonel Rigidbody2D'si")]
    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody2D playerRb;

    [Tooltip("Sedyenin Transform'u ve opsiyonel Rigidbody2D'si")]
    [SerializeField] private Transform stretcher;
    [SerializeField] private Rigidbody2D stretcherRb;

    [SerializeField] private bool clampToSpawnArea = true;   // hedef X'i alan sınırına kırp
    [Tooltip("Önden hedefleme: hedefHızı * fallTime * leadScale")]
    [SerializeField] private float leadScale = 1.0f;

    [Header("Between settings (Player <-> Stretcher)")]
    [Tooltip("Sabit t kullan (0=player, 1=stretcher). Kapalıysa her spawn'da rastgele t)")]
    [SerializeField] private bool betweenUseFixedT = false;
    [Range(0f, 1f)][SerializeField] private float betweenFixedT = 0.5f;
    [Tooltip("Rastgele t için aralık (0=player, 1=stretcher)")]
    [SerializeField] private Vector2 betweenTRange = new Vector2(0.3f, 0.7f);

    [Header("Grounding (Y belirleme)")]
    [SerializeField] private GroundingMode groundingMode = GroundingMode.UseTargetColliderBottom;
    [SerializeField] private LayerMask groundMask;            // Sadece GroundRaycast için kullanılır
    [SerializeField] private float raycastTopYOffset = 5f;    // Raycast başlama yüksekliği
    [SerializeField] private float raycastDistance = 50f;     // Raycast mesafesi

    private Coroutine _loop;

    private void Start()
    {
        if (playOnStart)
        {
            for (int i = 0; i < initialBurstCount; i++) SpawnOne();
            if (infiniteLoop) _loop = StartCoroutine(SpawnLoop());
        }
    }

    public void StartSpawning() { if (_loop == null) _loop = StartCoroutine(SpawnLoop()); }
    public void StopSpawning() { if (_loop != null) StopCoroutine(_loop); _loop = null; }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);
            SpawnOne();
        }
    }
    private void SpawnOne()
    {
        if (bombPrefab == null)
        {
            Debug.LogWarning("[BombSpawner] bombPrefab atanmamış.");
            return;
        }
        Vector3 dropPos = GetDropPoint();
        var bomb = Instantiate(bombPrefab, dropPos, Quaternion.identity);

        bomb.Configure(
            fallTime, dropHeight,
            shadowScale, damageRadius, damageRadiusExtra,
            radiusFromShadow, damageMask, damageAmount
        // , whistleSFX, explosionSFX
        );

        bomb.Activate();
    }

    private Vector3 GetDropPoint()
    {
        // Random alan modu (hedef yok)
        if (spawnMode == BombSpawnMode.RandomArea)
            return GetRandomGroundPoint();

        // Hedef pozisyonu ve hızını seç
        Vector3 pos;
        Vector2 vel;
        if (!TryGetActiveTarget(out pos, out vel))
        {
            // Hedef yoksa randoma düş
            return GetRandomGroundPoint();
        }

        // Predictive ise önden hedefle
        if (spawnMode == BombSpawnMode.PredictiveTarget)
        {
            pos += (Vector3)(vel * (fallTime * leadScale));
        }

        return GetTargetPoint(pos);
    }

    private bool TryGetActiveTarget(out Vector3 pos, out Vector2 vel)
    {
        pos = Vector3.zero;
        vel = Vector2.zero;

        switch (targetSource)
        {
            case TargetSource.Player:
                if (player == null) return false;
                pos = player.position;
                if (playerRb != null) vel = playerRb.linearVelocity;
                return true;

            case TargetSource.Stretcher:
                if (stretcher == null) return false;
                pos = stretcher.position;
                if (stretcherRb != null) vel = stretcherRb.linearVelocity;
                return true;

            case TargetSource.BetweenPlayerAndStretcher:
                if (player == null || stretcher == null) return false;

                float t;
                if (betweenUseFixedT)
                {
                    t = Mathf.Clamp01(betweenFixedT);
                }
                else
                {
                    float minT = Mathf.Clamp01(Mathf.Min(betweenTRange.x, betweenTRange.y));
                    float maxT = Mathf.Clamp01(Mathf.Max(betweenTRange.x, betweenTRange.y));
                    t = Random.Range(minT, maxT);
                }

                pos = Vector3.Lerp(player.position, stretcher.position, t);

                // Predictive için hızları da harmanlayalım (varsa)
                Vector2 vP = playerRb ? playerRb.linearVelocity : Vector2.zero;
                Vector2 vS = stretcherRb ? stretcherRb.linearVelocity : Vector2.zero;
                vel = Vector2.Lerp(vP, vS, t);
                return true;
        }

        return false;
    }

    private Vector3 GetTargetPoint(Vector3 worldPos)
    {
        // X'i (gerekirse) spawnArea sınırlarına kırp
        float x = worldPos.x;
        if (clampToSpawnArea && spawnArea != null)
        {
            var b = spawnArea.bounds;
            x = Mathf.Clamp(x, b.min.x, b.max.x);
        }

        // Y'yi grounding moduna göre belirle
        float y;
        switch (groundingMode)
        {
            case GroundingMode.UseTargetY:
                y = worldPos.y; // hedefin o anki dünya Y'si
                break;

            case GroundingMode.UseTargetColliderBottom:
                y = GetTargetColliderBottomY(worldPos, worldPos.y);
                break;

            case GroundingMode.GroundRaycast:
            default:
                y = GetGroundYAtX(x);
                break;
        }

        return new Vector3(x, y, 0f);
    }

    private float GetTargetColliderBottomY(Vector3 worldPos, float fallbackY)
    {
        // Player/stretcher fark etmeksizin en yakın hedef koliderini dener
        Collider2D col = null;

        if (targetSource == TargetSource.Player && player != null)
            col = player.GetComponentInChildren<Collider2D>();
        else if (targetSource == TargetSource.Stretcher && stretcher != null)
            col = stretcher.GetComponentInChildren<Collider2D>();
        else if (targetSource == TargetSource.BetweenPlayerAndStretcher)
        {
            // Between'de worldPos'a en yakın olanın alt kenarını kullanmak mantıklı
            float dP = player ? Mathf.Abs(worldPos.x - player.position.x) : float.PositiveInfinity;
            float dS = stretcher ? Mathf.Abs(worldPos.x - stretcher.position.x) : float.PositiveInfinity;
            Transform pick = (dP <= dS) ? player : stretcher;
            if (pick) col = pick.GetComponentInChildren<Collider2D>();
        }

        if (col != null) return col.bounds.min.y;
        return fallbackY; // collider yoksa hedef Y'yi kullan
    }

    private Vector3 GetRandomGroundPoint()
    {
        if (spawnArea == null)
        {
            Debug.LogWarning("[BombSpawner] spawnArea eksik. Spawner pozisyonu kullanılacak (Random).");
            return transform.position;
        }

        var b = spawnArea.bounds;
        float x = Random.Range(b.min.x, b.max.x);

        // Random modda Y'yi grounding moduna göre seçelim
        float y = (groundingMode == GroundingMode.GroundRaycast) ? GetGroundYAtX(x) : b.min.y;
        return new Vector3(x, y, 0f);
    }

    private float GetGroundYAtX(float x)
    {
        float topY = (spawnArea != null ? spawnArea.bounds.max.y : transform.position.y) + raycastTopYOffset;

        if (groundMask != 0)
        {
            Vector2 origin = new Vector2(x, topY);
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, raycastDistance, groundMask);
            // Debug.DrawLine(origin, origin + Vector2.down * raycastDistance, Color.yellow, 0.1f);
            if (hit.collider != null) return hit.point.y;
        }

        // Fallback: spawnArea alt kenarı veya spawner Y
        return (spawnArea != null) ? spawnArea.bounds.min.y : transform.position.y;
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnArea == null) return;
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.25f);
        Gizmos.DrawCube(spawnArea.bounds.center, spawnArea.bounds.size);
    }
}


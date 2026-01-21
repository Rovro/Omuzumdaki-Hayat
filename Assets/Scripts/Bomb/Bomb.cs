using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform shadow;            // gölge objesi
    [SerializeField] private Transform bombModel;         // havadaki bomba modeli
    [SerializeField] private ParticleSystem explosionFx;  // patlama efekti (disabled başlayabilir)

    // [SerializeField] private AudioSource audioSource;   // <- ŞİMDİLİK KAPALI
    // [SerializeField] private AudioClip whistleSFX;      // <- ŞİMDİLİK KAPALI
    // [SerializeField] private AudioClip explosionSFX;    // <- ŞİMDİLİK KAPALI

    [Header("Timing")]
    [Min(0.05f)][SerializeField] private float fallTime = 1.5f;  // gölge göründükten patlayana kadar
    [Min(0.05f)][SerializeField] private float dropHeight = 6f;  // bombModel'in başladığı yerel Y yüksekliği

    [Header("Damage & Visuals")]
    [Min(0.05f)][SerializeField] private float shadowScale = 1.0f;      // gölge boyutu (X ve Y'ye uygulanır)
    [Min(0.05f)][SerializeField] private float damageRadius = 0.6f;     // patlama yarıçapı
    [Range(0f, 2f)][SerializeField] private float damageRadiusExtra = 0.2f; // gölgeye oranla ekstra
    [SerializeField] private bool radiusFromShadow = true;               // true ise damage = shadow/2 + extra
    [SerializeField] private LayerMask damageMask;                       // Player, Stretcher vb.
    [SerializeField] private int damageAmount = 1;

    [Header("Lifecycle")]
    [SerializeField] private float destroyDelayAfterExplode = 1.0f; // efekt bittikten sonra yok et
    [SerializeField] private bool disableRenderersAfterExplode = true;

    public GameObject bombObjecteffect;

    private bool _isRunning;

    #region Public API (Spawner buradan ayar basar)
    public void Configure(
        float fallTime, float dropHeight,
        float shadowScale, float damageRadius, float damageRadiusExtra,
        bool radiusFromShadow, LayerMask damageMask, int damageAmount
    // , AudioClip whistle, AudioClip explosion   // <- ŞİMDİLİK KAPALI
    )
    {
        this.fallTime = fallTime;
        this.dropHeight = dropHeight;
        this.shadowScale = shadowScale;
        this.damageRadius = damageRadius;
        this.damageRadiusExtra = damageRadiusExtra;
        this.radiusFromShadow = radiusFromShadow;
        this.damageMask = damageMask;
        this.damageAmount = damageAmount;

        // if (whistle != null) whistleSFX = whistle;      // <- KAPALI
        // if (explosion != null) explosionSFX = explosion;// <- KAPALI
    }

    /// <summary>Spawner tarafından spawn edildikten sonra çağrılır.</summary>
    public void Activate()
    {
        // Gölgeyi ölçekle
        if (shadow != null)
            shadow.localScale = new Vector3(shadowScale, shadowScale, 1f);

        // Bombayı yukarıda başlat
        if (bombModel != null)
            bombModel.localPosition = new Vector3(0f, dropHeight, 0f);

        // İsteğe bağlı ses (şimdilik kapalı)
        // if (audioSource != null && whistleSFX != null)
        //     audioSource.PlayOneShot(whistleSFX);

        // Hasar yarıçapını gölgeye göre otomatikle
        if (radiusFromShadow)
            damageRadius = (shadowScale * 0.5f) + damageRadiusExtra;

        if (!_isRunning) StartCoroutine(FallThenExplode());
    }
    #endregion

    private IEnumerator FallThenExplode()
    {
        _isRunning = true;

        float t = 0f;
        while (t < fallTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / fallTime);
            if (bombModel != null)
            {
                float y = Mathf.Lerp(dropHeight, 0f, k);
                bombModel.localPosition = new Vector3(0f, y, 0f);
            }
            yield return null;
        }

        Explode();
    }

    private void Explode()
    {
        // Ses (şimdilik kapalı)
        // if (audioSource != null && explosionSFX != null)
        //     audioSource.PlayOneShot(explosionSFX);

        // Efekt
        if (explosionFx != null)
        {
            explosionFx.transform.position = transform.position;
            explosionFx.gameObject.SetActive(true);
            explosionFx.Play();
        }
        if (disableRenderersAfterExplode)
            ToggleSpriteRenderers(false);
        Instantiate(bombObjecteffect, transform.position, Quaternion.identity);

        DoDamage();
        Destroy(gameObject, destroyDelayAfterExplode);
    }

    private void DoDamage()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, damageRadius, damageMask);
        foreach (var col in hits)
        {
            var dmg = col.GetComponentInParent<PlayerHealth>();
            if (dmg != null)
            {
                dmg.TakeDamage(damageAmount);
                continue;
            }
            col.gameObject.SendMessage("ApplyDamage", damageAmount, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void ToggleSpriteRenderers(bool value)
    {
        var renderers = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var r in renderers) r.enabled = value;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.4f, 0.1f, 0.45f);
        float r = radiusFromShadow ? (shadowScale * 0.5f + damageRadiusExtra) : damageRadius;
        Gizmos.DrawSphere(transform.position, r);
    }
}

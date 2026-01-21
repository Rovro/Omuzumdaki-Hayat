using UnityEngine;
using UnityEngine.UI;

public class SafeZoneUIController : MonoBehaviour
{
    public Image safeZoneFillImage;
    public float fillSpeed , drainSpeed;
    public string playerZoneTag;
    public float fill = 0f;

    public Sniper2D sniper;

    Transform playerCheck;

    void Start()
    {
        playerCheck = GameObject.FindWithTag(playerZoneTag)?.transform;
    }

    void Update()
    {
        bool allSafe = false;
        if (playerCheck != null)
        {
            var hits = Physics2D.OverlapPointAll(playerCheck.position);
            foreach (var h in hits)
            {
                if (h.TryGetComponent<SafeZoneChecker>(out var sz))    
                    allSafe = sz.IsAllSafe; break; 
            }
        }

        fill = Mathf.Clamp01(allSafe ? fill - drainSpeed * Time.deltaTime : fill + fillSpeed * Time.deltaTime);

        if (safeZoneFillImage) 
            safeZoneFillImage.fillAmount = fill;

        if (fill >= 1f)
        {
            if (sniper != null) 
                sniper.FireAtPlayer();

            fill = 0f;

            if (safeZoneFillImage) 
                safeZoneFillImage.fillAmount = 0f;
        }
    }
}

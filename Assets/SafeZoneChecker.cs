using UnityEngine;

public class SafeZoneChecker : MonoBehaviour
{
    public bool isPlayerInSafeZone;
    public bool isStretcherInSafeZone;
    public bool IsAllSafe => isPlayerInSafeZone && isStretcherInSafeZone;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerZoneCheck")) isPlayerInSafeZone = true;
        if (collision.CompareTag("Stretcher")) isStretcherInSafeZone = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerZoneCheck")) isPlayerInSafeZone = false;
        if (collision.CompareTag("Stretcher")) isStretcherInSafeZone = false;
    }
}
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // Takip edilecek oyuncu

    [Header("Camera Settings")]
    public float smoothSpeed = 0.125f; // Kameranın takip hızını belirler
    public Vector3 offset;             // Kameranın hedefe göre uzaklığı

    private void LateUpdate()
    {
        if (target == null) return;

        // Hedef pozisyonu
        Vector3 desiredPosition = target.position + offset;

        // Smooth hareket
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Kamerayı yeni pozisyona taşı
        transform.position = smoothedPosition;
    }
}

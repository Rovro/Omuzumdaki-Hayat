// Sniper2D.cs
using UnityEngine;

public class Sniper2D : MonoBehaviour
{
    public Transform spawnPoint;     // Inspector: mermi spawn noktasý
    public GameObject bulletPrefab;  // Inspector: bullet prefab
    public float bulletSpeed = 15f;

    public void Fire(Transform target)
    {
        if (bulletPrefab == null || spawnPoint == null || target == null) return;

        var go = Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity);
        var rb = go.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        Vector2 dir = ((Vector2)target.position - (Vector2)spawnPoint.position).normalized;
        rb.linearVelocity = dir * bulletSpeed;
    }

    // Kolay kullaným: player referansý yoksa sahnedeki "Player" tag'li objeye ateþ et
    public void FireAtPlayer()
    {
        var p = GameObject.FindWithTag("Player")?.transform;
        if (p != null) Fire(p);
    }
}

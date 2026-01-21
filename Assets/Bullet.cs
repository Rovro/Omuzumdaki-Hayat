using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime; // Merminin sahnede kalma süresi
    public int damage;     // Merminin vereceði hasar
    public GameObject hitEffect; // Çarpma efekti prefab'ý

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Stretcher"))
        {
            if (collision.TryGetComponent<PlayerHealth>(out var hp))
            {
                hp.TakeDamage(damage);
                Debug.Log(damage);
                Destroy(gameObject);
            }
        }
    }
}

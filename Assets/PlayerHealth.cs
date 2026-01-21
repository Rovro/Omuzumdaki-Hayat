using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100;
    public float currentHealth;
    public Image healthImageUI;
    public GameObject redOverlayUI;
    public PlayerInput playerInput;
    public bool isDead = false;
    public Animator animator;
    void Start()
    {
        currentHealth = maxHealth;     
    }
    private void Update()
    {
        if (isDead && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            //animator.SetBool("isDead", false);
        }
    }
    public void TakeDamage(float damage)
    {
        //animator.SetTrigger("Hit");
        currentHealth -= damage;
        healthImageUI.fillAmount -= damage/100f;
        Debug.Log(currentHealth);
        Debug.Log(healthImageUI.fillAmount);
        if (currentHealth <= 0) Die();
    }
    void Die()
    {
        isDead = true;
        playerInput.enabled = false;
        //animator.SetBool("isDead", true);
        FindAnyObjectByType<SimpleTextPop>().ShowText();
        if (redOverlayUI != null)
            redOverlayUI.SetActive(true);
        StartCoroutine(FadeOverlay(redOverlayUI.GetComponent<Image>(), 0.4f, 1f));
    }
    private IEnumerator FadeOverlay(Image img, float targetAlpha, float duration)
    {
        Color c = img.color;
        float startAlpha = c.a;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, t / duration);
            img.color = c;
            yield return null;
        }
    }
}

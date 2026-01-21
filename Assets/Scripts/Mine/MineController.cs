using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MineController : MonoBehaviour
{
    private Animator anim; 
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        anim.SetBool("isTrigger", true);
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerHealth>().TakeDamage(50);
            Destroy(gameObject);
            Debug.Log("Player hit a mine!");
        }
        if (collision.CompareTag("Stretcher"))
        {
            collision.GetComponent<PlayerHealth>().TakeDamage(50);
            Destroy(gameObject);
            Debug.Log("Children hit a mine!");
        }
    }
}


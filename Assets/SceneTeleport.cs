using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleport : MonoBehaviour
{
    [SerializeField] string nextLevelSceneName;
    [SerializeField] string requiredPlayerTag;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(requiredPlayerTag))
        {
            SceneManager.LoadScene(nextLevelSceneName);
        }
    }
}
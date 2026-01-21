using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels (Inspector'dan ata)")]
    public GameObject MainMenuPanel;       // Canvas/MainMenuPanel
    public GameObject Settings_Panel;      // Canvas/Settings_Panel
    public GameObject Credits_Panel;       // Canvas/Credits_Panel
    public GameObject quitConfirm_Panel;   // Canvas/quitConfirm_Panel

    [Header("Scene")]
    public string firstSceneName;

    //[Header("Optional")]
    // public AudioSource uiClickSfx; // (İstersen tekrar açarsın)

    void Awake()
    {
        SafeSetActive(MainMenuPanel, true);
        SafeSetActive(Settings_Panel, false);
        SafeSetActive(Credits_Panel, false);
        SafeSetActive(quitConfirm_Panel, false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            HandleEsc();
    }

    // -------- BUTTON CALLBACKS --------
    public void OnPlay()
    {
        // PlayClick();
        if (!string.IsNullOrEmpty(firstSceneName))
            SceneManager.LoadScene(firstSceneName);
        else
            Debug.LogError("[MainMenuController] firstSceneName boş!");
    }

    public void OnOpenSettings()
    {
        // PlayClick();
        SafeSetActive(MainMenuPanel, false);
        SafeSetActive(Credits_Panel, false);
        SafeSetActive(Settings_Panel, true);
    }

    public void OnOpenCredits()
    {
        // PlayClick();
        SafeSetActive(MainMenuPanel, false);
        SafeSetActive(Settings_Panel, false);
        SafeSetActive(Credits_Panel, true);
    }

    public void OnBackFromSettings()
    {
        // PlayClick();
        SafeSetActive(Settings_Panel, false);
        SafeSetActive(MainMenuPanel, true);
    }

    public void OnBackFromCredits()
    {
        // PlayClick();
        SafeSetActive(Credits_Panel, false);
        SafeSetActive(MainMenuPanel, true);
    }

    public void OnOpenQuitConfirm()
    {
        // PlayClick();
        SafeSetActive(quitConfirm_Panel, true); // Ana menü görünür kalabilir
    }

    public void OnQuitYes()
    {
        // PlayClick();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnQuitNo()
    {
        // PlayClick();
        SafeSetActive(quitConfirm_Panel, false);
    }

    // -------- HELPERS --------
    void HandleEsc()
    {
        if (quitConfirm_Panel && quitConfirm_Panel.activeSelf)
        {
            OnQuitNo();
            return;
        }

        if (Settings_Panel && Settings_Panel.activeSelf)
        {
            OnBackFromSettings();
            return;
        }
        if (Credits_Panel && Credits_Panel.activeSelf)
        {
            OnBackFromCredits();
            return;
        }

        if (MainMenuPanel && MainMenuPanel.activeSelf)
        {
            OnOpenQuitConfirm();
        }
    }

    void SafeSetActive(GameObject go, bool state)
    {
        if (go && go.activeSelf != state) go.SetActive(state);
    }

    // void PlayClick() { if (uiClickSfx) uiClickSfx.Play(); }
}

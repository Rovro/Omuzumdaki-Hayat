using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SettingsMenuController : MonoBehaviour
{
    [Header("Panel (Inspector'dan ata)")]
    public GameObject MainMenu_Panel;  // Canvas/Settings_Panel
    public GameObject Settings_Panel;

    void Awake()
    {
        SafeSetActive(MainMenu_Panel, false); // Baþlangýçta kapalý olsun
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Açýk ise kapat, kapalý ise aç
            SafeSetActive(Settings_Panel, false);
            SafeSetActive(MainMenu_Panel, !MainMenu_Panel.activeSelf);
        }
    }

    void SafeSetActive(GameObject go, bool state)
    {
        if (go && go.activeSelf != state)
            go.SetActive(state);

        Time.timeScale = state ? 0f : 1f; // Menü açýkken oyunu durdur, kapalýyken devam ettir
    }




    public void BackButton()
    {
        SafeSetActive(Settings_Panel, false);
        SafeSetActive(MainMenu_Panel, true);
    }

    public void SettingsPanel()
    {
        SafeSetActive(MainMenu_Panel, false);
        SafeSetActive(Settings_Panel, true);
    }
    public void ResumeButton()
    {
        SafeSetActive(MainMenu_Panel, false);
    }
    public void QuitButton()
    {
        Application.Quit();
    }


}

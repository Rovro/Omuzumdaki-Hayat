using UnityEngine;

public class PlayerLightSkill : MonoBehaviour
{
    public GameObject soldierLight;
    public GameObject stretcherLight;
    public Animator animator;
    public string animBool = "IsOn";

    bool isOn;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isOn = !isOn;

            if (soldierLight)
                soldierLight.SetActive(isOn);
            if (stretcherLight)
                stretcherLight.SetActive(isOn);
            if (animator) 
                animator.SetBool("IsOn", isOn); 
        }
    }
}

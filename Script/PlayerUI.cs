using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Image m_playerStaminaBar;

    public void SetStaminaBar(float currentStamina )
    {
        m_playerStaminaBar.fillAmount = currentStamina * 0.01f;
    }
}

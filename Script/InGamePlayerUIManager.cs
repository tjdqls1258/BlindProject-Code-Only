using UnityEngine;

public class InGamePlayerUIManager : MonoBehaviour
{
    [SerializeField] private PlayerUI m_playerUI;
    [SerializeField] private IntercationUI m_interactionUI;

    public void Run(float currentStamina)
    {
        m_playerUI.SetStaminaBar(currentStamina);
    }
}

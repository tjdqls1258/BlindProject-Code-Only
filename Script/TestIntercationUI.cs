using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestIntercationUI : MonoBehaviour
{
    [SerializeField] private Image m_interactionBar;
    [SerializeField] private TextMeshProUGUI m_text;
    [SerializeField] private InteractionController m_interactionController;

    private void Awake()
    {
        m_interactionController.InitInteractionUI(m_interactionBar, m_text);
    }

}

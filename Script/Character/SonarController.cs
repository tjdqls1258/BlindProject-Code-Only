using UnityEngine;

public class SonarController : MonoBehaviour
{
    private SonarManager sonarManager => SonarManager.Instance;

    [SerializeField] private float m_power;
    [SerializeField] private LayerMask m_layMask;

    [SerializeField] private Color m_sonarColor = Color.red;

    private void Awake()
    {
        m_sonarColor.a = m_power;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            m_sonarColor = ColorUtil.GetRandomColor();
            m_sonarColor.a = m_power;
            Debug.Log(m_sonarColor);
            sonarManager.SonarAdd(transform.position, m_sonarColor, m_layMask);
        }
    }
}

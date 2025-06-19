using UnityEngine;

public class AutoSonarMainMenu : MonoBehaviour
{
    private SonarManager sonarManager => SonarManager.Instance;
    [SerializeField] private LayerMask m_layMask;
    [SerializeField] float m_autotime = 5;
    float m_currenttime = 0;
    Color m_sonarColor = Color.white;
    private void Update()
    {
        m_currenttime += Time.deltaTime;
        if (m_autotime < m_currenttime)
        {
            m_currenttime = 0;
            m_sonarColor.a = 20;
            Debug.Log(m_sonarColor);
            sonarManager.SonarAdd(transform.position, m_sonarColor, m_layMask);
        }
    }
}

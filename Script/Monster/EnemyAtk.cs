using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyAtk : MonoBehaviour
{
    private Transform m_traget;
    private float m_range = 3f;

    public void SetTarget(Transform target)
    {
        if (target == null) return;
        m_traget = target;
    }

    public void PlayerAtk(Transform Target)
    {
        SetTarget(Target);
        if (m_traget == null) return;
        float distance = Vector3.Distance(m_traget.transform.position, transform.position);

        m_traget.GetComponent<PlayerController_FSM>().PlayerDie();
    }
}

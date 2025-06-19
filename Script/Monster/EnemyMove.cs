using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MonsterAnimationState))]
public class EnemyMove : MonoBehaviour
{
    [Header("공격 관련")]
    [SerializeField] private EnemyAtk enemyAtk;

    [Space,Header("이동 관련")]
    [SerializeField] private NavMeshAgent m_agent;
    [SerializeField] private AreaManager m_areaManager;

    private bool m_wait = false;

    private MonsterAnimationState _animationState;
    private MonsterAnimationState animationState
    {
        get
        {
            if (_animationState == null)
            {
                _animationState = GetComponent<MonsterAnimationState>();
            }
            return _animationState;
        }
    }

    private void Start()
    {
        if (RandomPoint(out Vector3 hitpoint))
        {
            Debug.DrawRay(hitpoint, Vector3.up, Color.blue, 1f);
            Move(hitpoint);
        }
    }

    private void Update()
    {
        if (m_serchTarget) return;
        if((m_agent.remainingDistance <= m_agent.stoppingDistance) && m_wait == false)
        {
            m_wait = true;
            WaitSerchNew().Forget();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        CheckPlayerInDistance(transform.position, m_distance);
    }

    #region 랜덤 이동
    private bool RandomPoint(out Vector3 result)
    {
        if (m_agent.remainingDistance > m_agent.stoppingDistance)
        {
            result = Vector3.zero;
            //animationState.DoAnimation(MonsterAnimationState.AnimationState.Idle, 0);
            return false;
        }
        Vector3 randomPoint = m_areaManager.RandomAreaPosition();

        NavMeshHit hit;
        if(NavMesh.SamplePosition(randomPoint,out hit, 1, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }
    #endregion

    #region 시야각 추적
    [Header("플레이어 추적 시야각")]
    [SerializeField, Range(0,360)] private float m_viewAngle = 0f;
    [SerializeField] private float m_viewRadius = 1;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask otherMask;
    [SerializeField] private Transform m_target = null;
    [SerializeField] private float m_reserchTime = 3;
    [SerializeField] private float m_atkDistance = 1;
    
    float m_distance = 5f;

    Collider[] target;
    private bool m_serchTarget = false;

    private void CheckPlayerInDistance(Vector3 Center, float serchDistance)
    {
        if (m_serchTarget) return;
        float look = transform.eulerAngles.y;
        Vector3 lookDir = angletoDir(look);
        
        target = Physics.OverlapSphere(Center, m_viewRadius, targetMask);
        if (target.Length <= 0) return;

        Vector3 targetPos;
        Vector3 targetDir;
        float targetAngle;
        foreach (Collider c in target)
        {
            targetPos = c.transform.position;
            targetDir = (targetPos - Center).normalized;
            targetAngle = Mathf.Acos(Vector3.Dot(lookDir, targetDir)) * Mathf.Rad2Deg;
            if(targetAngle <= m_viewAngle * 0.5f && !Physics.Raycast(Center, targetDir, m_viewRadius, otherMask))
            {
                m_target = c.transform;
                m_serchTarget = true;
                Debug.Log("Find");
                TargetTracking().Forget();
                break;
            }
        }

        Vector3 angletoDir(float angle)
        {
            float radian = angle * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
        }
    }
        
    private async UniTask TargetTracking()
    {
        if (m_serchTarget)
        {
            float distance;
            float currentTime = 0;
            while(currentTime < m_reserchTime)
            {
                distance = Vector3.Distance(m_target.position, transform.position);
                Move(m_target.transform.position);
                await UniTask.WaitForFixedUpdate();
                if (distance <= m_atkDistance)
                {
                    enemyAtk.PlayerAtk(m_target);
                    animationState.DoAnimation(MonsterAnimationState.AnimationState.Attack);
                    m_target = null;
                    m_serchTarget = false;
                    currentTime = m_reserchTime - 1;
                }
                if(distance > m_distance)
                    currentTime += Time.fixedDeltaTime;
                else
                    currentTime = 0;
                
            }

            Debug.Log("ReSerch");
            m_serchTarget = false;
            
        }
    }
    #endregion

    #region 

    public void HitSonar(Vector3 hitPos)
    {
        if (m_serchTarget) return;

        Debug.Log("Sonar Hit");
        Move(hitPos);
        Debug.Log(hitPos);
    }

    private void Move(Vector3 hitPos)
    {
        m_agent.SetDestination(hitPos);
        animationState.DoAnimation(MonsterAnimationState.AnimationState.Move, 1);
    }

    private async UniTask WaitSerchNew()
    {
        animationState.DoAnimation(MonsterAnimationState.AnimationState.Idle, 0);
        await UniTask.WaitForSeconds(3f);

        if (m_serchTarget) return;
        if (RandomPoint(out Vector3 hitpoint))
        {
            Debug.DrawRay(hitpoint, Vector3.up, Color.blue, 1f);
            Move(hitpoint);
        }

        await UniTask.WaitForFixedUpdate();
        m_wait = false;
    }
    #endregion
}

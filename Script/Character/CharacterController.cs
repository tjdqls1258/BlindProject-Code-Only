using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    InteractionController interactionController;
    [SerializeField] private NavMeshAgent m_agent;
    [SerializeField] private Camera m_characterCamera;
    [SerializeField] private Transform m_characterHaedPos;
    private Animator m_animator;
    [SerializeField] float m_fixCameraPos;

    private bool m_playerDie = false;

    private Animator Animator
    {
        get
        {
            if(m_animator == null)
            {
                m_animator = GetComponentInChildren<Animator>();
            }

            return m_animator;  
        }
    }

    [Header("Player Setting")]
    [SerializeField] private float m_turnSpeed;
    [SerializeField] private float m_xRotate;
    [SerializeField] private float m_moveSpeed;
    [SerializeField] private Vector2 m_rotateMinMax;

    private bool m_isMine = true;
    private Vector3 dir;

    private void Awake()
    {
        interactionController = GetComponent<InteractionController>();
        transform.rotation = Quaternion.identity;
        m_agent.updateRotation = false;
        m_agent.updateUpAxis = false;
    }

    public void InitCharacter(bool isMine)
    {
        m_isMine = isMine;
        if(isMine)
            m_characterCamera = Camera.main;    
    }

    private void Update()
    {
        if(m_isMine)
            Move_Do(dir,transform.rotation);
    }

    public void OnLook(InputValue Look)
    {
        if (m_playerDie) return;
        if (interactionController.GetDoInterAction()) return;

        var mouse = Look.Get<Vector2>();
        float yRotateSize = mouse.x * m_turnSpeed;
        float yRotate = transform.eulerAngles.y + yRotateSize;

        float xRotateSzie = -mouse.y * m_turnSpeed;
        m_xRotate = Mathf.Clamp(m_xRotate + xRotateSzie, m_rotateMinMax.x, m_rotateMinMax.y);
        transform.eulerAngles = new Vector3(0, yRotate, 0);
        m_characterCamera.transform.eulerAngles = new Vector3(m_xRotate, m_characterCamera.transform.eulerAngles.y, m_characterCamera.transform.eulerAngles.z);
    }

    public void OnMove(InputValue Move)
    {
        if (m_playerDie) return;

        dir = Move.Get<Vector2>();
        dir = dir.normalized;
    }

    public void Move_Do(Vector3 dir, Quaternion quaternion)
    {
        if (m_playerDie) return;

        m_characterCamera.transform.position = m_characterHaedPos.position + (m_characterCamera.transform.forward * m_fixCameraPos);
        if (interactionController.GetDoInterAction()) return;
        
        if (m_isMine == false)
        {
            transform.rotation = quaternion;
            Debug.Log("Set Rotate");
        }
        if(dir.sqrMagnitude <= 0)
        {
            Animator.SetFloat("MoveF", 0);
            Animator.SetFloat("MoveL", 0);
            return;
        }
        else if(Input.GetKey(KeyCode.LeftShift))
        {
            if (dir.y != 0)
                Animator.SetFloat("MoveF", dir.y > 0 ? 1f : -1f);
            else Animator.SetFloat("MoveF", 0);

            if (dir.x != 0)
                Animator.SetFloat("MoveL", dir.x > 0 ? -1f : 1f);
            else Animator.SetFloat("MoveL", 0);

            m_agent.Move((transform.forward * dir.y +
                    (transform.right * dir.x)) * m_moveSpeed);
        }
        else
        {
            if (dir.y != 0)
                Animator.SetFloat("MoveF", dir.y > 0 ? 0.5f : -0.5f);
            else Animator.SetFloat("MoveF", 0);

            if (dir.x != 0)
                Animator.SetFloat("MoveL", dir.x > 0 ? -0.5f : 0.5f);
            else Animator.SetFloat("MoveL", 0);

            m_agent.Move((transform.forward * dir.y +
                    (transform.right * dir.x)) * m_moveSpeed * 0.5f);
        }
    }

    public void Die()
    {
        Debug.Log("Player Die");
        m_playerDie = true;
        m_agent.enabled = false;
        GetComponent<Collider>().enabled = false;
    }
}

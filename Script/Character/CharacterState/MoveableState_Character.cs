using Cysharp.Threading.Tasks;
using StateMachine;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[Serializable]
public class PlayerStateTools
{
    public NavMeshAgent m_agent;
    public InteractionController m_interaction;
    public Camera m_characterCamera;
    public Transform m_characterTransform;
    public Transform m_characterHeadPos;
    public Animator m_animator;
    public float m_fixCameraPos;
    public bool m_playerDie;
    public bool isMine;
    public float m_turnSpeed;
    public float m_xRotate;
    public float m_moveSpeed;
    public Vector2 m_rotateMinMax;
    public Vector2 dir;
}
public class MoveableState_Character : MonoBehaviour ,I_State<PlayerStateTools>
{
    private PlayerStateTools playerStateTools;
    private Transform m_characterHeadPos;
    private bool m_isMine = true;
    private Animator m_animator;

    public bool CheckExit()
    {
        return playerStateTools.m_playerDie;
    }

    public async UniTask Enter()
    {
        
    }

    public void Excute()
    {
        if(m_isMine)
            Move_Do(playerStateTools.dir, playerStateTools.m_characterTransform.rotation);
    }

    public async UniTask Exit()
    {
        if(playerStateTools.m_playerDie)
        {
            playerStateTools.m_animator.SetBool("Die", playerStateTools.m_playerDie);
        }
    }

    public Type GetNextType()
    {
        throw new NotImplementedException();
    }

    public void Init(PlayerStateTools initData)
    {
        playerStateTools = initData;
        m_isMine = initData.isMine;
    }

    #region Input System
    public void OnMove(InputValue Move)
    {
        if (playerStateTools.m_playerDie) return;

        playerStateTools.dir = Move.Get<Vector2>();
        playerStateTools.dir = playerStateTools.dir.normalized;
    }

    public void OnLook(InputValue Look)
    {
        if (playerStateTools.m_playerDie) return;
        if (playerStateTools.m_interaction.GetDoInterAction()) return;

        var mouse = Look.Get<Vector2>();
        float yRotateSize = mouse.x * playerStateTools.m_turnSpeed;
        float yRotate = playerStateTools.m_characterTransform.eulerAngles.y + yRotateSize;

        float xRotateSzie = -mouse.y * playerStateTools.m_turnSpeed;
        playerStateTools.m_xRotate = Mathf.Clamp(playerStateTools.m_xRotate + xRotateSzie, playerStateTools.m_rotateMinMax.x, playerStateTools.m_rotateMinMax.y);
        playerStateTools.m_characterTransform.eulerAngles = new Vector3(0, yRotate, 0);
        playerStateTools.m_characterCamera.transform.eulerAngles = new Vector3(playerStateTools.m_xRotate, playerStateTools.m_characterCamera.transform.eulerAngles.y, playerStateTools.m_characterCamera.transform.eulerAngles.z);
    }

    public void Move_Do(Vector3 dir, Quaternion quaternion)
    {
        playerStateTools.m_characterCamera.transform.position = playerStateTools.m_characterHeadPos.position + (playerStateTools.m_characterTransform.forward * playerStateTools.m_fixCameraPos);
        if (playerStateTools.m_interaction.GetDoInterAction()) return;

        if (m_isMine == false)
        {
            playerStateTools.m_characterTransform.rotation = quaternion;
            Debug.Log("Set Rotate");
        }
        if (dir.sqrMagnitude <= 0)
        {
            playerStateTools.m_animator.SetFloat("MoveF", 0);
            playerStateTools.m_animator.SetFloat("MoveL", 0);
            return;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            if (dir.y != 0)
                playerStateTools.m_animator.SetFloat("MoveF", dir.y > 0 ? 1f : -1f);
            else playerStateTools.m_animator.SetFloat("MoveF", 0);

            if (dir.x != 0)
                playerStateTools.m_animator.SetFloat("MoveL", dir.x > 0 ? -1f : 1f);
            else playerStateTools.m_animator.SetFloat("MoveL", 0);

            playerStateTools.m_agent.Move((playerStateTools.m_characterTransform.forward * dir.y +
                    (playerStateTools.m_characterTransform.right * dir.x)) * playerStateTools.m_moveSpeed);
        }
        else
        {
            if (dir.y != 0)
                playerStateTools.m_animator.SetFloat("MoveF", dir.y > 0 ? 0.5f : -0.5f);
            else playerStateTools.m_animator.SetFloat("MoveF", 0);

            if (dir.x != 0)
                playerStateTools.m_animator.SetFloat("MoveL", dir.x > 0 ? -0.5f : 0.5f);
            else playerStateTools.m_animator.SetFloat("MoveL", 0);

            playerStateTools.m_agent.Move((playerStateTools.m_characterTransform.forward * dir.y +
                    (playerStateTools.m_characterTransform.right * dir.x)) * playerStateTools.m_moveSpeed * 0.5f);
        }
    }
    #endregion
}

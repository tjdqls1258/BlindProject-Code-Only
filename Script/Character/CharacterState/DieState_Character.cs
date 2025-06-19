using Cysharp.Threading.Tasks;
using StateMachine;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class DieState_Character : I_State<PlayerStateTools>
{
    PlayerStateTools playerStateTools;
    bool m_isMine;

    public bool CheckExit()
    {
        if (playerStateTools.m_playerDie == false)
            return true;
        return false;
    }

    public async UniTask Enter()
    {
        playerStateTools.m_animator.SetBool("Die", true);
    }

    public void Excute()
    {

    }

    public async UniTask Exit()
    {
        //다시 일어나는 애니메이션 재생
    }

    public Type GetNextType()
    {
        return typeof(MoveableState_Character);
    }

    public void Init(PlayerStateTools initData)
    {
        playerStateTools = initData;
        m_isMine = initData.isMine;
    }
}

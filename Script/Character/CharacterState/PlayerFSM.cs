using Cysharp.Threading.Tasks;
using StateMachine;
using UnityEngine;

public class PlayerFSM : StateMachine.StateMachine<PlayerStateTools>
{
    private PlayerStateTools statetool;

    protected override PlayerStateTools m_getBaseData()
    {
        return statetool;
    }

    public override void AddState(I_State<PlayerStateTools> state)
    {
        base.AddState(state);
    }

    public override UniTask Init(I_State<PlayerStateTools> enterState, PlayerStateTools data)
    {
        statetool = data;
        return base.Init(enterState, data);
    }

    public override UniTask Run()
    {
        return base.Run();
    }

    public override void StopState()
    {
        base.StopState();
    }

    protected override UniTask ChangeState()
    {
        return base.ChangeState();
    }
}

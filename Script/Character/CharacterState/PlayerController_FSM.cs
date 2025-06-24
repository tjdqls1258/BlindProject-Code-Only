using Cysharp.Threading.Tasks;
using StateMachine;
using System.Threading.Tasks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController_FSM : MonoBehaviour
{
    [SerializeField] private PlayerStateTools statetool;
    [SerializeField] private MoveableState_Character mainState;
    private PlayerFSM fsm = new();

    private void Awake()
    {
        mainState = GetComponent<MoveableState_Character>();
        if(mainState == null)
        {
            mainState = gameObject.AddComponent<MoveableState_Character>();    
        }
        help().Forget();
        async UniTaskVoid help()
        {
            await fsm.Init(mainState, statetool);
            fsm.AddState(new DieState_Character());
            fsm.Run().Forget();
        }
    }

    public void PlayerDie()
    {
        Debug.Log("Player Die");
        statetool.m_playerDie = true;
        statetool.m_agent.enabled = false;
        GetComponent<Collider>().enabled = false;
    }
}

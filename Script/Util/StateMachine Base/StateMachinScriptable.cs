using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace StateMachine
{
    [Serializable]
    public struct State<T>
    {
        public I_State<T> state;
        public bool isEnterState;
    }
    public class StateMachinScriptable<T> : ScriptableObject
    {
        [SerializeField] private List<State<T>> m_stateList;
        private I_State<T> m_enterState, m_currentState;
        private Dictionary<Type, I_State<T>> stateMachine = new();
        private bool isRund = false;

        public virtual void AddState(I_State<T> state)
        {
            if (stateMachine.ContainsKey(state.GetType()) == false)
                stateMachine.Add(state.GetType(), state);
        }

        public virtual async UniTask Init(I_State<T> enterState)
        {
            foreach (var state in m_stateList)
            {
                AddState(state.state);
                if (state.isEnterState && m_currentState == null)
                {
                    m_currentState = m_enterState = state.state;
                }
            }
            
            if(m_currentState != null)
                await m_currentState.Enter();
        }

        public virtual async UniTask Run()
        {
            while (isRund)
            {
                m_currentState.Excute();
                await UniTask.WaitForFixedUpdate();
                if (m_currentState.CheckExit())
                {
                    m_currentState.Exit();
                    Type next = m_currentState.GetNextType();

                    if (stateMachine.ContainsKey(m_currentState.GetNextType()) && next != null)
                        m_currentState = stateMachine[next];
                    else
                        StopState();
                }
            }
        }

        public virtual void StopState()
        {
            isRund = false;
        }
    }
}
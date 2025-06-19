using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace StateMachine
{
    public interface I_State<T>
    {
        public void Init(T initData);
        public Type GetNextType();
        public UniTask Enter();
        public void Excute();
        public bool CheckExit();
        public UniTask Exit();
    }

    public interface I_StateMachine<T>
    {
        public UniTask Init(I_State<T> enterState, T data);
        public void AddState(I_State<T> state);
        public UniTask Run();
        public void StopState();
    }

    public abstract class StateMachine<T> : I_StateMachine<T>
    {
        protected I_State<T> m_enterState, m_currentState;
        protected Dictionary<Type, I_State<T>> stateMachine = new();
        protected bool isRun = false;
        protected abstract T m_getBaseData();

        public virtual void AddState(I_State<T> state)
        {
            if (stateMachine.ContainsKey(state.GetType()) == false)
            {
                stateMachine.Add(state.GetType(), state);
                state.Init(m_getBaseData());
            }
        }

        public virtual async UniTask Init(I_State<T> enterState, T data)
        {
            AddState(enterState);
            m_currentState = m_enterState = enterState;
            m_currentState.Init(m_getBaseData());
            await m_currentState.Enter();
        }

        public virtual async UniTask Run()
        {
            isRun = true;
            while (isRun)
            {
                m_currentState.Excute();
                await UniTask.WaitForFixedUpdate();
                if (m_currentState.CheckExit())
                {
                    await m_currentState.Exit();

                    await ChangeState();
                }
            }
        }

        public virtual void StopState()
        {
            isRun = false;
        }

        protected virtual async UniTask ChangeState()
        {
            Type type = m_currentState.GetNextType();

            if (stateMachine.ContainsKey(m_currentState.GetNextType()) && type != null)
            {
                m_currentState = stateMachine[type];
                await m_currentState.Enter();
            }
        }
    }
}
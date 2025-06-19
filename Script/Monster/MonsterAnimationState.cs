using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimationState : MonoBehaviour
{
    protected Animator _anim;

    public enum AnimationState
    {
        Idle,
        Attack,
        Move,
        Roar
    }
    
    public Animator myAnim 
    {
        get
        {
            if (_anim == null)
                _anim = GetComponentInChildren<Animator>();

            Debug.Assert( _anim, "Animation Null" );

            return _anim;
        }
    }

    private Dictionary<AnimationState, int> animationState = new();

    public void DoAnimation(AnimationState state)
    {
        CheckState(state);

        myAnim.SetTrigger(animationState[state]);
    }

    public void DoAnimation(AnimationState state, float value)
    {
        AnimationState s = state;
        if (s == AnimationState.Idle)
            s = AnimationState.Move;

        CheckState(s);

        myAnim.SetFloat(animationState[s], value);
    }

    public void DoAnimation(AnimationState state, bool value)
    {
        AnimationState s = state;
        if (s == AnimationState.Idle)
            s = AnimationState.Move;

        CheckState(s);

        myAnim.SetBool(animationState[s], value);
    }

    private void CheckState(AnimationState state)
    {
        if (animationState.ContainsKey(state) == false)
        {
            string s = state.ToString();
            animationState.Add(state, Animator.StringToHash(s));
        }
    }
}

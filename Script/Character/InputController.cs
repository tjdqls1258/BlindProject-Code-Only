using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public event Action<Vector2> OnMove;
    public event Action<Vector2> OnLook;

    public void CallMoveEvent(Vector2 dir)
    {
        OnMove?.Invoke(dir);
    }

    public void CallMouseMoveEvent(Vector2 pos)
    {
        OnLook?.Invoke(pos);
    }

}

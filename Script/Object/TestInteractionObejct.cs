using DG.Tweening;
using UnityEngine;

public class TestInteractionObejct : MonoBehaviour, I_InteractionObejct
{
    [SerializeField] private LayerMask mask;
    [SerializeField] private float interactionTime = 3;
    bool isOpen = false;
    bool isPressed = false;
    public float InteractionTime() => interactionTime;

    private void Awake()
    {
        Init();
    }

    public void OnInteraction(params object[] obj)
    {
        Debug.Log($"Interaction {gameObject.name} {isPressed}");
        if (isPressed) return;

        isPressed = true;
        transform.DOLocalMoveY(isOpen ? -50f : 50f, 2f).OnComplete(()=>isPressed = false);
    }

    public void Init()
    {
        // gameObject.layer = mask;
    }

    public InteractionType interactionType()
    {
        return InteractionType.InterActionDoor;
    }

    public string GetInteractionText()
    {
        return "Open....";
    }

    public string GetCastText()
    {
        return "Try Open";
    }
}

using UnityEngine;

public enum InteractionType
{
    None = -1,
    InterActionDoor,
    Plant,

}

public interface I_InteractionObejct
{
    public void Init();
    public void OnInteraction(params object[] obj);
    public float InteractionTime();
    public InteractionType interactionType();
    public string GetInteractionText();
    public string GetCastText();
}

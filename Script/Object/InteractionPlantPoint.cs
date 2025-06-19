using UnityEngine;

public class InteractionPlantPoint : MonoBehaviour, I_InteractionObejct
{
    public string GetCastText()
    {
        return "Do Plant";
    }

    public string GetInteractionText()
    {
        return "Planting...";
    }

    public void Init()
    {
        
    }

    public float InteractionTime()
    {
        return 5;
    }

    public InteractionType interactionType()
    {
        return InteractionType.Plant;
    }

    public void OnInteraction(params object[] obj)
    {
        Debug.Log("Plant Succcec");
        GetComponent<Collider>().enabled = false;


        QuestManager.Instance.QuestBorad(Quest.Q_Kind.Install, Quest.Q_Detail.None, 1);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }
}

using UnityEngine;

public class MasterManager : MonoSingleton<MasterManager>
{
    private QuestManager _questManger;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Init()
    {
        _questManger = QuestManager.Instance;

        ManagerInit();
    }

    private void ManagerInit()
    {
        _questManger.Init();
    }
}

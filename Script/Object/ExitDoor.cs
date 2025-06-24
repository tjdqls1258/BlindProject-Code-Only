using Scene;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(QuestManager.Instance.CheckClearGuestAll())
            SceneLoadManager.Instance.SceneLoad(SceneInfo.SceneType.Home);

    }
}

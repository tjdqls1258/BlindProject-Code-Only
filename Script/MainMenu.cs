using Scene;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button TestButton;
    void Start()
    {
        TestButton.onClick.RemoveAllListeners();
        TestButton.onClick.AddListener(async () =>
        {
            TestButton.enabled = false;
            await SceneLoadManager.Instance.SceneLoad(SceneInfo.SceneType.InGame);
        });
    }

}

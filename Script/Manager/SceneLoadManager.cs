using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scene
{
    public class SceneLoadManager : MonoSingleton<SceneLoadManager>
    {
        [SerializeField] private GameObject m_loadingImage;
        [SerializeField] private TextMeshProUGUI m_text;
        public float m_currentProgress = 0;
        public float CurrentSceneLoadprogress
        {
            private set;
            get;
        }

        public async UniTask SceneLoad(SceneInfo.SceneType type)
        {
            m_currentProgress = 0;
            string SceneName = SceneInfo.GetSceneName(type);
            if (string.IsNullOrEmpty(SceneName))
            {
                return;
            }
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneName);
            m_loadingImage.SetActive(true);
            while (!asyncOperation.isDone)
            {
                m_text.text = m_currentProgress.ToString("%");
                m_currentProgress = asyncOperation.progress;
                await UniTask.WaitForEndOfFrame();
            }
            m_loadingImage.SetActive(false);
        }
    }
}
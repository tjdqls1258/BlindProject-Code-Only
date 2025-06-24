using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scene
{
    public class SceneLoadManager : MonoSingleton<SceneLoadManager>
    {
        [SerializeField] private GameObject m_loadingImage;
        [SerializeField] private TextMeshProUGUI m_text;
        private CanvasGroup m_fadeGroup;
        private float m_currentProgress = 0;
        private float m_fadeTime = 1;
        public float CurrentSceneLoadprogress
        {
            private set;
            get;
        }

        protected override void Init()
        {
            base.Init();
            m_fadeGroup = m_loadingImage.GetComponent<CanvasGroup>();
        }

        public async UniTask SceneLoad(SceneInfo.SceneType type)
        {
            m_currentProgress = 0;
            string SceneName = SceneInfo.GetSceneName(type);
            if (string.IsNullOrEmpty(SceneName))
            {
                return;
            }
           
            m_loadingImage.SetActive(true);
            m_fadeGroup.DOFade(1, m_fadeTime);
            await UniTask.WaitForSeconds(m_fadeTime);

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneName);
            while (!asyncOperation.isDone)
            {
                m_text.text = $"{m_currentProgress * 100}%";
                Debug.Log($"{m_currentProgress}");
                m_currentProgress = asyncOperation.progress;
                await UniTask.WaitForFixedUpdate();
            }
            m_fadeGroup.DOFade(0, m_fadeTime);
            await UniTask.WaitForSeconds(m_fadeTime);
            m_loadingImage.SetActive(false);
        }
    }
}
using UnityEngine;

namespace Scene
{
    public class SceneInfo
    {
        public enum SceneType
        {
            Home,
            InGame,
        }
        public static string GetSceneName(SceneType scene) => scene switch
        {
            SceneType.Home => "MainUI",
            SceneType.InGame => "Stage1",
            _ => "",
        };

    }
}

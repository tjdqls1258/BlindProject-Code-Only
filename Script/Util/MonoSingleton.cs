using Unity.VisualScripting;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject();
                go.name = $"(Singleton){typeof(T).Name}";
                _instance = go.AddComponent<T>();
            }
            return _instance;
        }
    }

    protected virtual void Init() 
    {
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void Awake() 
    {
        Init();
    }
}

public class Singleton<T> where T : class, new()
{
    protected static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }
    }

    public virtual void Init() { }
}

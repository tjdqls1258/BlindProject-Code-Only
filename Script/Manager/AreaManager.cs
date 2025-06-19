using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(AreaManager))]
[CanEditMultipleObjects]
public class AreaManagerEdit : Editor
{
    SerializedProperty manager;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        AreaManager manager = (AreaManager)target;

        for (int i = 0; i < manager.area.Length; i++)
        {
            manager.area[i].leftTopFoward = Handles.PositionHandle(manager.area[i].Center + manager.area[i].leftTopFoward, Quaternion.identity) - manager.area[i].Center;
            manager.area[i].rightBottonBack = Handles.PositionHandle(manager.area[i].Center + manager.area[i].rightBottonBack, Quaternion.identity) - manager.area[i].Center;
            manager.area[i].Center = Handles.PositionHandle(manager.area[i].Center, Quaternion.identity);
        }
    }

}

#endif


public class AreaManager : MonoBehaviour
{
    [SerializeField] private BaseArea[] m_area;

    private void Awake()
    {
        for (int i = 0; i < m_area.Length; i++)
        {
            m_area[i] = InitArea(m_area[i]);
        }

        BaseArea InitArea(BaseArea area)
        {
            switch (area.m_type)
            { 
                case BaseArea.AreaType.Box:
                    BoxArea boxArea = new BoxArea();
                    boxArea.CreateArea(area);
                    return boxArea;
                default:
                    return area;
            }
        }
    }

    public Vector3 RandomAreaPosition()
    {
        int RandomIndex = Random.Range(0, m_area.Length);

        return m_area[RandomIndex].GetRandomPosition();
    }
#if UNITY_EDITOR
    public BaseArea[] area => m_area;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0, 0, 0.5f);
        for (int i = 0; i < area.Length; i++)
        {
            Gizmos.DrawCube(
            area[i].Center +
            new Vector3(
            (area[i].rightBottonBack.x + (area[i].leftTopFoward.x - area[i].rightBottonBack.x) * 0.5f),
            (area[i].rightBottonBack.y + (area[i].leftTopFoward.y - area[i].rightBottonBack.y) * 0.5f),
            (area[i].rightBottonBack.z + (area[i].leftTopFoward.z - area[i].rightBottonBack.z) * 0.5f)),
            new Vector3(
            (area[i].leftTopFoward.x - area[i].rightBottonBack.x),
            (area[i].leftTopFoward.y - area[i].rightBottonBack.y),
            (area[i].leftTopFoward.z - area[i].rightBottonBack.z)));
        }
    }
#endif
}

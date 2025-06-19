using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[System.Serializable]
public class BaseArea
{
    public enum AreaType
    {
        None = 0,
        Box,
        End,
    }
    public AreaType m_type;
    public Vector3 m_center;
    public Vector3 m_leftTopPosition;
    public Vector3 m_rightBottonPosition;

    public virtual Vector3 GetRandomPosition() 
    {
        return Vector3.zero; 
    }

    public virtual BaseArea CreateArea(BaseArea baseData)
    {
        m_type = baseData.m_type;
        m_center = baseData.m_center;
        m_leftTopPosition = baseData.m_leftTopPosition;
        m_rightBottonPosition = baseData.m_rightBottonPosition;

        return this;
    }

#if UNITY_EDITOR
    [HideInInspector]
    public Vector3 Center
    {
        get => m_center;
        set => m_center = value;
    }

    [HideInInspector] public Vector3 leftTopFoward
    {
        get => m_leftTopPosition;
        set
        {
            m_leftTopPosition = value;
        }
    }

    [HideInInspector] public Vector3 rightBottonBack
    {
        get => m_rightBottonPosition;
        set
        {
            m_rightBottonPosition = value;
        }
    }
#endif
}

public class BoxArea : BaseArea
{
    public override Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(Center.x + m_leftTopPosition.x, Center.x + m_rightBottonPosition.x), Center.y,  (Random.Range(Center.z + m_leftTopPosition.z, Center.z + m_rightBottonPosition.z)));
    }
}
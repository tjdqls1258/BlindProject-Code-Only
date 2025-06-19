using FancyScrollView;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem
{
    public int roomID;
    public string roomName;
    public string roomMaster;
}

public class NullContext : FancyScrollRectContext
{
}

public class RoomCell : FancyScrollRectCell<RoomItem, NullContext>
{
    [SerializeField] private TextMeshProUGUI m_Text;
    [SerializeField] private Button btn;
    private RoomItem currentData;
    private void Awake()
    {
        btn.onClick.RemoveListener(OnClick);
        btn.onClick.AddListener(OnClick);
    }

    public override void UpdateContent(RoomItem itemData)
    {
        currentData = itemData;
        m_Text.text = itemData.roomName;
    }

    protected override void UpdatePosition(float normalizedPosition, float localPosition)
    {
        base.UpdatePosition(normalizedPosition, localPosition);
    }

    private void OnClick()
    {
        Debug.Log($"OnClick {currentData.roomName}");
    }
}

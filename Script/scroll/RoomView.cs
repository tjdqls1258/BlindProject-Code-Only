using FancyScrollView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomView : FancyScrollRect<RoomItem, NullContext>
{
    [SerializeField] float cellSize = 100;
    [SerializeField] Scroller scroller = new Scroller();
    protected override GameObject CellPrefab => itemCell;

    protected override float CellSize => cellSize;

    [SerializeField] private GameObject itemCell;

    protected override void Initialize()
    {
        base.Initialize();
    }

    public void UpdateData(IList<RoomItem> items)
    {
        base.UpdateContents(items);
        scroller.SetTotalCount(items.Count);
    }
}

using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private RoomView roomView;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<RoomItem> list = new();
        for(int i = 0;i < 50; i++)
        {
            list.Add(new RoomItem() { roomName = i.ToString()});
        }

        roomView.UpdateData(list);
    }
}

using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Objects/Quest")]
public class Quest : ScriptableObject, IUpdateDataFormSheet
{
    public int id;
    public enum Q_Kind
    {
        None = -1,
        Get,
        Install,
        Ues,
        End
    }
    public enum Q_Detail
    {
        None = -1,
        Key,
    }

    public enum Q_AddAction
    {
        None = -1,

    }

    public Q_Detail questDetail;
    public Q_Kind questKind;
    public Q_AddAction qusetAddAction;
    public string desc;
    public string title;
    protected int targetCount;
    public int nextQuestID; //���� ����Ʈ ���

    protected int currentCount;
    protected Action action_QuestAddDount;
    public bool isClearQuest = false; //Ŭ���� ���ǿ� �´� ����Ʈ ���� Ȯ��

    public int GetCurrentCount() => currentCount;
    public int GetTargetCount() => targetCount;


    public bool CheckQuestDone()
    {
        return targetCount <= currentCount;
    }

    public void QuestAddCountActionAdd(Action AddCountQuestAction)
    {
        action_QuestAddDount = AddCountQuestAction;
    }

    public void AddCount(int count)
    {
        currentCount += count;
        action_QuestAddDount?.Invoke(); //���൵�� ���� AI ��ȭ�� �ʿ��Ұ�� ���
    }

#if UNITY_EDITOR
    public void TestSetTargetCount(int count) { targetCount = count; }
#endif
}

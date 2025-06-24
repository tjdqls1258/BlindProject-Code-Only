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
    public int nextQuestID; //연계 퀘스트 사용

    protected int currentCount;
    protected Action action_QuestAddDount;
    public bool isClearQuest = false; //클리어 조건에 맞는 퀘스트 인지 확인

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
        action_QuestAddDount?.Invoke(); //진행도에 따른 AI 변화가 필요할경우 사용
    }

#if UNITY_EDITOR
    public void TestSetTargetCount(int count) { targetCount = count; }
#endif
}

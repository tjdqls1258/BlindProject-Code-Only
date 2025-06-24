using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Quest;

public class QuestManager : Singleton<QuestManager>
{
    protected List<Quest> _questsAll = new();
    protected List<int> _endQuest = new();
    protected List<Quest> _currentQuests = new();

    public QuestManager() { Init();  }

    public override void Init()
    {
        base.Init();
        _questsAll.Clear();
        _endQuest.Clear();
        _currentQuests.Clear();

        TestQuest();
    }

    private void TestQuest()
    {
        Quest test_Q = new Quest()
        {
            id = 1,
            questDetail = Q_Detail.None,
            questKind = Q_Kind.Install,
            qusetAddAction = Q_AddAction.None,
            title = "테스트",
            desc = "설치 테스트",
            nextQuestID = 0,
        };

        test_Q.TestSetTargetCount(1);

        _questsAll.Add(test_Q);
        _currentQuests.Add(test_Q);
    }

    public void QuestBorad(Q_Kind kind, Q_Detail detail, int Addcount = 1)
    {
        Debug.Log($"Kind = {kind}, Detail = {detail}, AddCount = {Addcount}");

        var quest = GetQuest(kind, detail);

        if (quest != null && quest.Count > 0)
        {
            for(int i = _currentQuests.Count-1; i >= 0;i--)
            {
                if (_currentQuests[i].questKind == kind && _currentQuests[i].questDetail == detail)
                    _currentQuests[i].AddCount(Addcount);

                if (_currentQuests[i].CheckQuestDone())
                {
                    QuestDoneAction(_currentQuests[i].id);
                    AddNextQuest(_currentQuests[i].nextQuestID);
                }
            }
        }
    }

    public bool CheckQuest(int QuestID)
    {
        var q = GetQuest(QuestID);

        if(q != null)
            return q.CheckQuestDone();

        return false;
    }

    private Quest GetQuest(int id)
    {
        var quest = _questsAll.Find((x) => x.id == id);
        if (quest != null)
            return quest;

        Debug.LogError($"{id} Quest Is Null");
        return null;
    }

    private List<Quest> GetQuest(Q_Kind kind, Q_Detail detail)
    {
        var quest = _questsAll.FindAll((x) => x.questKind == kind && x.questDetail == detail && x.id != 0);
        if(quest != null)
            return quest;

        Debug.LogError($"{kind}, {detail} Quest Is Null");
        return null;
    }

    public void QuestDoneAction(int QuestID)
    {
        _endQuest.Add(QuestID);
    }

    private void AddNextQuest(int next)
    {
        if (next <= 0) return;

        _currentQuests.Add(GetQuest(next));
    }

    public struct QuestInfo
    {
        public string title;
        public string desc;
        public int targetCount;
        public int currentCount;
        public bool isDone() { return currentCount >= targetCount; }
    }

    public List<QuestInfo> GetCurrentQuestInfo()
    {
        List<QuestInfo> info = new();
        foreach (Quest quest in _currentQuests)
        {
            info.Add(new QuestInfo()
            {
                currentCount = quest.GetCurrentCount(),
                title = quest.title,
                desc = quest.desc,
                targetCount = quest.GetTargetCount()
            });
        }


        return info;
    }

    public List<string> GetDescList()
    {
        List<string> desc = new();
        foreach (Quest quest in _currentQuests)
        {
            desc.Add(quest.desc);
        }


        return desc;
    }

    public bool CheckClearGuestAll()
    {
        bool isClear = true;
        foreach (Quest quest in _currentQuests)
        {
            if (quest.CheckQuestDone() == false)
            {
                isClear = false;
                break;
            }

        }
        return isClear;
    }
}

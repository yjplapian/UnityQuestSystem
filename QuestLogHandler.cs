using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//TODO: If there is one quest instance, and is never selected, keep updating the UI on each change.
public class QuestLogHandler : MonoBehaviour
{
    bool isActive;

    [Header("Components")]
    [SerializeField] InputHandler input;
    [SerializeField] GameObject _gameObject;

    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI logText;

    [SerializeField] RectTransform activeQuestContent;
    [SerializeField] RectTransform finishedQuestContent;
    [SerializeField] RectTransform objectivesContent;

    [SerializeField] List<GameObject> objectiveInstances = new();
    Dictionary<Quest, QuestEntry> questInstances = new();

    [Header("Prefabs")]
    [SerializeField] GameObject entryPrefab;
    [SerializeField] GameObject objectivePrefab;

    [Header("Events")]
    [SerializeField] EventChannel<InputConfig> OnGetControls;
    [Space(5)]
    [SerializeField] EventChannel<Quest> OnStartQuest;
    [SerializeField] EventChannel<Quest> OnObjectiveEnd;
    [SerializeField] EventChannel<Quest> OnEndQuest;
    [SerializeField] EventChannel<Quest> OnUpdateSelection;

    private void OnEnable()
    {
        OnStartQuest.OnEventRaised += AddNewQuestInstance;

        OnEndQuest.OnEventRaised += Repaint;
        OnObjectiveEnd.OnEventRaised += Repaint;
        OnUpdateSelection.OnEventRaised += Repaint;
    }

    private void OnDisable()
    {
        OnStartQuest.OnEventRaised -= AddNewQuestInstance;

        OnEndQuest.OnEventRaised -= Repaint;
        OnObjectiveEnd.OnEventRaised -= Repaint;
        OnUpdateSelection.OnEventRaised -= Repaint;
    }


    private void Update() =>
        GetWindow();

    private void Repaint(Quest quest)
    {
        if (quest.questCondition == QuestCondition.Completed || quest.questCondition == QuestCondition.Failed)
        {
            questInstances[quest].transform.SetParent(finishedQuestContent);
            titleText.text = $"{quest.questCondition}: {quest._name}";
        }

        else
        titleText.text = quest._name;

        logText.text = quest.CurrentStage.stageLog;

        int currentStage = quest.stageIndex;
        int instances = 0;

        for(int i = 0; i <= currentStage; i++)
            instances += quest.stages[i].objectives.Count;

        for(int i = 0; i < objectiveInstances.Count; i++)
        {
            if(i < instances)
            {
                objectiveInstances[i].SetActive(true);

                if (!objectiveInstances[i].TryGetComponent(out QuestObjectiveUI temp))
                    return;

                string context = questInstances[quest].UI.objectives[i].context;
                QuestObjectiveCondition condition = questInstances[quest].UI.objectives[i].condition;
                temp.SetUI(quest, context);
                temp.IsComplete(condition);
            }

            else
                objectiveInstances[i].SetActive(false);
        }
    }


    private void AddNewQuestInstance(Quest quest)
    {
        var instance = Instantiate(entryPrefab, activeQuestContent);

        if (!instance.TryGetComponent(out QuestEntryUI temp))
        {
            Debug.LogError($"{instance} object doesnt have the UI script.");
            Destroy(instance);
            return;
        }

        temp.Set(quest);

        QuestEntry element = new(temp, instance.transform);

        if (!questInstances.ContainsKey(quest))
            questInstances.Add(quest, element);

        if (questInstances.Count == 1)
            Repaint(quest);
    }

    private void GetWindow()
    {
        if (input.OnKeyDown("QuestLog"))
        {
            isActive = !isActive;
            _gameObject.SetActive(isActive);

            if (isActive)
            {
                MouseState.SetMouseState(1);
                OnGetControls?.RaiseEvent(input.MenuControls);
            }

            else
            {
                MouseState.SetMouseState(0);
                OnGetControls?.RaiseEvent(input.MainControls);
            }
        }
    }
}
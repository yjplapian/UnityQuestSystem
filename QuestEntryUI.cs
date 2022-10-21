using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

//TODO: FIX Entry UI
public class QuestEntryUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public List<QuestObjective> objectives = new();

    bool isActive;
    QuestCondition condition;
    Quest quest;

    [Header("Components")]
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject imageObject;
    [SerializeField] Animator animator;

    [Header("Events")]
    [SerializeField] EventChannel<Quest> OnSelectionUpdate;
    [SerializeField] EventChannel<Quest> OnTrackQuest;

    public void Set(Quest quest)
    {
        this.quest = quest;
        this.text.text = quest.name;

        this.quest.questCondition = condition;

        int stage = quest.stages.Count;
        for (int i = 0; i < stage; i++)
        {
            for (int j = 0; j < quest.stages[i].objectives.Count; j++)
            {
                if (!objectives.Contains(quest.stages[i].objectives[j]))
                {
                    objectives.Add(quest.stages[i].objectives[j]);
                }
            }
        }           
    }

    public void SetCondition(QuestCondition condition) =>
        this.condition = condition;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnSelectionUpdate.RaiseEvent(quest);

        if (animator != null && quest.questCondition != QuestCondition.Completed || quest.questCondition != QuestCondition.Failed)
            animator.Play("Hover");

        if (animator != null && quest.questCondition == QuestCondition.Completed || quest.questCondition == QuestCondition.Failed)
            animator.Play("Hover Completed");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (quest.questCondition == QuestCondition.Completed || quest.questCondition == QuestCondition.Failed)
            return;

        isActive = !isActive;
        imageObject.SetActive(isActive);
        OnTrackQuest?.RaiseEvent(quest);
    }

    public void OnPointerExit(PointerEventData eventData) =>
        StartCoroutine(OnExit());

    private IEnumerator OnExit()
    {
        yield return new WaitForSeconds(0.1f);

        if (animator != null && quest.questCondition != QuestCondition.Completed || quest.questCondition != QuestCondition.Failed)
            animator.Play("Hover");

        if (animator != null && quest.questCondition == QuestCondition.Completed || quest.questCondition == QuestCondition.Failed)
            animator.Play("Hover Completed");
    }
}


public struct QuestEntry
{
    public QuestEntry(QuestEntryUI entry, Transform transform)
    {
        this.UI = entry;
        this.transform = transform;
    }

    public QuestEntryUI UI;
    public Transform transform;
}
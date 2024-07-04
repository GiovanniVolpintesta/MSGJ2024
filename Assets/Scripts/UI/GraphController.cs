using data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GraphController : MonoBehaviour
{
    [SerializeField]
    private RectTransform graphArea;

    [SerializeField]
    private string displayedCharId;
    [SerializeField]
    private string displayedStatId;

    [SerializeField]
    private string dayStatId;

    [SerializeField]
    private GameObject piecePrefab;
    [SerializeField]
    private Color positiveColor;
    [SerializeField]
    private Color negativeColor;

    //[SerializeField]
    //private GameObject separatorPrefab;

    Stat dayStat = null;
    StatProgressData dayStatProgress = null;
    Stat displayedStat = null;
    StatProgressData displayedStatProgress = null;

    bool isStarted = false;

    void Start()
    {
        if (graphArea != null)
        {
            dayStat = GameData.Instance.findStat(dayStatId);
            displayedStat = GameData.Instance.findStat(displayedStatId);

            dayStatProgress = (dayStat != null) ? GameData.Instance.ProgressData.findGlobalStat(dayStat.Id) : null;

            if (displayedStat != null)
            {
                if (displayedStat.IsGlobalStat())
                {
                    displayedStatProgress = GameData.Instance.ProgressData.findGlobalStat(displayedStat.Id);
                    if (displayedCharId != null && displayedCharId != "") Debug.LogWarning("Stat '"+ displayedStat.Id + "' is not a character stat. The displayedCharId is ignored.");
                }
                else
                {
                    CharacterProgressData charProgress = GameData.Instance.ProgressData.findCharData(displayedCharId);
                    displayedStatProgress = (charProgress != null) ? charProgress.findStat(displayedStat.Id) : null;
                }
            }            
        }

        Bind();

        isStarted = true;
    }

    private void OnEnable()
    {
        if (isStarted)
        {
            Bind();
        }
    }

    private void OnDisable()
    {
        Unbind();
    }

    private void Bind()
    {
        if (dayStatProgress != null)
        {
            dayStatProgress.onValueChanged += onDayChange;
        }

        if (displayedStatProgress != null && GameData.Instance != null && GameData.Instance.ProgressData != null)
        {
            GameData.Instance.ProgressData.onDialogueEnded += OnDialogueEnded;
        }
    }

    private void Unbind()
    {
        if (dayStatProgress != null)
        {
            dayStatProgress.onValueChanged -= onDayChange;
        }

        if (displayedStatProgress != null && GameData.Instance != null && GameData.Instance.ProgressData != null)
        {
            GameData.Instance.ProgressData.onDialogueEnded -= OnDialogueEnded;
        }
    }

    private void onDayChange(float oldValue, float newValue)
    {

    }

    private void OnDialogueEnded(Dialogue dialogue, bool incrementedTime)
    {
        if (incrementedTime)
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        if (graphArea != null && piecePrefab != null
            && dayStat != null && dayStatProgress != null
            && displayedStat != null && displayedStatProgress != null)
        {
            Color color = (displayedStatProgress.Value >= displayedStat.InitValue) ? positiveColor : negativeColor;

            while (graphArea.childCount > 0)
            {
                GameObject child = graphArea.GetChild(0).gameObject;
                child.transform.SetParent(null, false);
                Destroy(child);
            }

            var valuesHistory = displayedStatProgress.History;
            int expectedDomainPoints = Mathf.Max(GameData.Instance.DialoguesPerDay * dayStat.Max + 2, valuesHistory.Count);

            float pointsDistance = graphArea.sizeDelta.x / expectedDomainPoints;
            float oldValue = valuesHistory.Count > 0 ? valuesHistory.ElementAt(0) : 0;
            for (int i = 0; i < valuesHistory.Count; i++)
            {
                float value = valuesHistory.ElementAt(i);
                float startHeightPercentage = (oldValue - displayedStat.Min) / (displayedStat.Max - displayedStat.Min);
                float endHeightPercentage = (value - displayedStat.Min) / (displayedStat.Max - displayedStat.Min);

                float linePositionX = i * pointsDistance;
                float linePositionY = startHeightPercentage * graphArea.sizeDelta.y;
                float lineVectorX = pointsDistance;
                float lineVectorY = (endHeightPercentage - startHeightPercentage) * graphArea.sizeDelta.y;
                float lineVectorMagnitude = Mathf.Sqrt(Mathf.Pow(lineVectorX, 2) + Mathf.Pow(lineVectorY, 2));
                float lineVectorAngle = Mathf.Rad2Deg * Mathf.Atan2(lineVectorY, lineVectorX);

                GameObject newLine = GameObject.Instantiate(piecePrefab, graphArea);
                RectTransform newLineRect = newLine.GetComponent<RectTransform>();
                newLineRect.pivot = new Vector2(0, 0.5f);
                newLineRect.anchorMin = newLineRect.anchorMax = new Vector2(0, 0);
                newLineRect.anchoredPosition = new Vector2(linePositionX, linePositionY);
                newLineRect.sizeDelta = new Vector2(lineVectorMagnitude, newLineRect.sizeDelta.y);
                newLineRect.Rotate(0, 0, lineVectorAngle);

                Image newLineImage = newLine.GetComponent<Image>();
                if (newLineImage == null && newLine.transform.childCount > 0)
                {
                    newLineImage = newLine.GetComponentInChildren<Image>();
                }
                if (newLineImage != null)
                {
                    newLineImage.color = color;
                }

                oldValue = value;
            }
        }
    }
}

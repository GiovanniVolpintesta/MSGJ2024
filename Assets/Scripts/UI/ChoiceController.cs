using data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public delegate void OnChoiceSelected(int index);

public class ChoiceController : MonoBehaviour
{
    public event OnChoiceSelected onChoiceSelected;

    [SerializeField]
    TextMeshProUGUI text;
    [SerializeField]
    Button button;

    int choiceIndex;

    public void SetupButton(string message, int choiceIndex)
    {
        this.choiceIndex = choiceIndex;
        if (text != null)
        {
            text.SetText(message);
        }
    }

    private void OnEnable()
    {
        if (button)
            button.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable()
    {
        if (button)
            button.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        if (onChoiceSelected != null)
        {
            onChoiceSelected(choiceIndex);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitGameController : MonoBehaviour
{
    [SerializeField]
    Button exitButton;

    [SerializeField]
    GameObject confirmExitPanel;

    [SerializeField]
    Button confirmExitButton;
    [SerializeField]
    Button cancelExitButton;
    private void Start()
    {
        if (exitButton != null && exitButton.onClick != null)
        {
            exitButton.onClick.AddListener(Exit);
        }
        if (confirmExitButton != null && confirmExitButton.onClick != null)
        {
            confirmExitButton.onClick.AddListener(ConfirmExit);
        }
        if (cancelExitButton != null && cancelExitButton.onClick != null)
        {
            cancelExitButton.onClick.AddListener(CancelExit);
        }
        if (confirmExitPanel != null)
        {
            confirmExitPanel.SetActive(false);
        }
    }

    private void Exit()
    {
        if (confirmExitPanel != null)
        {
            confirmExitPanel.SetActive(true);
        }
        else
        {
            ConfirmExit();
        }
    }

    private void ConfirmExit()
    {
        Application.Quit();
    }

    private void CancelExit()
    {
        if (confirmExitPanel != null)
        {
            confirmExitPanel.SetActive(false);
        }
    }
}

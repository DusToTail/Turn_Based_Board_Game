using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tips for controls in each control mode
/// </summary>
public class TipUI : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI textUI;

    [TextArea()]
    [SerializeField] private string characterModeTip;

    [TextArea()]
    [SerializeField] private string revealSkillTip;

    [TextArea()]
    [SerializeField] private string surveyTip;

    [TextArea()]
    [SerializeField] private string waitText;

    private void OnEnable()
    {
        PlayerController.OnControlModeSwitched += ChangePlayerBottomTip;
    }

    private void OnDisable()
    {
        PlayerController.OnControlModeSwitched -= ChangePlayerBottomTip;
    }

    private void Update()
    {
        Color newColor = new Color(textUI.color.r, textUI.color.g, textUI.color.b, 0.4f + 0.2f * Mathf.Cos(Time.time));
        textUI.color = newColor;
    }

    public void DisplayWaitingText()
    {
        if (AIController.AIs.Count == 0) { return; }
        textUI.text = waitText;
        //Debug.Log("Waiting for AI text");
    }

    public void DisplayPlayerText()
    {
        textUI.text = characterModeTip;
        //Debug.Log("Waiting for Player text");
    }

    private void ChangePlayerBottomTip(PlayerController.ControlMode mode)
    {
        if(mode == PlayerController.ControlMode.Character)
            textUI.text = characterModeTip;
        else if(mode == PlayerController.ControlMode.RevealSkill)
            textUI.text = revealSkillTip;
        else if(mode == PlayerController.ControlMode.Survey)
            textUI.text = surveyTip;
    }

    

}

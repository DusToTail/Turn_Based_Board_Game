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

    private void OnEnable()
    {
        PlayerController.OnControlModeSwitched += ChangeBottomTip;
    }

    private void OnDisable()
    {
        PlayerController.OnControlModeSwitched -= ChangeBottomTip;
    }

    private void Update()
    {
        Color newColor = new Color(textUI.color.r, textUI.color.g, textUI.color.b, 0.4f + 0.2f * Mathf.Cos(Time.time));
        textUI.color = newColor;
    }

    private void ChangeBottomTip(PlayerController.ControlMode mode)
    {
        if(mode == PlayerController.ControlMode.Character)
            textUI.text = characterModeTip;
        else if(mode == PlayerController.ControlMode.RevealSkill)
            textUI.text = revealSkillTip;
        else if(mode == PlayerController.ControlMode.Survey)
            textUI.text = surveyTip;
    }

}

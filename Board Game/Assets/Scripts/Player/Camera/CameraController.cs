using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the movement of the camera in game
/// </summary>
public class CameraController : MonoBehaviour
{
    public PlayerController playerController;
    public Vector3 offset;

    private void LateUpdate()
    {
        if(playerController == null) { return; }
        if(playerController.playerBlock == null) { return; }
        if(playerController.Mode == PlayerController.ControlMode.Character)
            transform.position = playerController.playerBlock.transform.position + offset;
        else
        {
            if (playerController.focusCell == null) { return; }
            transform.position = playerController.focusCell.worldPosition + offset;
        }
    }
}

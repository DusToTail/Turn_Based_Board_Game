using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerBlockTransform;
    public Vector3 offset;


    private void LateUpdate()
    {
        if(playerBlockTransform == null) { return; }
        transform.position = playerBlockTransform.position + offset;
    }


}

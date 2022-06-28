using UnityEngine;

public class FaceCameraBehaviour : MonoBehaviour
{
    private void LateUpdate()
    {
        Vector3 direction = Camera.main.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
    }
}

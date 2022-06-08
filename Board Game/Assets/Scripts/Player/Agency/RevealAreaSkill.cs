using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealAreaSkill : MonoBehaviour
{
    public Transform moveTransform;
    public Transform from;
    public Transform controlPoint;
    public Transform to;
    public Light moveLight;
    public float revealAreaRange;
    public float speed;
    public int coolDown;
    public int currentCooddown;
    public bool isFinished;

    [SerializeField]
    private bool displayGizmos;
    [SerializeField]
    private int drawSegments;

    public bool IsOffCooldown { get { return currentCooddown == 0; } }
    private GridController _gridController;

    private void OnEnable()
    {
        GameManager.OnPlayerTurnEnded += DecrementCurrentCooldown;
        GameManager.OnLevelStarted += RewindCooldown;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerTurnEnded -= DecrementCurrentCooldown;
        GameManager.OnLevelStarted -= RewindCooldown;
    }

    private void Start()
    {
        _gridController = FindObjectOfType<GridController>();
        moveLight.range = revealAreaRange;
    }

    public void RevealArea(Cell atCell)
    {
        isFinished = false;
        RewindCooldown();
        StartCoroutine(RevealAreaCoroutine(atCell));
    }

    private IEnumerator RevealAreaCoroutine(Cell atCell)
    {
        Cell fromCell = _gridController.GetCellFromCellWithDirection(atCell, GridDirection.Left);
        fromCell = _gridController.grid[_gridController.gridSize.y - 1, fromCell.gridPosition.z, fromCell.gridPosition.x];
        from.position = fromCell.worldPosition + Vector3.up * 10;
        from.rotation = Quaternion.LookRotation(Vector3.down, Vector3.right);
        Cell toCell = _gridController.GetCellFromCellWithDirection(atCell, GridDirection.Right);
        toCell = _gridController.grid[_gridController.gridSize.y - 1, toCell.gridPosition.z, toCell.gridPosition.x];
        to.position = toCell.worldPosition + Vector3.up * 10;
        to.rotation = Quaternion.LookRotation(Vector3.up, Vector3.left);
        controlPoint.position = atCell.worldPosition - Vector3.up * 10;
        controlPoint.rotation = Quaternion.LookRotation(Vector3.right, Vector3.up);

        float t = 0;
        float x = 0;
        while (true)
        {
            yield return null;
            if (t >= 1)
            {
                t = 1;
                MovementUtilities.QuadraticBezierLerp(moveTransform, from, to, controlPoint, t, true);
                break;
            }
            t = YFunction(x);
            MovementUtilities.QuadraticBezierLerp(moveTransform, from, to, controlPoint, t, true);
            x += Time.deltaTime * speed;
        }
        isFinished = true;
    }

    private float YFunction(float x)
    {
        return 4 * (x - 0.5f) * (x - 0.5f) * (x - 0.5f) + 0.5f;
    }

    private void DecrementCurrentCooldown()
    {
        currentCooddown--;
        if(currentCooddown <= 0) { currentCooddown = 0; }
    }

    private void RewindCooldown()
    {
        currentCooddown = coolDown;
    }

    private void OnDrawGizmos()
    {
        if (!displayGizmos) { return; }
        if(drawSegments == 0) { return; }

        Vector3[] vertices = new Vector3[drawSegments + 1];
        float t = 0;
        for (int i = 0; i < drawSegments + 1; i++)
        {
            t = (float)i * (1f / (float)drawSegments);
            vertices[i] = MovementUtilities.GetQuadraticBezierLerp(from, to, controlPoint, t);
        }

        Gizmos.color = Color.magenta;
        for (int i = 0; i < drawSegments; i++)
        {
            Gizmos.DrawLine(vertices[i], vertices[i + 1]);
        }
    }


}

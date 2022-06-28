using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component attached to player controller that allows calling down a point light at a certain position via Bezier Curve and returns it back up
/// </summary>
public class RevealAreaSkill : MonoBehaviour
{
    public AudioHandler audioHandler;
    public Transform moveTransform;
    public Transform from;
    public Transform controlPoint;
    public Transform to;
    public Light moveLight;
    public float revealAreaRange;
    public float speed;
    public int coolDown;
    public int currentCooldown;
    public bool isFinished;
    public delegate void CooldownRewinded();
    public static event CooldownRewinded OnCooldownRewinded;
    public delegate void CooldownDecremented();
    public static event CooldownDecremented OnCooldownDecremented;
    public bool IsOffCooldown { get { return currentCooldown == 0; } }
    [SerializeField] private bool displayGizmos;
    [SerializeField] private int drawSegments;
    private GridController _gridController;

    private void OnEnable()
    {
        GameManager.OnPlayerTurnEnded += DecrementCurrentCooldown;
        GameManager.OnLevelStarted += ResetCooldown;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerTurnEnded -= DecrementCurrentCooldown;
        GameManager.OnLevelStarted -= ResetCooldown;
    }

    private void Awake()
    {
        _gridController = FindObjectOfType<GridController>();
    }
    private void Start()
    {
        moveLight.enabled = false;
    }
    public void RevealArea(Cell atCell)
    {
        isFinished = false;
        RewindCooldown();
        StartCoroutine(RevealAreaCoroutine(atCell));
    }
    private IEnumerator RevealAreaCoroutine(Cell atCell)
    {
        moveLight.enabled = true;
        // Setting up trajectory with bezier curve's control points
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

        // Play sound effect
        audioHandler.Play("Reveal");

        // movement depends on t
        // t depends on x
        float t = 0;
        float x = 0;
        while (true)
        {
            yield return null;
            if (t > 1)
            {
                MovementUtilities.MoveQuadraticBezierLerp(moveTransform, from, to, controlPoint, 1, true);
                break;
            }
            MovementUtilities.MoveQuadraticBezierLerp(moveTransform, from, to, controlPoint, t, true);
            x += Time.deltaTime * speed;
            t = YFunction(x);
        }
        moveLight.enabled = false;
        isFinished = true;
    }
    private float YFunction(float x)
    {
        return 4 * (x - 0.5f) * (x - 0.5f) * (x - 0.5f) + 0.5f;
    }
    private void DecrementCurrentCooldown()
    {
        if(currentCooldown <= 0) { return; }
        currentCooldown--;
        if(OnCooldownDecremented != null)
            OnCooldownDecremented();
    }
    private void RewindCooldown()
    {
        currentCooldown = coolDown;
        if(OnCooldownRewinded != null)
            OnCooldownRewinded();
    }
    private void ResetCooldown() { currentCooldown = 0; }

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

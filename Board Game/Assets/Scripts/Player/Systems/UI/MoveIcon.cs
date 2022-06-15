using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Move Icon that can be removed or added
/// </summary>
public class MoveIcon : MonoBehaviour
{
    [Header("Parts")]
    [SerializeField]
    private RectTransform full;
    [SerializeField]
    private RectTransform left;
    [SerializeField]
    private RectTransform right;
    [SerializeField]
    private RectTransform large1;
    [SerializeField]
    private RectTransform large2;
    [SerializeField]
    private RectTransform small1;
    [SerializeField]
    private RectTransform small2;
    [Header("Movement")]
    [SerializeField]
    private float speed;

    private void Start()
    {
    }

    public void OnAdded()
    {
        StartCoroutine(FadeInCoroutine(right));
        StartCoroutine(FadeInCoroutine(left));
        StartCoroutine(FadeInCoroutine(large1));
        StartCoroutine(FadeInCoroutine(large2));
        StartCoroutine(FadeInCoroutine(small1));
        StartCoroutine(FadeInCoroutine(small2));
    }

    public void OnRemoved()
    {
        full.gameObject.SetActive(false);
        StartCoroutine(ShatteredCoroutine(right));
        StartCoroutine(ShatteredCoroutine(left));
        StartCoroutine(ShatteredCoroutine(large1));
        StartCoroutine(ShatteredCoroutine(large2));
        StartCoroutine(ShatteredCoroutine(small1));
        StartCoroutine(ShatteredCoroutine(small2));
    }

    private IEnumerator FadeInCoroutine(RectTransform rect)
    {
        float t = 1;
        Transform from = rect.GetChild(0) as Transform;
        Transform control = rect.GetChild(1) as Transform;
        Transform to = rect.GetChild(2) as Transform;
        Transform move = rect.GetChild(3) as Transform;

        // Play sound effect

        while (true)
        {
            if (t < 0)
            {
                t = 0;
                MovementUtilities.MoveQuadraticBezierLerp(move, from, to, control, t, true);
                Color curColor = move.GetComponent<Image>().color;
                Color endColor = new Color(curColor.r, curColor.g, curColor.b, 1 - t);
                move.GetComponent<Image>().color = endColor;
                break;
            }
            yield return null;
            MovementUtilities.MoveQuadraticBezierLerp(move, from, to, control, t, true);
            Color currentColor = move.GetComponent<Image>().color;
            Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, 1 - t);
            move.GetComponent<Image>().color = newColor;
            t -= Time.deltaTime * speed;
        }
        full.gameObject.SetActive(true);

        transform.GetComponentInParent<MovesUI>().isFinished = true;
    }


    private IEnumerator ShatteredCoroutine(RectTransform rect)
    {
        float t = 0;
        Transform from = rect.GetChild(0) as Transform;
        Transform control = rect.GetChild(1) as Transform;
        Transform to = rect.GetChild(2) as Transform;
        Transform move = rect.GetChild(3) as Transform;
        full.gameObject.SetActive(false);

        // Play sound effect

        while (true)
        {
            if (t > 1)
            {
                t = 1;
                MovementUtilities.MoveQuadraticBezierLerp(move, from, to, control, t, true);
                Color curColor = move.GetComponent<Image>().color;
                Color endColor = new Color(curColor.r, curColor.g, curColor.b, 1 - t);
                move.GetComponent<Image>().color = endColor;
                break;
            }
            yield return null;
            MovementUtilities.MoveQuadraticBezierLerp(move, from, to, control, t, true);
            Color currentColor = move.GetComponent<Image>().color;
            Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, 1 - t);
            move.GetComponent<Image>().color = newColor;
            t += Time.deltaTime * speed;
        }

        transform.GetComponentInParent<MovesUI>().isFinished = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Cooldown Icon that can be removed or added
/// </summary>
public class RevealSkillIcon : MonoBehaviour
{
    [Header("Parts")]
    [SerializeField] private RectTransform full;
    [SerializeField] private RectTransform left;
    [SerializeField] private RectTransform right;
    [SerializeField] private RectTransform large1;
    [SerializeField] private RectTransform large2;
    [SerializeField] private RectTransform small1;
    [SerializeField] private RectTransform small2;
    [Header("Movement")]
    [SerializeField] private float speed;

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
        Transform moveTransform = rect.GetChild(3) as Transform;

        // Play sound effect

        while (true)
        {
            yield return null;
            if (t < 0)
            {
                LerpTransformAndColor(moveTransform, from, to, control, 0);
                break;
            }
            LerpTransformAndColor(moveTransform, from, to, control, t);
            t -= Time.deltaTime * speed;
        }
        full.gameObject.SetActive(true);
    }


    private IEnumerator ShatteredCoroutine(RectTransform rect)
    {
        float t = 0;
        Transform from = rect.GetChild(0) as Transform;
        Transform control = rect.GetChild(1) as Transform;
        Transform to = rect.GetChild(2) as Transform;
        Transform moveTransform = rect.GetChild(3) as Transform;
        full.gameObject.SetActive(false);

        // Play sound effect

        while (true)
        {
            yield return null;
            if (t > 1)
            {
                LerpTransformAndColor(moveTransform, from, to, control, 1);
                break;
            }
            LerpTransformAndColor(moveTransform, from, to, control, t);
            t += Time.deltaTime * speed;
        }
    }

    private void LerpTransformAndColor(Transform moveTransform, Transform from, Transform to, Transform control, float t)
    {
        MovementUtilities.MoveQuadraticBezierLerp(moveTransform, from, to, control, t, true);
        Color currentColor = moveTransform.GetComponent<Image>().color;
        Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, 1 - t);
        moveTransform.GetComponent<Image>().color = newColor;
    }
}

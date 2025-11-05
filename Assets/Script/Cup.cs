using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cup : MonoBehaviour
{
    [HideInInspector] public Button button;

    [Header("Cup Images")]
    public Image standingImage;
    public Image layingImage;

    private RectTransform rectTransform;

    public void Initialize()
    {
        button = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();

        SetStanding();
        SetInteractable(false);
    }

    public void SetStanding()
    {
        if (standingImage != null)
            standingImage.gameObject.SetActive(true);
        if (layingImage != null)
            layingImage.gameObject.SetActive(false);
    }

    public void SetLaying()
    {
        if (standingImage != null)
            standingImage.gameObject.SetActive(false);
        if (layingImage != null)
            layingImage.gameObject.SetActive(true);
    }

    public void SetInteractable(bool state)
    {
        if (button != null)
            button.interactable = state;
    }

    public IEnumerator MoveTo(Vector2 targetPos, float duration)
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPos;
    }

    public IEnumerator MoveRelative(Vector2 movement, float duration)
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 targetPos = startPos + movement;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPos;
    }
}

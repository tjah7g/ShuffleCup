using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CupManager : MonoBehaviour
{
    [Header("Cup References")]
    public GameObject cupPrefab;
    public Transform cupsContainer;
    public float cupSpacing = 200f;

    [Header("Animation Settings")]
    public int roundsToMix = 3;
    public float mixSpeed = 0.3f;
    public float mixHeight = 100f;

    [Header("Die Image")]
    public Image dieImage;

    private List<Cup> activeCups = new List<Cup>();
    private Cup cupWithDie;

    public void SpawnCups(int amount)
    {
        foreach (var cup in activeCups)
        {
            if (cup != null)
                Destroy(cup.gameObject);
        }
        activeCups.Clear();

        float totalWidth = (amount - 1) * cupSpacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < amount; i++)
        {
            GameObject cupObj = Instantiate(cupPrefab, cupsContainer);
            Cup cup = cupObj.GetComponent<Cup>();

            if (cup != null)
            {
                RectTransform rt = cupObj.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(startX + (i * cupSpacing), 0);

                cup.Initialize();
                cup.button.onClick.AddListener(() => GameManager.Instance.OnCupClicked(cup));

                activeCups.Add(cup);
            }
        }

        cupWithDie = activeCups[Random.Range(0, activeCups.Count)];
    }

    public IEnumerator RevealPhase()
    {
        foreach (var cup in activeCups)
        {
            cup.SetLaying();
        }

        if (dieImage != null)
        {
            dieImage.enabled = true;
            RectTransform cupRect = cupWithDie.GetComponent<RectTransform>();
            RectTransform dieRect = dieImage.GetComponent<RectTransform>();
            dieRect.anchoredPosition = cupRect.anchoredPosition;
        }

        yield return new WaitForSeconds(1.5f);

        foreach (var cup in activeCups)
        {
            cup.SetStanding();
        }

        if (dieImage != null)
            dieImage.enabled = false;

        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator MixPhase()
    {
        Vector2[] originalPositions = new Vector2[activeCups.Count];
        for (int i = 0; i < activeCups.Count; i++)
        {
            RectTransform rt = activeCups[i].GetComponent<RectTransform>();
            originalPositions[i] = rt.anchoredPosition;
        }

        for (int round = 0; round < roundsToMix; round++)
        {
            foreach (var cup in activeCups)
            {
                StartCoroutine(cup.MoveRelative(Vector2.up * mixHeight, mixSpeed));
            }
            yield return new WaitForSeconds(mixSpeed);

            Vector2[] shuffled = ShuffleArray(originalPositions);
            for (int i = 0; i < activeCups.Count; i++)
            {
                RectTransform rt = activeCups[i].GetComponent<RectTransform>();
                Vector2 targetPos = new Vector2(shuffled[i].x, 0);
                StartCoroutine(activeCups[i].MoveTo(targetPos, mixSpeed));
            }
            yield return new WaitForSeconds(mixSpeed);
        }
    }

    public bool RevealCup(Cup clickedCup)
    {
        clickedCup.SetLaying();

        bool isCorrect = clickedCup == cupWithDie;

        if (isCorrect && dieImage != null)
        {
            dieImage.enabled = true;
            RectTransform cupRect = clickedCup.GetComponent<RectTransform>();
            RectTransform dieRect = dieImage.GetComponent<RectTransform>();
            dieRect.anchoredPosition = cupRect.anchoredPosition;
        }

        return isCorrect;
    }

    public void ResetCups()
    {
        if (dieImage != null)
            dieImage.enabled = false;

        foreach (var cup in activeCups)
        {
            cup.SetStanding();
        }

        cupWithDie = activeCups[Random.Range(0, activeCups.Count)];
    }

    public void EnableAllCupButtons()
    {
        foreach (var cup in activeCups)
            cup.SetInteractable(true);
    }

    public void DisableAllCupButtons()
    {
        foreach (var cup in activeCups)
            cup.SetInteractable(false);
    }

    T[] ShuffleArray<T>(T[] array)
    {
        T[] newArray = new T[array.Length];
        System.Array.Copy(array, newArray, array.Length);

        for (int i = 0; i < newArray.Length; i++)
        {
            T temp = newArray[i];
            int randomIndex = Random.Range(i, newArray.Length);
            newArray[i] = newArray[randomIndex];
            newArray[randomIndex] = temp;
        }

        return newArray;
    }
}
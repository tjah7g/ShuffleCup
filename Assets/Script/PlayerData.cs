using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData : MonoBehaviour
{
    [Header("Starting Values")]
    public int startingHealth = 3;
    public int startingCups = 3;

    [Header("Current Stats")]
    public int score;
    public int health;
    public int currentLevel;
    public int currentCups;

    public void ResetData()
    {
        score = 0;
        health = startingHealth;
        currentLevel = 1;
        currentCups = startingCups;
    }

    public void AddScore(int amount)
    {
        score += amount;
    }

    public void LoseHealth()
    {
        health--;
        if (health < 0) health = 0;
    }

    public void LevelUp()
    {
        currentLevel++;
        currentCups++;
    }
}

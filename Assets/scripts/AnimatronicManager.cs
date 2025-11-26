using UnityEngine;
using TMPro;
using System.Collections.Generic;

public enum Animatronic
{
    Steve,
    Marionette,
    John,
    Female
}
public class AnimatronicManager : MonoBehaviour
{
    [System.Serializable]
    public class DayDifficulties
    {
        public int[] dayDifficultyList; // Or any other type
    }
    [Header("AI Difficulty")]

    // overall AI scaling per day. define in unity in the same order as enum
    // (Steve = 0, Marionette = 1, etc)
    [SerializeField] private List<DayDifficulties> animatronicDifficultyPerDay; 

    void Update()
    {
    }


    public int GetAILevel(int day, Animatronic animatronic)
    {
        DayDifficulties animatronicDifficulty = animatronicDifficultyPerDay[(int) animatronic];
        if (day > animatronicDifficulty.dayDifficultyList.Length)
        {
            return day * animatronicDifficulty.dayDifficultyList[0];
        }
        return animatronicDifficulty.dayDifficultyList[day - 1];
    }
}

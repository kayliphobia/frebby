using UnityEngine;
using TMPro;

public enum Animatronic
{
    Steve,
    Marionette,
}
public class AnimatronicManager : MonoBehaviour
{
    [Header("AI Difficulty")]

    // overall AI scaling per day. define in unity in the same order as enum
    // (Steve = 0, Marionette = 1, etc)
    [SerializeField]
    private int[] baseDifficultyPerDay;    


    void Start()
    {
    }

    void Update()
    {
    }


    public int GetAILevel(int day, Animatronic animatronic)
    {
        return day * baseDifficultyPerDay[(int) animatronic];
    }

}

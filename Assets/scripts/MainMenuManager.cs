using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    /// <summary>
    /// Starts a new game.
    /// Saves day 1 and loads Main Scene.
    /// </summary>
    
    public void Start()
    {
        Time.timeScale = 1f;
    }
    
    public void NewGame()
    {
        SaveSystem.SaveDay(1); // start at day 1
        SceneManager.LoadScene("Main Scene");
    }

    /// <summary>
    /// Continues the game by reading the saved day.
    /// Loads Main Scene. The GameManager in Main Scene will handle loading the day.
    /// </summary>
    public void ContinueGame()
    {
        SceneManager.LoadScene("Main Scene");
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quit button pressed.");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

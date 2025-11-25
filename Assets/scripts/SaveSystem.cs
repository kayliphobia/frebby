using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string saveFileName = "savefile.txt";
    private static int maxDay = 5;

    // Save a day to the file
    public static void SaveDay(int day)
    {
        day = Mathf.Clamp(day, 1, maxDay);
        File.WriteAllText(saveFileName, day.ToString());
        Debug.Log("Saved Day: " + day);
    }

    // Load the day from the file
    public static int LoadDay()
    {
        if (File.Exists(saveFileName))
        {
            string text = File.ReadAllText(saveFileName);
            if (int.TryParse(text, out int day))
            {
                return Mathf.Clamp(day, 1, maxDay);
            }
        }

        // Default if no save exists
        return 1;
    }
}

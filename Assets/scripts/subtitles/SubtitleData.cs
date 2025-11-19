using UnityEngine;

[CreateAssetMenu(fileName = "NewSubtitles", menuName = "Audio/Subtitles")]
public class SubtitleData : ScriptableObject
{
    public SubtitleLine[] lines;
}

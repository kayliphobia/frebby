using UnityEngine;

public class PlushtrapClick : MonoBehaviour
{
    public AudioController plushtrap; // Drag your sound script here

    void OnMouseDown()
    {
        plushtrap.plushTrapSound();   // ← call your function
    }
}

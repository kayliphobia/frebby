using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class FisheyeEffect : MonoBehaviour
{
    public Material fisheyeMaterial;
    [Range(0f, 1f)]
    public float strength = 0.3f;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (fisheyeMaterial != null)
        {
            fisheyeMaterial.SetFloat("_Strength", strength);
            Graphics.Blit(src, dest, fisheyeMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}

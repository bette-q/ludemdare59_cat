using UnityEngine;

[ExecuteInEditMode]
public class FishEyeEffect : MonoBehaviour
{
    public Material mat;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (mat != null)
        {
            Graphics.Blit(src, dest, mat);
        }
        else
        {
            Graphics.Blit(src, dest); 
        }
    }
}
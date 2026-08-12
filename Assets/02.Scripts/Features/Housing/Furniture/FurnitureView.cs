using UnityEngine;

public class FurnitureView : ViewBase
{
    [SerializeField] Renderer[] Renderers;

    public void SetGhostMode(Material ghost)
    {
        for (int i = 0; i < Renderers.Length; i++)
        {
            Renderers[i].sharedMaterial = ghost;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public class FurnitureView : MonoBehaviour
{
    [SerializeField] Renderer[] Renderers;
    private Dictionary<Renderer, Material[]> _originMaterial = new Dictionary<Renderer, Material[]>();

    private void Awake()
    {
        InitRederers();
    }

    private void InitRederers()
    {
        if (_originMaterial.Count == 0)
        {
            foreach (Renderer renderer in Renderers)
            {
                _originMaterial[renderer] = renderer.sharedMaterials;
            }
        }
    }

    public void SetGhostMode(Material ghost)
    {
        InitRederers();

        foreach (Renderer renderer in Renderers)
        {
            int count = renderer.sharedMaterials.Length;

            Material[] ghostMat = new Material[count];

            for (int i = 0; i < count; i++)
            {
                ghostMat[i] = ghost;
            }

            renderer.materials = ghostMat;
        }
    }

    public void ResetMaterial()
    {
        foreach (var pair in _originMaterial)
        {
            pair.Key.materials = pair.Value;
        }
    }
}

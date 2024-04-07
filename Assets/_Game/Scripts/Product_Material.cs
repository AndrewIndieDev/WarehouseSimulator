using UnityEngine;

public class Product_Material : Product
{
    [Header("Material Specific")]
    [SerializeField] private MeshRenderer meshRenderer;
    public Material material;

    protected override void Start()
    {
        base.Start();
        if (meshRenderer != null)
        {
            meshRenderer.material = material;
        }
    }
}

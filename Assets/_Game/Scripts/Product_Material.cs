using UnityEngine;

public class Product_Material : Product
{
    [Header("Product Specific References")]
    public Material material;

    protected override void Start()
    {
        base.Start();
        if (mr != null)
        {
            mr.material = material;
        }
    }
}

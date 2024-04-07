using UnityEngine;

public class Product_Sprite : Product
{
    [Header("Sprite Specific")]
    [SerializeField] private MeshRenderer meshRenderer;
    public Sprite sprite;

    protected override void Start()
    {
        base.Start();
        if (meshRenderer != null)
        {
            meshRenderer.material.SetTexture("_MainTex", sprite.texture);
        }
    }
}

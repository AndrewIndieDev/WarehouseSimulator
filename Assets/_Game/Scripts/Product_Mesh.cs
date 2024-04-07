using UnityEngine;

public class Product_Mesh : Product
{
    [Header("Mesh Specific")]
    [SerializeField] private MeshFilter meshFilter;
    public Mesh mesh;

    protected override void Start()
    {
        base.Start();
        if (meshFilter != null)
        {
            meshFilter.mesh = mesh;
        }
    }
}

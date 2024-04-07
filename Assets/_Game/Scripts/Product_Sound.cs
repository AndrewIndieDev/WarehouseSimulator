using UnityEngine;
using UnityEngine.Audio;

public class Product_Sound : Product
{
    [Header("Sound Specific")]
    [SerializeField] private AudioSource audioSource;
    public AudioResource audioResource;

    protected override void Start()
    {
        base.Start();
        if (audioResource != null)
        {
            audioSource.resource = audioResource;
            audioSource.Play();
        }
    }
}

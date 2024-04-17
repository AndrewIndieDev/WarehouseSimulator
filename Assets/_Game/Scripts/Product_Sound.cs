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

    public override void PutInContainer(ContainerBox container)
    {
        base.PutInContainer(container);
        audioSource.volume = 0.2f;
    }

    public override void RemoveFromContainer()
    {
        base.RemoveFromContainer();
        audioSource.volume = 1.0f;
    }
}

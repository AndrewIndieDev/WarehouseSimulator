using MoreMountains.Feedbacks;
using Unity.Netcode;
using UnityEngine;

public class LabelPrinter : NetworkBehaviour
{
    [SerializeField] private MMF_Player feedbacks;
    [SerializeField] private Transform localLabel;
    [SerializeField] private GameObject labelPrefab;

    public void Print()
    {
        feedbacks.PlayFeedbacks();
    }

    public void InstantiateLabel()
    {
        NetworkObject no = Instantiate(labelPrefab, localLabel.transform.position, localLabel.transform.rotation).GetComponent<NetworkObject>();
        no.Spawn();
    }
}

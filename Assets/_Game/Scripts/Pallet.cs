using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Pallet : MonoBehaviour
{
    public int MaxContainerCount
    {
        get
        {
            return containerSlotParent.childCount;
        }
    }
    private int TotalContainersSpawned => Containers.Count;

    [SerializeField] private Transform containerSlotParent;
    [SerializeField] private GameObject containerPrefab;
    [SerializeField] private Transform palletVisualsParent;

    public Dictionary<int, ContainerBox> Containers = new();

    private void Start()
    {
        RandomisePalletVisuals();
    }

    public int AddItemToPallet(string productId, int amount)
    {
        if (Containers.Count == 0)
        {
            ContainerBox container = Instantiate(containerPrefab).GetComponent<ContainerBox>();
            Containers.Add(0, container);
            container.transform.SetLocalPositionAndRotation(containerSlotParent.GetChild(0).position, containerSlotParent.GetChild(0).rotation);
            container.NetworkObject.Spawn();
        }

        int amountLeft = amount;
        for (int i = 0; i < amount; i++)
        {
            if (!Containers[TotalContainersSpawned - 1].SpawnAndAddProduct(productId))
            {
                if (TotalContainersSpawned < MaxContainerCount)
                {
                    ContainerBox container = Instantiate(containerPrefab).GetComponent<ContainerBox>();
                    Containers.Add(TotalContainersSpawned, container);
                    container.transform.SetPositionAndRotation(containerSlotParent.GetChild(TotalContainersSpawned - 1).position, containerSlotParent.GetChild(TotalContainersSpawned - 1).rotation);
                    Containers[TotalContainersSpawned - 1].SpawnAndAddProduct(productId);
                    container.NetworkObject.Spawn();
                }
                else
                {
                    break;
                }
            }
            amountLeft--;
        }
        return amountLeft; // This returns the amount that couldn't be placed on this Pallet as it's full
    }

    private void RandomisePalletVisuals()
    {
        for (int i = 0 ; i < palletVisualsParent.childCount; i++)
        {
            palletVisualsParent.GetChild(i).gameObject.SetActive(false);
        }
        palletVisualsParent.GetChild(Random.Range(0, palletVisualsParent.childCount)).gameObject.SetActive(true);
    }
}

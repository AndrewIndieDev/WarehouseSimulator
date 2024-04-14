using System.Collections.Generic;
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
            ContainerBox container = Instantiate(containerPrefab, containerSlotParent.GetChild(0)).GetComponent<ContainerBox>();
            Containers.Add(0, container);
            container.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        foreach (ContainerBox container in Containers.Values)
        {
            if (amount <= 0)
                break;

            if (container.AddProduct(productId))
            {
                amount--;
                continue;
            }
        }

        for (int i = 0; i < amount; i++)
        {
            if (!Containers[TotalContainersSpawned - 1].AddProduct(productId))
            {
                if (TotalContainersSpawned < MaxContainerCount)
                {
                    ContainerBox container = Instantiate(containerPrefab, containerSlotParent.GetChild(TotalContainersSpawned)).GetComponent<ContainerBox>();
                    Containers.Add(TotalContainersSpawned, container);
                    container.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    Containers[TotalContainersSpawned - 1].AddProduct(productId);
                }
                else
                {
                    break;
                }
            }
        }
        return amount; // This returns the amount that couldn't be placed on this Pallet as it's full
    }

    private void RemoveContainer(Container toRemove)
    {
        foreach (var intContainerPair in Containers)
        {
            if (intContainerPair.Value == toRemove)
            {
                Containers.Remove(intContainerPair.Key);
                break;
            }
        }
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

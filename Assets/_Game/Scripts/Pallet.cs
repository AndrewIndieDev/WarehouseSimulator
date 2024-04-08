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
    [SerializeField] private Container containerPrefab;
    [SerializeField] private Transform palletVisualsParent;

    public Dictionary<int, Container> Containers = new();

    private void Start()
    {
        RandomisePalletVisuals();
    }

    public int AddItemToPallet(string productId, int amount)
    {
        foreach (Container container in FindAllContainersWithProduct(productId))
        {
            if (amount <= 0)
                break; ;

            int remainder = HasSpaceInContainer(container, amount);
            if (remainder != amount)
            {
                PlaceProductInContainer(container, productId, amount - remainder);
                amount = remainder;
                continue;
            }
        }

        while (amount > 10)
        {
            if (TotalContainersSpawned < MaxContainerCount)
            {
                Container container = Instantiate(containerPrefab, containerSlotParent.GetChild(TotalContainersSpawned));
                Containers.Add(TotalContainersSpawned, container);
                container.Transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                PlaceProductInContainer(container, productId, 10);
                amount -= 10;
            }
            else
            {
                break;
            }
        }

        if (amount <= 0)
            return 0;

        if (TotalContainersSpawned < MaxContainerCount)
        {
            Container container = Instantiate(containerPrefab, containerSlotParent.GetChild(TotalContainersSpawned));
            Containers.Add(TotalContainersSpawned, container);
            container.Transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            PlaceProductInContainer(container, productId, amount);
            amount = 0;
        }

        return amount; // This returns the amount that couldn't be placed on this Pallet as it's full
    }

    private List<Container> FindAllContainersWithProduct(string productId)
    {
        List<Container> containersWithProduct = new();
        foreach (Container container in Containers.Values)
        {
            if ((container.ContainedItem as Product).productId == productId)
            {
                containersWithProduct.Add(container);
            }
        }
        return containersWithProduct;
    }

    /// <summary>
    /// Returns the amount of items that couldn't be placed in the container
    /// </summary>
    /// <param name="container">Container to place items in.</param>
    /// <param name="amount">Amount to try place in the container.</param>
    /// <returns>Returns the amount of items that couldn't be placed in the container</returns>
    private int HasSpaceInContainer(Container container, int amount)
    {
        return container.CurrentAmount + amount <= container.MaxStackSize ? 0 : amount - (container.MaxStackSize - container.CurrentAmount);
    }

    private void PlaceProductInContainer(Container container, string productId, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Product product = ProductManager.Instance.SpawnProductPrefab(productId, container.Transform.position, container.Transform.rotation);
            if (product == null)
            {
                Debug.LogError($"No product found with id <{productId}>");
                return;
            }
            product.ProductIcon = ProductManager.Instance.productItemDictionary[productId].icon;
            container.OnPickupContainer.AddListener(RemoveContainer);
            container.PlaceItemInContainer(product);
        }
    }

    private void RemoveContainer(Container toRemove)
    {
        toRemove.OnPickupContainer.RemoveListener(RemoveContainer);
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

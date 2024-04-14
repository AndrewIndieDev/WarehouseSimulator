using System.Collections.Generic;
using UnityEngine;

public class PalletManager : MonoBehaviour
{
    public static PalletManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private Pallet palletPrefab;
    [SerializeField] private List<Pallet> pallets = new();
    [SerializeField] private Transform palletParent;

    public int PlaceOrderInTruck()
    {
        if (pallets.Count == 0)
        {
            Pallet newPallet = Instantiate(palletPrefab, palletParent.GetChild(0).transform.position, Quaternion.identity);
            pallets.Add(newPallet);
        }

        int remainder = 0;
        foreach (var productData in ProductManager.Instance.productData)
        {
            for (int i = 0; i < pallets.Count; i++)
            {
                Pallet pallet = pallets[i];
                productData.Value.transitCount = pallet.AddItemToPallet(productData.Key, productData.Value.transitCount);
                if (productData.Value.transitCount > 0 && i == pallets.Count - 1 && pallets.Count < palletParent.childCount)
                {
                    Pallet newPallet = Instantiate(palletPrefab, palletParent.GetChild(i + 1).position, Quaternion.identity);
                    pallets.Add(newPallet);
                }
            }
            if (productData.Value.transitCount > 0)
                remainder += productData.Value.transitCount;
        }
        // Unable to find enough space, remainder will need to be refunded, or needs to be rolled over to next order
        return remainder;
    }
}

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

    public int OrderNewStock(string productId, int amount)
    {
        if (pallets.Count == 0)
        {
            Pallet newPallet = Instantiate(palletPrefab, palletParent.GetChild(0).transform.position, Quaternion.identity);
            pallets.Add(newPallet);
        }

        for (int i = 0; i < pallets.Count; i++)
        {
            Pallet pallet = pallets[i];
            amount = pallet.AddItemToPallet(productId, amount);
            if (amount > 0 && i == pallets.Count - 1 && pallets.Count < palletParent.childCount)
            {
                Pallet newPallet = Instantiate(palletPrefab, palletParent.GetChild(i + 1).position, Quaternion.identity);
                pallets.Add(newPallet);
            }
        }
        // Unable to find enough space, remainder will need to be refunded, or needs to be rolled over to next order
        return amount;
    }


    /// <summary>
    /// REMOVE THIS LATER WHEN THE ORDERS CAN BE COMPLETED WITH UI
    /// </summary>
    private void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            EnableOrderPhysics();
        }
    }

    public void EnableOrderPhysics()
    {
        foreach (Pallet pallet in pallets)
        {
            foreach (KeyValuePair<int, Container> container in pallet.Containers)
            {
                container.Value.UnFreezeContainer();
            }
        }
    }
}

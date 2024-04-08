using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public long Money { get { return money; } }
    [SerializeField] private long money = 0;

    public Vector3 RespawnPosition { get { return respawnProduct.position; } }
    [SerializeField] private Transform respawnProduct;

    private void Start()
    {
        NameGenerator.Init();
        AddMoney(100000);
    }

    public void AddMoney(long amount)
    {
        money += amount;
    }

    public bool BuyProduct(long amount)
    {
        if (money >= amount)
        {
            money -= amount;
            return true;
        }
        return false;
    }
}

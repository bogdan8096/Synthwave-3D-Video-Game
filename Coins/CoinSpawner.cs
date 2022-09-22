using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public int maxCoins = 5;
    public float chanceToSpawn = 0.75f;
    public bool forceSpawnAll = false;
    private GameObject[] coins;

    private void Awake() {
        coins = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++) {
            coins[i] = transform.GetChild(i).gameObject;
        }
    }

    private void OnEnable()
    {
        if (Random.Range(0.0f, 1.0f) < chanceToSpawn)
            return;

        if (forceSpawnAll) {
            for (int i = 0; i < maxCoins; i++) {
                coins[i].SetActive(true);
            }
        }
        else {
            int coinsToSpawn = Random.Range(0, maxCoins);
            for (int i = 0; i < coinsToSpawn; i++) {
                coins[i].SetActive(true);
            }
        }
    }

    private void OnDisable() {
        foreach (GameObject coin in coins) {
            coin.SetActive(false);
        }
    }
}

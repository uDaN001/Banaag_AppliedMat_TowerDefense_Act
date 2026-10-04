using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("UI Elements")]
    public RectTransform coinUITarget;
    public TextMeshProUGUI coinText;
    public GameObject coinPrefab;

    private int currentCoins = 0;
    private int targetCoins = 0;

    void Awake() => Instance = this;
    void Start() => Enemy.OnEnemyDied += SpawnCoin;
    void OnDestroy() => Enemy.OnEnemyDied -= SpawnCoin;

    void SpawnCoin(Enemy enemy)
    {
        // Spawns a coin at the enemy's world position
        GameObject coin = Instantiate(coinPrefab, enemy.transform.position, Quaternion.identity);
        StartCoroutine(MoveCoinToUI(coin.transform));
    }

    IEnumerator MoveCoinToUI(Transform coinTrans)
    {
        Vector3 startPos = coinTrans.position;
        float duration = 1f;
        float time = 0;

        while (time < duration)
        {
            if (coinTrans == null) yield break;

            time += Time.deltaTime;
            float t = time / duration;

            // Convert UI target rect to world position for the coin to fly toward
            Vector3 targetWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(coinUITarget.position.x, coinUITarget.position.y, Camera.main.nearClipPlane + 5f));

            coinTrans.position = Vector3.Lerp(startPos, targetWorldPos, t);
            yield return null;
        }

        Destroy(coinTrans.gameObject);
        AddCoins(10);
    }

    public void AddCoins(int amount)
    {
        targetCoins += amount;
        StopAllCoroutines();
        StartCoroutine(PunchAndCountRoutine());
    }

    IEnumerator PunchAndCountRoutine()
    {
        // 1. UI Punch Scale
        Vector3 originalScale = Vector3.one;
        Vector3 punchScale = originalScale * 0.9f;
        float punchDuration = 0.15f;

        float t = 0;
        while (t < punchDuration)
        {
            t += Time.deltaTime;
            coinUITarget.localScale = Vector3.Lerp(originalScale, punchScale, t / punchDuration);
            yield return null;
        }

        // 2. Ease Value and Scale Down
        float duration = 0.5f;
        t = 0;
        int startCoins = currentCoins;

        while (t < duration)
        {
            t += Time.deltaTime;
            float easeOut = 1 - Mathf.Pow(1 - (t / duration), 3);

            currentCoins = Mathf.RoundToInt(Mathf.Lerp(startCoins, targetCoins, easeOut));
            coinText.text = currentCoins.ToString();

            coinUITarget.localScale = Vector3.Lerp(punchScale, originalScale, easeOut);
            yield return null;
        }

        currentCoins = targetCoins;
        coinText.text = currentCoins.ToString();
        coinUITarget.localScale = originalScale;
    }
}
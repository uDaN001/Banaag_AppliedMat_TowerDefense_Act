using UnityEngine;
using TMPro;
using System.Collections;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("UI Elements")]
    public RectTransform coinUITarget;
    public TextMeshProUGUI coinText;
    public GameObject coinPrefab;

    private int currentCoins = 0;
    private int targetCoins = 0;

    [SerializeField] float punchAmt = 1.1f;

    // Store reference to UI animation coroutine only
    private Coroutine punchAnimationCoroutine;

    void Awake() => Instance = this;

    void OnEnable() => Enemy.OnEnemyDied += SpawnCoin;
    void OnDisable() => Enemy.OnEnemyDied -= SpawnCoin;

    void SpawnCoin(Enemy enemy)
    {
        if (enemy == null) return;
        GameObject coin = Instantiate(coinPrefab, enemy.transform.position, Quaternion.identity);
        StartCoroutine(MoveCoinToUI(coin.transform));
    }

    IEnumerator MoveCoinToUI(Transform coinTrans)
    {
        Vector3 startPos = coinTrans.position;
        float duration = 0.8f;
        float time = 0f;

        while (time < duration)
        {
            if (coinTrans == null) yield break;

            time += Time.deltaTime;
            float t = time / duration;

            // Calculate target position in 2D space matching the coin's Z depth
            Vector3 targetScreenPos = coinUITarget.position;
            targetScreenPos.z = Mathf.Abs(Camera.main.transform.position.z - startPos.z);
            Vector3 targetWorldPos = Camera.main.ScreenToWorldPoint(targetScreenPos);

            // Add slight upward arc curve for better visual feel
            Vector3 currentLerp = Vector3.Lerp(startPos, targetWorldPos, t);
            float arc = Mathf.Sin(t * Mathf.PI) * 1.5f;
            coinTrans.position = currentLerp + new Vector3(0, arc, 0);

            yield return null;
        }

        Destroy(coinTrans.gameObject);
        AddCoins(10);
    }

    public void AddCoins(int amount)
    {
        targetCoins += amount;

        // Stop ONLY the UI animation routine, allowing coin flight routines to continue
        if (punchAnimationCoroutine != null)
        {
            StopCoroutine(punchAnimationCoroutine);
        }
        punchAnimationCoroutine = StartCoroutine(PunchAndCountRoutine());
    }

    IEnumerator PunchAndCountRoutine()
    {
        Vector3 originalScale = Vector3.one;
        Vector3 punchScale = originalScale * punchAmt; // Pop outwards
        float punchDuration = 0.1f;

        // 1. Expand Punch
        float t = 0f;
        while (t < punchDuration)
        {
            t += Time.deltaTime;
            coinUITarget.localScale = Vector3.Lerp(originalScale, punchScale, t / punchDuration);
            yield return null;
        }

        // 2. Count up numbers while returning scale to normal
        float duration = 0.4f;
        t = 0f;
        int startCoins = currentCoins;

        while (t < duration)
        {
            t += Time.deltaTime;
            float easeOut = 1f - Mathf.Pow(1f - (t / duration), 3);

            currentCoins = Mathf.RoundToInt(Mathf.Lerp(startCoins, targetCoins, easeOut));
            coinText.text = currentCoins.ToString();

            coinUITarget.localScale = Vector3.Lerp(punchScale, originalScale, easeOut);
            yield return null;
        }

        currentCoins = targetCoins;
        coinText.text = currentCoins.ToString();
        coinUITarget.localScale = originalScale;
        punchAnimationCoroutine = null;
    }
}
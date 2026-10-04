using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 20;
    private int currentHP;

    [Header("UI")]
    public Image realHealthBar;
    public Image ghostHealthBar;
    public float ghostDelay = 0.5f;
    public float ghostDuration = 1f;

    void OnEnable()
    {
        Enemy.OnEnemyReachedBase += TakeDamage;
    }

    void OnDisable()
    {
        Enemy.OnEnemyReachedBase -= TakeDamage;
    }

    void Start()
    {
        currentHP = maxHP;

        // Initialize UI fills at full health
        if (realHealthBar) realHealthBar.fillAmount = 1f;
        if (ghostHealthBar) ghostHealthBar.fillAmount = 1f;
    }

    void TakeDamage()
    {
        currentHP = Mathf.Max(0, currentHP - 1);
        UpdateUI();
    }

    void UpdateUI()
    {
        float targetFill = (float)currentHP / maxHP;
        if (realHealthBar) realHealthBar.fillAmount = targetFill;

        StopAllCoroutines();
        StartCoroutine(UpdateGhostBar(targetFill));
    }

    IEnumerator UpdateGhostBar(float targetFill)
    {
        yield return new WaitForSeconds(ghostDelay);

        if (ghostHealthBar == null) yield break;

        float startFill = ghostHealthBar.fillAmount;
        float time = 0;

        while (time < ghostDuration)
        {
            time += Time.deltaTime;
            float t = time / ghostDuration;
            float easeOut = 1f - Mathf.Pow(1f - t, 3);

            ghostHealthBar.fillAmount = Mathf.Lerp(startFill, targetFill, easeOut);
            yield return null;
        }
        ghostHealthBar.fillAmount = targetFill;
    }
}
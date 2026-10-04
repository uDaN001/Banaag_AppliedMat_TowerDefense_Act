using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    public BezierPath path;
    public float travelTime = 5f;
    public float spriteRotationOffset = 0f; // Use 90 if your sprite faces UP by default

    private float t = 0f;

    public static event Action<Enemy> OnEnemyDied;
    public static event Action OnEnemyReachedBase;

    void Start()
    {
        EnemyManager.Register(this);
    }

    void Update()
    {
        if (path == null) return;

        // 1. Advance time
        t += Time.deltaTime / travelTime;

        // 2. Move position using your fast polynomial math
        transform.position = path.GetPoint(t);

        // 3. Rotate to face travel direction using your Tangent math (Adapted for 2D)
        Vector3 dir = path.GetTangent(t);
        if (dir != Vector3.zero)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - spriteRotationOffset);
        }

        // 4. Check if reached the end
        if (t >= 1f)
        {
            OnEnemyReachedBase?.Invoke();
            Destroy(gameObject);
        }
    }

    public void TakeDamage()
    {
        OnEnemyDied?.Invoke(this);
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        EnemyManager.Unregister(this);
    }
}
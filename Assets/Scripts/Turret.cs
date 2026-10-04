using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Combat Settings")]
    public float range = 5f;
    public float fireRate = 1f;
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Rotation Settings")]
    public float spriteRotationOffset = 0f;
    public float rotationSpeed = 10f; // How fast the turret turns

    private float fireCooldown = 0f;

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        Enemy target = FindClosestEnemy();

        if (target != null)
        {
            RotateTowards(target);

            if (fireCooldown <= 0f)
            {
                Fire(target);
                fireCooldown = 1f / fireRate;
            }
        }
    }

    void RotateTowards(Enemy target)
    {
        // 1. Get the direction vector to the target
        Vector3 dir = target.transform.position - transform.position;

        // 2. Calculate the exact angle we WANT to be facing
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - spriteRotationOffset;

        // 3. Convert that angle into a rotation object (Quaternion)
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        // 4. Smoothly blend (Lerp) from the current rotation to the target rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    Enemy FindClosestEnemy()
    {
        Enemy closest = null;
        float minDistance = range;

        foreach (Enemy enemy in EnemyManager.ActiveEnemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = enemy;
            }
        }
        return closest;
    }

    void Fire(Enemy target)
    {
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bulletObj.GetComponent<Bullet>().SetTarget(target);
    }
}
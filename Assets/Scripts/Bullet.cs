using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Enemy target;
    public float speed = 10f;

    public void SetTarget(Enemy _target) => target = _target;

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.transform.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            target.TakeDamage(); // 1-hit kill
            Destroy(gameObject);
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }
}
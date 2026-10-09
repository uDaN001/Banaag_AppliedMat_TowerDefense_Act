using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] float maxLifeTime = 5f;
    [SerializeField] float hitDistanceThreshold = 1f;

    private Transform playerTransform;
    private PlayerController playerController;
    private float lifeTime = 0f;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null )
        {
            playerTransform = playerObj.transform;
            playerController = playerObj.GetComponent<PlayerController>();
            Debug.Log("Player Detected!");
        }
    }

    private void Update()
    {
        lifeTime += Time.deltaTime;
        if (lifeTime >= maxLifeTime)
        {
            Destroy(gameObject);
            return;
        }

        if (playerTransform == null) return;

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        if (directionToPlayer != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.AngleAxis (targetAngle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.Self);

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= hitDistanceThreshold)
        {
            if (playerController != null)
            {
                playerController.TakeHit();
            }
            Destroy(gameObject);
        }
    }
}

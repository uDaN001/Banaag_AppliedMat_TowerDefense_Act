using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float forwardSpeed = 10f;
    [SerializeField] float turnSpeed = 120f;

    [SerializeField] int maxHits = 5;
    private int currentHits = 0;

    private void Update()
    {
        transform.Translate(Vector3.up * forwardSpeed * Time.deltaTime, Space.Self);

        float turnInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.forward, -turnInput * turnSpeed * Time.deltaTime);
    }

    public void TakeHit()
    {
        currentHits++;

        if (currentHits >= maxHits)
        {
            RestartGame();
        }
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

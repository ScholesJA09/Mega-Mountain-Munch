using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public static Vector3 respawnPosition;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (respawnPosition == Vector3.zero)
        {
            respawnPosition = transform.position;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            TeleportToRespawn();
        }
    }

    public void TeleportToRespawn()
    {
        transform.position = respawnPosition;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}

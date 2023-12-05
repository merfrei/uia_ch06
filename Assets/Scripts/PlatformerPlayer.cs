using UnityEngine;

public class PlatformerPlayer : MonoBehaviour
{

    public float speed = 4.5f;
    public float jumpForce = 4.0f;

    private BoxCollider2D box;
    private Rigidbody2D body;
    private Animator anim;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        box = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        float deltaX = Input.GetAxis("Horizontal") * speed;

        if (Mathf.Approximately(deltaX, 0))
        {
            transform.localScale = new Vector3(Mathf.Sign(deltaX), 1, 1);
        }
        Vector2 movement = new Vector2(deltaX, body.velocity.y);
        body.velocity = movement;

        // Animation
        anim.SetFloat("speed", Mathf.Abs(deltaX));

        Vector3 max = box.bounds.max;
        Vector3 min = box.bounds.min;
        // Check bellow the collider's min Y values
        Vector2 corner1 = new Vector2(max.x, min.y - .1f);
        Vector2 corner2 = new Vector2(min.x, min.y - .1f);
        Collider2D hit = Physics2D.OverlapArea(corner1, corner2);

        // Moving platforms
        MovingPlatform movingPlatform = null;

        bool grounded = false;
        if (hit != null)
        {
            grounded = true;
            movingPlatform = hit.GetComponent<MovingPlatform>();
        }

        // Changing parent transform element when the player is on a moving platform
        if (movingPlatform != null)
        {
            StickToPlatform(movingPlatform, deltaX);
        }
        else if (transform.parent != null)
        {
            transform.parent = null;
        }

        // Disable gravity when the player is grounded and not moving
        body.gravityScale = (grounded && Mathf.Approximately(deltaX, 0)) ? 0 : 1;

        if (grounded && Input.GetKeyDown(KeyCode.UpArrow))
        {
            body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void StickToPlatform(MovingPlatform platform, float deltaX)
    {
        transform.parent = platform.transform;
        Vector3 pScale = platform.transform.localScale;
        transform.localScale = new Vector3(Mathf.Sign(deltaX) / pScale.x, 1 / pScale.y, 1);
    }
}

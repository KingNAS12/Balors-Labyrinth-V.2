using UnityEngine;

public class KnightMoveScript : MonoBehaviour
{
    public BoxCollider2D knightBoxCollider;
    public Animator knightAnimator;
    public AudioSource knightAudio;
    public AudioClip knightWalk;

    public LayerMask enemyLayer;
    public LayerMask backgroundLayer;

    public bool walkingSound = false;
    public bool isPaused = false; 
    public float walkSpeed = 7;
    public float turnSpeed = 7;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        knightAudio.volume = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPaused)
        {
            checkMove();
        }
    }

    public void checkMove() // Checks the user's inputs, and accordingly moves the Knight. 
    {
        if ((Input.GetKey(KeyCode.W) == true) || (Input.GetKey(KeyCode.A) == true) || (Input.GetKey(KeyCode.S) == true) || (Input.GetKey(KeyCode.D) == true)) // If <WASD> is being pressed...
        {
            knightAnimator.SetBool("isMoving", true); // Play walking animation
            if (walkingSound == false) // If not already playing footsteps sound, then play sound. 
            {
                walkingSound = true;
                knightAudio.clip = knightWalk;
                knightAudio.Play();
                knightAudio.loop = true;
            }
        }
        else // If <WASD> is not being pressed...
        {
            knightAnimator.SetBool("isMoving", false); // Stop walking animation
            if (walkingSound == true) // If playing footsteps sound, stop playing sound. 
            {
                walkingSound = false;
                knightAudio.Stop();
                knightAudio.loop = false;
            }
        }
        if (Input.GetKeyUp(KeyCode.Space) == true) // Turn around
        {
            if (transform.localScale.x > 0)
            {
                transform.Translate(new Vector2(-turnSpeed * Time.deltaTime, 0));
            }
            else
            {
                transform.Translate(new Vector2(turnSpeed * Time.deltaTime, 0));
            }
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        }
        float x1 = 0;
        float y1 = 0;
        if (Input.GetKey(KeyCode.W) == true) // Move Up
        {
            y1 = walkSpeed;
        }
        if (Input.GetKey(KeyCode.A) == true) // Move Left
        {
            x1 = -walkSpeed;
        }
        if (Input.GetKey(KeyCode.S) == true) // Move Down
        {
            y1 = -walkSpeed;
        }
        if (Input.GetKey(KeyCode.D) == true) // Move Right
        {
            x1 = walkSpeed;
        }
        Vector2 movement = new Vector2(x1 * Time.deltaTime, y1 * Time.deltaTime);
        bool canMove = checkCollide(movement); 
        if (canMove)
        {
            transform.Translate(movement);
        }
    }

    public bool checkCollide(Vector2 movement)
    {
        bool canMove = true; 
        float x1 = movement.x; 
        float y1 = movement.y;
        RaycastHit2D hitUp = Physics2D.BoxCast(new Vector2(knightBoxCollider.bounds.center.x + (x1 * 1.5f), knightBoxCollider.bounds.center.y + (y1 * 1.5f)), knightBoxCollider.size, 0, Vector2.up, 0, backgroundLayer);
        RaycastHit2D hitLeft = Physics2D.BoxCast(new Vector2(knightBoxCollider.bounds.center.x + (x1 * 1.5f), knightBoxCollider.bounds.center.y + (y1 * 1.5f)), knightBoxCollider.size, 0, Vector2.left, 0, backgroundLayer);
        RaycastHit2D hitDown = Physics2D.BoxCast(new Vector2(knightBoxCollider.bounds.center.x + (x1 * 1.5f), knightBoxCollider.bounds.center.y + (y1 * 1.5f)), knightBoxCollider.size, 0, Vector2.down, 0, backgroundLayer);
        RaycastHit2D hitRight = Physics2D.BoxCast(new Vector2(knightBoxCollider.bounds.center.x + (x1 * 1.5f), knightBoxCollider.bounds.center.y + (y1 * 1.5f)), knightBoxCollider.size, 0, Vector2.right, 0, backgroundLayer);
        if (hitUp.collider || hitLeft || hitDown || hitRight)
        {
            canMove = false; 
        }
        return canMove; 
    }

    public void pauseMovement()
    {
        isPaused = true;
        knightAnimator.SetBool("isMoving", false);
        walkingSound = false;
        knightAudio.Stop();
        knightAudio.loop = false;
    }

    public void resumeMovement()
    {
        isPaused = false;
    }
}




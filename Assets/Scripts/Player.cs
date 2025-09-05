using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    // Simplify player states into an enumeration
    public enum PlayerState
    {
        Grounded,
        Jumping,
        Falling
    }

    [Header("Player Movement")]
    [SerializeField] private float moveSpeed = 5.0f;

    [Header("Player Threshold")]
    [SerializeField] private float apexHeight = 10.0f;

    [Header("Interactable Block")]
    [SerializeField] private GameObject interactableBlock;

    private new Rigidbody2D rigidbody2D;

    private PlayerState playerState = PlayerState.Falling;

    // Power up logic after hitting a ! block
    private bool powerUpActive = false;
    private float powerUpTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize components
        rigidbody2D = GetComponent<Rigidbody2D>();

        // Initialize player's scale in game before updating
        rigidbody2D.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
    }
    
    void FixedUpdate()
    {
        // Get horizontal movement by player input
        float horizontalMovement = Input.GetAxis("Horizontal");
        Vector2 movement = new Vector2(horizontalMovement, 0.0f);

        PerformJump();

        // Update player's position
        rigidbody2D.transform.position += (Vector3)movement * moveSpeed * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        PowerUpLogic();
    }

    private void PerformJump()
    {
        // DETECT INPUT FOR JUMPING

        // Make the player jump whenever space key is held and player is grounded
        if (Input.GetKey(KeyCode.Space) && playerState == PlayerState.Grounded)
        {
            playerState = PlayerState.Jumping;
        }

        /* Otherwise space key isn't held and player is still jumping, set state to falling to true to trigger
        falling logic */
        else if (!Input.GetKey(KeyCode.Space) && playerState == PlayerState.Jumping)
        {
            playerState = PlayerState.Falling;
        }

        // EXECUTE PLAYER STATES

        /* If player's state is not falling and the player reaches the apex height of the jump, set state to falling
        to make the player fall down */
        if (playerState != PlayerState.Falling && rigidbody2D.velocity.y > apexHeight)
        {
            playerState = PlayerState.Falling;
        }

        // If player is currently jumping, increase player's y velocity
        if (playerState == PlayerState.Jumping)
        {
            rigidbody2D.velocity += new Vector2(0.0f, 1.0f);
        }

        // Else if player is currently falling, decrease player's y velocity via gravity
        else if (playerState == PlayerState.Falling)
        {
            rigidbody2D.velocity += Physics2D.gravity * Time.deltaTime;
        }
    }

    // Let's handle some power up functionality here for organization
    private void PowerUpLogic()
    {
        // If the player's power up is active
        if (powerUpActive)
        {
            // Increment the power up time to determine when the power up will wear off
            powerUpTime += Time.deltaTime;

            // Increase the player's size
            if (rigidbody2D.transform.localScale.x != 1.0f && rigidbody2D.transform.localScale.y != 1.0f)
            {
                rigidbody2D.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }

            // Change the alpha of the interactable blocks to notify the player they can't interact with it until the power up wears off
            if (interactableBlock.gameObject.GetComponent<Tilemap>().color != new Color(1.0f, 1.0f, 1.0f, 0.1f))
            {
                interactableBlock.gameObject.GetComponent<Tilemap>().color = new Color(1.0f, 1.0f, 1.0f, 0.1f);
            }

            // If the power up time exceeds some value, reset power up time to 0 and deactivate the power up
            if (powerUpTime > 4.0f)
            {
                powerUpTime = 0.0f;
                powerUpActive = false;
            }
        }

        // Because if the power up is not active
        else
        {
            // Reset player size to default
            if (rigidbody2D.transform.localScale.x != 0.75f && rigidbody2D.transform.localScale.y != 0.75f)
            {
                rigidbody2D.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            }

            // Reset tilemap's alpha color back to 1
            if (interactableBlock.gameObject.GetComponent<Tilemap>().color != Color.white)
            {
                interactableBlock.gameObject.GetComponent<Tilemap>().color = Color.white;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Player hits ground or the higher ground, set player state to grounded
        if (collision.gameObject.name == "Ground" || collision.gameObject.name == "Higher Ground")
        {
            playerState = PlayerState.Grounded;
        }

        // Make the player fall down after hitting the bottom of the block while jumping to interrupt the jump state
        if (collision.gameObject.name == "Bottom Block Trigger" && playerState != PlayerState.Falling)
        {
            playerState = PlayerState.Falling;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Make sure when the player hits the set interactable block trigger and the power up isn't active yet, activate the power up
        if (collision.gameObject.name == "Interactable Block Trigger" && !powerUpActive)
        {
            powerUpActive = true;
        }

        // If the player hits the interactable block trigger, make the player fall down
        if (collision.gameObject.name == "Interactable Block Trigger" && playerState != PlayerState.Falling)
        {
            playerState = PlayerState.Falling;
        }
    }
}

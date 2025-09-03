using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float moveSpeed = 5.0f;

    [Header("Player Threshold")]
    [SerializeField] private float apexHeight = 10.0f;

    private new Rigidbody2D rigidbody2D;

    private bool isFalling = false;
    private bool isJumping = false;

    private bool hitWall = false;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize components
        rigidbody2D = GetComponent<Rigidbody2D>();
    }
    
    void FixedUpdate()
    {
        // Get horizontal movement by player input
        float horizontalMovement = Input.GetAxis("Horizontal");
        Vector2 movement = new Vector2(horizontalMovement, 0.0f);

        // Make the player jump whenever space key is held and isJumping is false
        if (Input.GetKey(KeyCode.Space) && !isJumping && !isFalling)
        {
            isJumping = true;
        }

        /* Otherwise space key isn't held and isJumping is true, set isJumping false and isFalling to true to
        trigger falling logic */
        else if (!Input.GetKey(KeyCode.Space) && isJumping)
        {
            isJumping = false;
            isFalling = true;
        }

        /* If isFalling is false and the player reaches the apex height of the jump, set isFalling to true to make
        the player fall down and not jump anymore */
        if (!isFalling && rigidbody2D.velocity.y > apexHeight)
        {
            isJumping = false;
            isFalling = true;
        }

        // If player is currently jumping and not falling, increase player's y velocity
        if (isJumping && !isFalling)
        {
            rigidbody2D.velocity += new Vector2(0.0f, 1.0f);
        }

        // Else if player is currently falling and not jumping, decrease player's y velocity via gravity
        else if (isFalling && !isJumping)
        {
            rigidbody2D.velocity += Physics2D.gravity * Time.deltaTime;
        }

        // Update player's position
        rigidbody2D.transform.position += (Vector3)movement * moveSpeed * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Player hits ground, set falling and jumping to false
        if (collision.gameObject.name == "Ground")
        {
            if (isFalling != false) isFalling = false;
            if (isJumping != false) isJumping = false;
        }

        // Player hits wall, set hit wall to true
        if (collision.gameObject.name == "Wall")
        {
            hitWall = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Make player fall if it's not on ground anymore and the player has hit a wall
        if (collision.gameObject.name == "Ground" && hitWall)
        {
            if (isFalling != true) isFalling = true;
            if (isJumping != false) isJumping = false;
        }

        // If player isn't hitting the wall anymore, set hit wall to false
        if (collision.gameObject.name == "Wall")
        {
            hitWall = false;
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private Animator anim;
    [SerializeField] private TrailRenderer tr;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private float groundCheckSize;
    private float wallCheckSize = 0.12f;
    private float checkRadius;

    [SerializeField] private float moveVel = 15f;
    [SerializeField] private float jumpVel = 17f;
    private float dirX = 0f;
    private bool isDoubleJump = true;
    private float gravity;

    private bool isWallGrab = false;
    private bool isWallJump = false;
    private float wallJumpDir;
    private float wallJumptime = 0.2f;
    private float wallJumpCounter;
    private float wallJumpDuration = 0.4f;
    [SerializeField] private Vector2 wallJumpVel = new Vector2(6f, 14f);

    private bool canDash = true;
    private bool isDash = false;
    [SerializeField] private float dashVel = 34f;
    private float dashTime = 0.25f;
    private float dashCooldown = 1f;

    private bool isRoll = false;
    [SerializeField] private float rollModifierX = 1.2f;
    [SerializeField] private float rollModifierY = 0.8f;

    private enum MovementState { idle, running, jumping, falling, wallgrab, rolling, airrolling, idlerolling, dashing }

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        tr = GetComponent<TrailRenderer>();

        checkRadius = coll.bounds.size.x / 2;
        groundCheckSize = coll.bounds.size.y / 2 + wallCheckSize - coll.bounds.size.x / 2;
        gravity = rb.gravityScale;
    }

    // Update is called once per frame
    private void Update()
    {
        if (isDash)
        {
            return;
        }

        if (IsGrounded())
        {
            isDoubleJump = true;
            isWallJump = false;
        } else
        {
            WallGrab();
            WallJump();
        }

        rb.velocity = new Vector2(dirX * moveVel, rb.velocity.y);
        UpdateAnimationState();
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (isDash)
        {
            return;
        }

        dirX = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isDash)
        {
            return;
        }

        if (context.performed)
        {
            if (wallJumpCounter > 0f)
            {
                isWallJump = true;
                rb.velocity = new Vector2(wallJumpDir * wallJumpVel.x, wallJumpVel.y);
                wallJumpCounter = 0f;
                Invoke(nameof(StopWallJump), wallJumpDuration);
            }
            else if ((IsGrounded() || isDoubleJump) && !isWallJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpVel);
                isDoubleJump = !isDoubleJump;
            }
        }

        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.9f);
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash && !isRoll)
        {
            StartCoroutine(Dash());
        }
    }

    // special moves -----------------------------------------------------------

    private void WallGrab()
    {
        if (IsWalled() && !IsGrounded() && !isRoll && (dirX * Utils.flipToInt(sprite.flipX) > 0f))
        {
            isDoubleJump = false;
            isWallGrab = true;
            rb.gravityScale = 0f;
            rb.velocity = (System.Math.Abs(rb.velocity.y) < 0.05f * jumpVel) ? new Vector2(0f, 0f) : new Vector2(0f, System.Convert.ToSingle(rb.velocity.y * 0.5f));        
        } else
        {
            isWallGrab = false;
            rb.gravityScale = gravity;
        }
    }

    private void WallJump()
    {
        if (isWallGrab)
        {
            isWallJump = false;
            wallJumpDir = sprite.flipX ? -1f : 1f;
            wallJumpCounter = wallJumptime;
        }
        else
        {
            wallJumpCounter -= Time.deltaTime;
        }
    }

    private void StopWallJump()
    {
        isWallJump = false;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDash = true;
        anim.SetInteger("state", (int)MovementState.dashing);
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(Utils.flipToInt(sprite.flipX) * dashVel, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashTime);
        tr.emitting = false;
        rb.gravityScale = gravity;
        isDash = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    // rolling -----------------------------------------------------------

    public void Roll(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            if (!isRoll) // start roll
            {
                moveVel *= rollModifierX;
                jumpVel *= rollModifierY;
                isRoll = true;
            }
            else if (!Physics2D.OverlapCircle(Utils.ChangeY(coll.bounds.center, coll.bounds.size.y * 1.5f), checkRadius, groundLayer))
            { // if has 1+ block space above, stop roll
                {
                    moveVel /= rollModifierX;
                    jumpVel /= rollModifierY;
                    rb.position = new Vector2(rb.position.x, rb.position.y + 0.5f);
                    //rb.velocity = new Vector2(rb.velocity.x, 2f); // TODO: this is hacky
                    isRoll = false;
                    UpdateAnimationState();
                }
            }
        }
    }

    // animation state -----------------------------------------------------------

    private void UpdateAnimationState()
    {
        MovementState state;

        if (dirX > 0f) // right
        {
            state = MovementState.running;
            sprite.flipX = false;
        }
        else if (dirX < 0f) // left
        {
            state = MovementState.running;
            sprite.flipX = true;
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.velocity.y > 0.1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -0.1f || !IsGrounded())
        {
            state = MovementState.falling;
        }

        if (isWallGrab)
        {
            state = MovementState.wallgrab;
        }

        if (isRoll)
        {
            if (dirX > 0f || dirX < 0f)
            {
                state = MovementState.rolling;
            } else
            {
                state = MovementState.idlerolling;
            }

            if (rb.velocity.y > 0.1f || rb.velocity.y < -0.1f)
            {
                state = MovementState.airrolling;
            }
        }

        anim.SetInteger("state", (int)state);
    }

    // player checks -----------------------------------------------------------

    private bool IsGrounded()
    {
        if (isRoll)
        {
            return Physics2D.OverlapCircle(Utils.ChangeY(coll.bounds.center, -0.2f), coll.bounds.size.x / 2, groundLayer);
        }
        else
        {
            return Physics2D.OverlapCircle(Utils.ChangeY(coll.bounds.center, -groundCheckSize), checkRadius, groundLayer);
        }
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(Utils.ChangeX(coll.bounds.center, wallCheckSize * Utils.flipToInt(sprite.flipX)), checkRadius, groundLayer);
    }
}

using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found!");
        }
    }

    // Plays the walking animation based on the move direction
    public void PlayWalkAnimation(Vector3 moveDirection)
    {
        ResetWalkAndIdleAnimations();

        if (moveDirection.x > 0 && moveDirection.y > 0) animator.SetBool("NE_Walk", true);
        else if (moveDirection.x > 0 && moveDirection.y < 0) animator.SetBool("SE_Walk", true);
        else if (moveDirection.x > 0) animator.SetBool("E_Walk", true);
        else if (moveDirection.x < 0 && moveDirection.y > 0) animator.SetBool("NW_Walk", true);
        else if (moveDirection.x < 0 && moveDirection.y < 0) animator.SetBool("SW_Walk", true);
        else if (moveDirection.x < 0) animator.SetBool("W_Walk", true);
        else if (moveDirection.y > 0) animator.SetBool("N_Walk", true);
        else if (moveDirection.y < 0) animator.SetBool("S_Walk", true);

        FlipSprite(moveDirection);
    }

    // Plays the idle animation based on the last move direction
    public void PlayIdleAnimation(Vector3 lastMoveDirection)
    {
        ResetWalkAndIdleAnimations();

        if (lastMoveDirection.x > 0 && lastMoveDirection.y > 0) animator.SetBool("NE_Idle", true);
        else if (lastMoveDirection.x > 0 && lastMoveDirection.y < 0) animator.SetBool("SE_Idle", true);
        else if (lastMoveDirection.x > 0) animator.SetBool("E_Idle", true);
        else if (lastMoveDirection.x < 0 && lastMoveDirection.y > 0) animator.SetBool("NW_Idle", true);
        else if (lastMoveDirection.x < 0 && lastMoveDirection.y < 0) animator.SetBool("SW_Idle", true);
        else if (lastMoveDirection.x < 0) animator.SetBool("W_Idle", true);
        else if (lastMoveDirection.y > 0) animator.SetBool("N_Idle", true);
        else if (lastMoveDirection.y < 0) animator.SetBool("S_Idle", true);
    }

    // Helper function to reset all animation states before setting new ones
    private void ResetWalkAndIdleAnimations()
    {
        animator.SetBool("N_Walk", false);
        animator.SetBool("NE_Walk", false);
        animator.SetBool("E_Walk", false);
        animator.SetBool("SE_Walk", false);
        animator.SetBool("S_Walk", false);
        animator.SetBool("SW_Walk", false);
        animator.SetBool("W_Walk", false);
        animator.SetBool("NW_Walk", false);

        animator.SetBool("N_Idle", false);
        animator.SetBool("NE_Idle", false);
        animator.SetBool("E_Idle", false);
        animator.SetBool("SE_Idle", false);
        animator.SetBool("S_Idle", false);
        animator.SetBool("SW_Idle", false);
        animator.SetBool("W_Idle", false);
        animator.SetBool("NW_Idle", false);
    }

    // Flip sprite based on direction
    private void FlipSprite(Vector3 moveDirection)
    {
        if (moveDirection.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveDirection.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }
}

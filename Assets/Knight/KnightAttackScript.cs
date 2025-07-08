using UnityEngine;

public class KnightAttackScript : MonoBehaviour
{
    public BoxCollider2D knightBoxCollider;
    public Animator knightAnimator;
    public AudioSource knightAudio;
    public AudioClip knightSwordSlash, knightAttackGrunt, knightShieldSound, knightBlockGrunt;

    public bool attackingSound = false;
    public bool blockingSound = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        checkAttack(); 
    }

    public void checkAttack() // Check the user's input and accordingly use weapons. 
    {
        if ((Input.GetMouseButtonUp(0) == true) && (knightAnimator.GetBool("isAttacking") == false)) // If <right-click> and not already attacking...
        {
            knightAnimator.SetBool("isAttacking", true); // Play sword animation
            if (attackingSound == false) // If not already playing attacking sounds, then play sounds. 
            {
                attackingSound = true;
                knightAudio.PlayOneShot(knightSwordSlash);
                knightAudio.PlayOneShot(knightAttackGrunt);
            }
        }
        else if ((Input.GetMouseButtonUp(1) == true) && (knightAnimator.GetBool("isBlocking") == false)) // If <left-click> and not already blocking...
        {
            knightAnimator.SetBool("isBlocking", true); // Play block animation
            if (blockingSound == false) // If not already playing blocking sounds, then play sounds. 
            {
                blockingSound = true;
                knightAudio.PlayOneShot(knightShieldSound);
                knightAudio.PlayOneShot(knightBlockGrunt);
            }
        }
        // Revert animator bools to false after animation completed. 
        if (knightAnimator.GetBool("isAttacking") == true)
        {
            AnimatorStateInfo stateInfo = knightAnimator.GetCurrentAnimatorStateInfo(0);
            if ((stateInfo.IsName("KnightAttacking") || stateInfo.IsName("KnightMove&Attack")) && stateInfo.normalizedTime >= 1f)
            {
                knightAnimator.SetBool("isAttacking", false);
                attackingSound = false;
            }
        }
        if (knightAnimator.GetBool("isBlocking") == true)
        {
            AnimatorStateInfo stateInfo = knightAnimator.GetCurrentAnimatorStateInfo(0);
            if ((stateInfo.IsName("KnightBlocking") || stateInfo.IsName("KnightMove&Block")) && stateInfo.normalizedTime >= 1f)
            {
                knightAnimator.SetBool("isBlocking", false);
                blockingSound = false;
            }
        }
    }
}

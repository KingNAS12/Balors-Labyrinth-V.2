using System.Numerics;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine; 
using UnityEngine.InputSystem.Processors;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using NUnit.Framework.Constraints;
using TreeEditor;
using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public class KnightScript : MonoBehaviour
{
    public Rigidbody2D knightRigidBody;
    public SpriteRenderer knightSpriteRenderer;
    public Animator knightAnimator;
    public BoxCollider2D knightBody;

    public AudioSource knightAudio;
    public AudioClip knightWalk, knightSwordSlash, knightAttackGrunt, knightShieldSound, knightBlockGrunt; 

    public float walkSpeed = 7f;
    public float turnSpeed = 1f;
    public bool walkingSound = false;
    public bool attackingSound = false;
    public bool blockingSound = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() 
    {
        knightAudio.volume = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        checkMove(); 
        checkAttack();
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
                transform.Translate(new UnityEngine.Vector2(-turnSpeed*Time.deltaTime, 0));
            }
            else
            {
                transform.Translate(new UnityEngine.Vector2(turnSpeed*Time.deltaTime, 0));
            }
            transform.localScale = new UnityEngine.Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
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
        transform.Translate(new UnityEngine.Vector2(x1*Time.deltaTime, y1*Time.deltaTime));
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

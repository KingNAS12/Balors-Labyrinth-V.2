using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using NUnit.Framework.Constraints;
using TreeEditor;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyPatrolScript : MonoBehaviour
{
    public float attackCooldown;
    public float attackCooldownTimer = 0;
    public float followCooldown;
    public float followCooldownTimer = 0;
    public int horSpeed;
    public int vertSpeed;
    public BoxCollider2D bodyBoxCollider; 
    public BoxCollider2D attackBoxCollider;
    public BoxCollider2D sightBoxCollider;
    public float sibSepDist; 
    public LayerMask playerLayer;
    public LayerMask backgroundLayer;
    public LayerMask enemyLayer;
    public GameObject Player;
    public float x0; 
    public float y0;
    public Component oldWallHit = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Save original coordinates. 
        x0 = transform.position.x;
        y0 = transform.position.y;
        attackBoxCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();
        sightBoxCollider = transform.GetChild(1).GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerInRange())
        {
            followCooldownTimer = 0; // Reset follow cooldown timer, before activating/incrementing it. If you don't reset, then it might accidently passes cooldown limit. 
            followCooldownTimer += Time.deltaTime;
            attackCooldownTimer += Time.deltaTime;
            if (attackCooldownTimer >= attackCooldown && !NearOtherEnemy())
            {
                attackCooldownTimer = 0;
                Attack();
            }
        }
        else
        {
            if (PlayerInSight())
            {
                Follow();
            }
            else
            {
                Patrol();
            }
        }
    }


    // Moving functions

    private void Patrol() // Make enemy patrol through hallway. 
    {
        Component newWallHit = ReachedHorWall(); 
        if (newWallHit != null && newWallHit != oldWallHit) // Flip x if enemy has hit a new wall. 
        {
            oldWallHit = newWallHit;
            transform.localScale = new UnityEngine.Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z); // Turn around
        }
        float x1 = 0;
        float y1 = 0;
        if (transform.localScale.x > 0) // If enemy is facing right; then move right. 
        {
            x1 = horSpeed;
        }
        else // If enemy is facing left; then move left. 
        {
            x1 = -horSpeed;
        }
        if (transform.position.y < y0) // If enemy is below original y; then move up. 
        {
            y1 = vertSpeed;
        }
        else if (transform.position.y > y0) // If enemy is above original y; then move down. 
        {
            y1 = -vertSpeed;
        }
        transform.Translate(new UnityEngine.Vector2(x1 * Time.deltaTime, y1 * Time.deltaTime)); // Move enemy. 
        if (math.abs(transform.position.y - y0) < (vertSpeed * Time.deltaTime)) // Telerport to original y if close enough. 
        {
            transform.position = new UnityEngine.Vector2(transform.position.x, y0); 
        }
    }

    private void Follow() // Make enemy follow player. 
    {
        float x1 = 0;
        float y1 = 0;
        // Check if enemy needs to change directions. 
        if (transform.position.x > Player.transform.position.x && transform.localScale.x > 0) // Enemy is facing right, but player is left.  
        {
            transform.localScale = new UnityEngine.Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            oldWallHit = null;
        }
        else if (transform.position.x < Player.transform.position.x && transform.localScale.x < 0) // Enemy is facing left, but player is right. 
        {
            transform.localScale = new UnityEngine.Vector3(math.abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            oldWallHit = null;
        }
        if (followCooldownTimer > 0) // If follow cooldown timer had been previously activated, then increment it. 
        {
            followCooldownTimer += Time.deltaTime;
        }
        else
        {
            if (!NearOtherEnemy()) // If enemy is not near another enemy then, check how to move. 
            {
                // x position checks
                if (math.abs(Player.transform.position.x - transform.position.x) <= attackBoxCollider.size.x) // If player is within attack range, but not in the attack collider, then they are probably above/below enemy; so pause movement.
                {
                    followCooldownTimer += Time.deltaTime;
                }
                else
                {
                    if (transform.localScale.x > 0) // Enemy is moving right. 
                    {
                        x1 = horSpeed;
                    }
                    else // Enemy is moving left. 
                    {
                        x1 = -horSpeed;
                    }
                }
                // y position checks
                Component wallHit = ReachedVertWall();
                if (wallHit != null) // If the enemy has hit a wall above/below it (because Player moved into a different hallway but is still within sight box i.e behing a wall), then reset enemy's y position. 
                {
                    if (transform.position.y < y0) // If enemy is below original y, then move up. 
                    {
                        y1 = vertSpeed * 2;
                    }
                    else if (transform.position.y > y0) // If enemy is above original y, then move down. 
                    {
                        y1 = -vertSpeed * 2;
                    }
                }
                else // If Player is still in the same hallway, then move towards player. 
                {
                    if (transform.position.y < Player.transform.position.y) // Player is above enemy. 
                    {
                        y1 = vertSpeed;
                    }
                    else if (transform.position.y > Player.transform.position.y) // Player is below enemy. 
                    {
                        y1 = -vertSpeed;
                    }
                }
            }
        }
        if (followCooldownTimer == 0 || followCooldownTimer > followCooldown) // If cooldown timer is activated, do not move. Once timer reaches cooldown limit, then reset timer and move. 
        {
            followCooldownTimer = 0;
            transform.Translate(new UnityEngine.Vector2(x1 * Time.deltaTime, y1 * Time.deltaTime));
        }
    }


    // Attacking functions

    private void Attack()
    {
        print("Attack");
        Animator knightAnimator = Player.GetComponent<Animator>(); 
        if (knightAnimator.GetBool("isBlocking"))
        {
            print("Blocked"); 
        }
        else
        {
            print("Damage");
        }
    }


    // Hit collider checks

    private bool PlayerInRange() // Return true if player is within attacking range. 
    {
        RaycastHit2D hit = Physics2D.BoxCast(attackBoxCollider.bounds.center, new Vector3(attackBoxCollider.bounds.size.x, attackBoxCollider.bounds.size.y, attackBoxCollider.bounds.size.z), 0, Vector2.left, 0, playerLayer);
        return hit.collider != null;
    }

    private bool PlayerInSight() // Return true if player is within sight box. 
    {
        RaycastHit2D hit = Physics2D.BoxCast(sightBoxCollider.bounds.center, new Vector3(sightBoxCollider.bounds.size.x, sightBoxCollider.bounds.size.y, sightBoxCollider.bounds.size.z), 0, Vector2.left, 0, playerLayer);
        return hit.collider != null;
    }

    private Component ReachedHorWall() // Return true if enemy has hit a wall while moving left or right. 
    {
        RaycastHit2D hit = Physics2D.BoxCast(bodyBoxCollider.bounds.center, new Vector3(bodyBoxCollider.bounds.size.x, bodyBoxCollider.bounds.size.y, bodyBoxCollider.bounds.size.z), 0, Vector2.left, 0, backgroundLayer);
        return hit.collider;
    }

    private Component ReachedVertWall() // Return true if enemy has hit a wall while moving up or down. 
    {
        RaycastHit2D hit = Physics2D.BoxCast(bodyBoxCollider.bounds.center, new Vector3(bodyBoxCollider.bounds.size.x, bodyBoxCollider.bounds.size.y, bodyBoxCollider.bounds.size.z), 0, Vector2.up, 0, backgroundLayer);
        return hit.collider;
    }

    private bool NearOtherEnemy() // Return true if enemy is not near another enemy. If it is near another enemy, then the enemy which spawned first (older sibling) returns false while the other returns true. 
    {
        Transform enemyParent = transform.parent; // Self's parent. 
        int selfIndex = transform.GetSiblingIndex(); // Self's index relative to its siblings. 
        for (int i = 0; i < selfIndex; i++) // Start from oldest/first child, end at self. Self only stops moving if it is near an older sibling. It ignores younger siblings. 
        {
            Transform sibling = enemyParent.GetChild(i); // enemyParent's children are self's siblings.
            Vector2 sibPos = sibling.position;
            if ((math.abs(transform.position.x - sibPos.x) <= sibSepDist) || (math.abs(transform.position.y - sibPos.y) <= sibSepDist)) // If the distance between self and sibling is too small. 
            {
                return true;
            }
        }
        return false;
    }
}

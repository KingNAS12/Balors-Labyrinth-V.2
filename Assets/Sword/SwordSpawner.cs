using UnityEngine;

public class SwordScript : MonoBehaviour
{
    public GameObject swordPrefab;
    public GameObject swordObject;
    public Animator knightAnimator; 
    public bool isSpawned = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (knightAnimator.GetBool("isAttacking") == true && isSpawned == false)
        {
            // knightAnimator.GetBool("isAttacking") == true
            swordObject = Instantiate(swordPrefab, transform.position, Quaternion.identity, transform.parent);
            isSpawned = true;
        }
        else if (knightAnimator.GetBool("isAttacking") == false && isSpawned == true) 
        {
            Destroy(swordObject);
            isSpawned = false; 
        }
    }
}

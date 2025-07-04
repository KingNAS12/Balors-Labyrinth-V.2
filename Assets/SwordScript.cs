using UnityEngine;

public class SwordScript : MonoBehaviour
{
    public Animator knightAnimator;
    public PolygonCollider2D swordCollider; 
    private ContactFilter2D contactFilter;
    private Collider2D[] results = new Collider2D[10];
    ContactFilter2D filter = new ContactFilter2D();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = false;
        //swordCollider.excludeLayers(0); 
    }

    // Update is called once per frame
    void Update()
    {
        if (knightAnimator.GetBool("isAttacking") == true)
        {
            filter.SetLayerMask(1);
            /*
            int hitCount = swordCollider.Overlap(contactFilter, results);
            for (int i = 0; i < hitCount; i++)
            {
                Debug.Log("Overlapping with: " + results[i].name);
            } */
        }
        else
        {
            filter.SetLayerMask(0);
        }
    }
}

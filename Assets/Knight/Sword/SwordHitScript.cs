using UnityEngine;

public class SwordHitScript : MonoBehaviour
{
    public PolygonCollider2D swordCollider;
    public LayerMask enemyLayer; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(enemyLayer);
        filter.useTriggers = false; // or true if your colliders are triggers
        Collider2D[] results = new Collider2D[10];
        int hitCount = swordCollider.Overlap(filter, results);

        if (hitCount > 0)
        {
            print("Sword is hitting something in the Enemy layer!");

            for (int i = 0; i < hitCount; i++)
            {
                print("Hit: " + results[i].name);
            }
        }
        else
        {
            print("no"); 
        }
    }
}

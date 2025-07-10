using Unity.VisualScripting;
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
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        print("sword hit = " + other + " " + other.gameObject.layer);
        if (other.gameObject.layer == enemyLayer)
        {
            Destroy(other);
        }
    }
}

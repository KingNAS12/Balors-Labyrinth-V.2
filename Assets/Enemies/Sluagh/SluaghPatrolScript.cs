using UnityEngine;

public class SluaghPatrolScript : MonoBehaviour
{
    public BoxCollider2D bodyBoxCollider;
    public LayerMask backgroundLayer;
    public float speed = -7;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector2(0, speed*Time.deltaTime));
        RaycastHit2D hit = Physics2D.BoxCast(bodyBoxCollider.bounds.center, new Vector3(bodyBoxCollider.bounds.size.x, bodyBoxCollider.bounds.size.y, bodyBoxCollider.bounds.size.z), 0, Vector2.up, 0, backgroundLayer);
        if (hit.collider == true)
        {
            Destroy(gameObject); 
        }
    }
}

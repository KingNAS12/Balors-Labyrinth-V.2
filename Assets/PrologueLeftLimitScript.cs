using UnityEngine;

public class PrologueLeftLimitScript : MonoBehaviour
{
    public int playerLayer = 3; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision); 
        if (collision.gameObject.layer == playerLayer)
        {
            collision.transform.position = new Vector2(0, collision.transform.position.y);
        }
    }
}

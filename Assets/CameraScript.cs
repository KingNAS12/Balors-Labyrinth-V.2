using UnityEngine;
using UnityEngine.UIElements;

public class CameraScript : MonoBehaviour
{
    public float followSpace; // Distance which player can move before the camera starts following. 
    public float playerSpeed = 7;
    public GameObject Player; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new UnityEngine.Vector3(Player.transform.position.x, Player.transform.position.y, -10); // Set camera's position to Knight's position. 
    }

    // Update is called once per frame
    void Update()
    {
        if ((Player.transform.position.x - transform.position.x) > (followSpace * Time.deltaTime)) // Camera is too far left; move right. 
        {
            transform.Translate(new UnityEngine.Vector2(playerSpeed * Time.deltaTime, 0)); 
        }
        else if ((Player.transform.position.x - transform.position.x) < -(followSpace * Time.deltaTime)) // Camera is too far right; move left. 
        {
            transform.Translate(new UnityEngine.Vector2(-playerSpeed * Time.deltaTime, 0));
        }
        if ((Player.transform.position.y - transform.position.y) > (followSpace * Time.deltaTime)) // Camera is too far down; move up. 
        {
            transform.Translate(new UnityEngine.Vector2(0, playerSpeed * Time.deltaTime));
        }
        else if ((Player.transform.position.y - transform.position.y) < -(followSpace * Time.deltaTime)) // Camera is too far up; move down. 
        {
            transform.Translate(new UnityEngine.Vector2(0, -playerSpeed * Time.deltaTime));
        }
    }
}

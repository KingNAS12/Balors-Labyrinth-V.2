using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawnScript : MonoBehaviour
{
    public GameObject WaterLeaperPrefab;
    public GameObject[] WaterLeaperObjects = new GameObject[3];
    public GameObject Player;
    public int[] state = new int[3]; // 0 = Not spawned and alive; 1 = Spawned and alive; 2 = Dead

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.Find("Knight");
        for (int i = 0; i < 3; i++)
        {
            state[i] = 0; 
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            if (state[i] == 1) // If the enemy is supposed to be spawned and alive.....
            {
                if (WaterLeaperObjects[i].IsDestroyed()) // But the game object is destroyed...
                {
                    state[i] = 2; // That means it has been killed. 
                }
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == Player.name) // The player has entered the spawn area. 
        {
            for (int i = 0; i < 3; i++)
            {
                print("state " + i + " " + state[i]); 
                // Check if enemy is alive before spawning. 
                if (state[i] == 0) // Never been spawned before. 
                {
                    WaterLeaperObjects[i] = Instantiate(WaterLeaperPrefab, new Vector3(transform.position.x + (i * 1.5f), transform.position.y + (i * 1.5f), transform.position.z), Quaternion.identity, transform);
                    state[i] = 1;
                }
                else if (state[i] == 1) // Has been spawned before. 
                {
                    WaterLeaperObjects[i] = Instantiate(WaterLeaperPrefab, new Vector3(transform.position.x + (i * 1.5f), transform.position.y + (i * 1.5f), transform.position.z), Quaternion.identity, transform); 
                }
            }
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == Player.name) // The player has exited the spawn area. 
        {
            for (int i = 0; i < 3; i++)
            {
                if (state[i] == 1) // Check if enemy is still alive and has not already been destroyed. 
                {
                    Destroy(WaterLeaperObjects[i]);
                    state[i] = 0; 
                }
            }
        }
    }
}

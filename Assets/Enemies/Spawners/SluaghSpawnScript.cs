using Unity.VisualScripting;
using UnityEngine;

public class SluaghSpawnScript : MonoBehaviour
{
    public GameObject sluaghPrefab;
    public GameObject Player;
    public float spawnCooldown = 1.5f;
    public float spawnCooldownTimer = 0;
    public bool canSpawn = false; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.Find("Knight");
    }

    // Update is called once per frame
    void Update()
    {
        if (canSpawn)
        {
            spawnCooldownTimer += Time.deltaTime;
            if (spawnCooldownTimer > spawnCooldown)
            {
                spawnCooldownTimer = 0;
                int side = UnityEngine.Random.Range(0, 2); 
                if (side == 0)
                {
                    Instantiate(sluaghPrefab, new Vector3(transform.position.x - 1, transform.position.y, transform.position.z), Quaternion.identity, transform);
                }
                else
                {
                    Instantiate(sluaghPrefab, new Vector3(transform.position.x + 1, transform.position.y, transform.position.z), Quaternion.identity, transform);
                }
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == Player.name) // The player has entered the spawn area. 
        {
            canSpawn = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == Player.name) // The player has entered the spawn area. 
        {
            canSpawn = false;
        }
    }
}

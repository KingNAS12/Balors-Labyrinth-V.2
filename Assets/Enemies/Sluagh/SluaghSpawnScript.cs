using UnityEngine;

public class SluaghSpawnScript : MonoBehaviour
{
    public GameObject sluaghPrefab;
    public bool rightSide = true;
    public float spawnCooldown = 0;
    public float spawnCooldownTimer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        spawnCooldownTimer += Time.deltaTime;
        if (spawnCooldownTimer > spawnCooldown)
        {
            spawnCooldownTimer = 0;
            if (rightSide)
            {
                Instantiate(sluaghPrefab, new Vector3(transform.position.x-1, transform.position.y, transform.position.z), Quaternion.identity, transform.parent);
                rightSide = false; 
            }
            else
            {
                Instantiate(sluaghPrefab, new Vector3(transform.position.x+1, transform.position.y, transform.position.z), Quaternion.identity, transform.parent);
                rightSide = true;
            }
        }
    }
}

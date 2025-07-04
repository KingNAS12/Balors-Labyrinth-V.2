using UnityEngine;

public class MusicScript : MonoBehaviour
{
    public AudioSource musicAudio;
    public AudioClip mainTheme, dungeonTheme; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Play the main theme on loop. 
        musicAudio.clip = dungeonTheme; 
        musicAudio.Play();
        musicAudio.loop = true; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

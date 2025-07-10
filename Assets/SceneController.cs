using UnityEngine;
using UnityEngine.SceneManagement; 

public class SceneController : MonoBehaviour
{
    public AudioSource musicAudio;
    public AudioClip mainTheme, dungeonTheme;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicAudio.volume = 0.5f; 
        musicAudio.clip = mainTheme;
        musicAudio.Play();
        musicAudio.loop = true;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("Prologue");
    }
}

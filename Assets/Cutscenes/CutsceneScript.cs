using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CutsceneScript : MonoBehaviour
{
    public GameObject[] scrolls = new GameObject[10];
    public GameObject currentScroll = null;
    public float scrollOffsetX = 10;
    public float scrollOffsetY = 3;
    public Vector2 position; 
    public AudioSource cutsceneAudio; 
    public AudioClip[] dialogues = new AudioClip[10];
    public int dialogueLine = -1;
    public AudioSource backgroundMusic;
    public AudioClip dungeonTheme;
    public GameObject skipButtonPrefab;
    public float skipOffsetX = -1.5f;
    public float skipOffsetY = 0.5f;
    public int playerLayer = 3;
    public Camera camera;
    public SceneControllerScript sceneController;
    public bool isPaused = false; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cutsceneAudio.loop = false;
        cutsceneAudio.volume = 0.5f;
        backgroundMusic.loop = false;
        backgroundMusic.volume = 0.5f;
        backgroundMusic.clip = dungeonTheme;
        backgroundMusic.loop = true;  
        backgroundMusic.Play(); 
        camera = Camera.main;
        sceneController = GameObject.Find("SceneController").GetComponent<SceneControllerScript>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (cutsceneAudio.isPlaying == false && !isPaused)
        {
            dialogueLine++;
            if (scrolls[dialogueLine] == null)
            {
                EndScene();
            }
            else
            {
                NextScene(dialogueLine);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == playerLayer && dialogueLine == -1)
        {
            StartCutscene();
        }
    }

    public void StartCutscene()
    {
        sceneController.PauseGame();
        isPaused = false; 
        float vertExtent = camera.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;
        Vector3 camPos = camera.transform.position;
        float leftEdge = camPos.x - horzExtent;
        float rightEdge = camPos.x + horzExtent;
        float bottomEdge = camPos.y - vertExtent;
        //float topEdge = camPos.y + vertExtent;
        position = new Vector2(leftEdge + scrollOffsetX, bottomEdge + scrollOffsetY); 
        currentScroll = Instantiate(scrolls[0], position, Quaternion.identity, transform);
        dialogueLine = 0; 
        cutsceneAudio.clip = dialogues[0];
        backgroundMusic.volume = 0.3f;
        cutsceneAudio.Play();
        Instantiate(skipButtonPrefab, new Vector2(rightEdge + skipOffsetX, bottomEdge + skipOffsetY), Quaternion.identity, transform);
    }

    public void NextScene(int scene)
    {
        Destroy(currentScroll); 
        currentScroll = Instantiate(scrolls[dialogueLine], position, Quaternion.identity, transform);
        cutsceneAudio.clip = dialogues[dialogueLine];
        cutsceneAudio.Play();
    }

    public void EndScene()
    {
        Destroy(currentScroll);
        dialogueLine = 10; 
        sceneController.ResumeGame(); 
    }

    public void PauseScene()
    {
        isPaused = true;
        backgroundMusic.Stop();
        cutsceneAudio.Stop();
        dialogueLine--;
    }

    public void ResumeScene()
    {
        isPaused = false;
        backgroundMusic.Play();
    }
}

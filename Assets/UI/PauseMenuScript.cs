using UnityEngine;

public class PauseMenuScript : MonoBehaviour
{
    SceneControllerScript SceneController; 
    PauseButtonScript PauseButtonScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneController = GameObject.Find("SceneController").GetComponent<SceneControllerScript>();
        PauseButtonScript = GameObject.Find("PlayCanvas").GetComponent<PauseButtonScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResumeGame()
    {
        PauseButtonScript.Pause();
    }

    public void ExitGame()
    {
        SceneController.ExitGame();
    }
}

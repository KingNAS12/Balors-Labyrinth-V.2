using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseButtonScript : MonoBehaviour
{
    public SceneControllerScript SceneController;
    public GameObject PauseMenuPrefab;
    public GameObject PauseMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneController = GameObject.Find("SceneController").GetComponent<SceneControllerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) == true)
        {
            Pause(); 
        }
    }

    public void Pause()
    {
        SceneController = GameObject.Find("SceneController").GetComponent<SceneControllerScript>(); 
        if (SceneController.isPaused) 
        {
            SceneController.ResumeGame();
            Destroy(PauseMenu);
        }
        else
        {
            SceneController.PauseGame();
            PauseMenu = Instantiate(PauseMenuPrefab);
        }
    }
}

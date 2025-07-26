using UnityEditor.SearchService;
using UnityEngine;

public class SceneControllerScript : MonoBehaviour
{
    public KnightMoveScript KnightMoveScript;
    public CutsceneScript CutsceneScript; 
    public bool isPaused = false; 

    private void Start()
    {
        KnightMoveScript = GameObject.Find("Knight").GetComponent<KnightMoveScript>();
        CutsceneScript = GameObject.Find("Cutscene").GetComponent<CutsceneScript>();
    }

    public void PauseGame()
    {
        isPaused = true; 
        KnightMoveScript.pauseMovement();
        if (CutsceneScript.dialogueLine != -1 && CutsceneScript.dialogueLine != 10)
        {
            CutsceneScript.PauseScene();
        }
    }

    public void ResumeGame()
    {
        isPaused = false; 
        KnightMoveScript.resumeMovement();
        if (CutsceneScript.dialogueLine != -1 && CutsceneScript.dialogueLine != 10)
        {
            CutsceneScript.ResumeScene();
        }
    }
}

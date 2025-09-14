using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControllerScript : MonoBehaviour
{
    public KnightMoveScript KnightMoveScript;
    public KnightAttackScript KnightAttackScript; 
    public CutsceneScript CutsceneScript; 
    public bool isPaused = false;
    public GameObject PauseMenuPrefab;
    public GameObject PauseMenu;

    private void Start()
    {
        KnightMoveScript = GameObject.Find("Knight").GetComponent<KnightMoveScript>();
        KnightAttackScript = GameObject.Find("Knight").GetComponent<KnightAttackScript>();
        CutsceneScript = GameObject.Find("Cutscene").GetComponent<CutsceneScript>();
    }

    public void PauseGame()
    {
        isPaused = true; 
        KnightMoveScript.pauseMovement();
        KnightAttackScript.pauseAttack();
        if (CutsceneScript.dialogueLine != -1 && CutsceneScript.dialogueLine != 10)
        {
            CutsceneScript.PauseScene();
        }
        PauseMenu = Instantiate(PauseMenuPrefab); 
    }

    public void ResumeGame()
    {
        print("resume"); 
        isPaused = false; 
        if (CutsceneScript.dialogueLine != -1 && CutsceneScript.dialogueLine != 10)
        {
            CutsceneScript.ResumeScene();
        } 
        else
        {
            KnightMoveScript.resumeMovement();
            KnightAttackScript.resumeAttack();
        }
        Destroy(PauseMenu);
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

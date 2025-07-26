using UnityEngine;

public class SkipCutsceneScript : MonoBehaviour
{
    public CutsceneScript CutsceneScript; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CutsceneScript = GameObject.Find("Cutscene").GetComponent<CutsceneScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space) == true)
        {
            NextDialogue(); 
        }
    }

    public void OnButtonClick()
    {
        NextDialogue();
    }

    public void NextDialogue()
    {
        CutsceneScript.cutsceneAudio.Stop(); 
    }
}

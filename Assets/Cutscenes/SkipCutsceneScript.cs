using UnityEngine;

public class SkipCutsceneScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space) == true)
        {
            Skip(); 
        }
    }

    public void Skip()
    {
        GameObject.Find("Cutscene").GetComponent<CutsceneScript>().cutsceneAudio.Stop(); 
    }
}

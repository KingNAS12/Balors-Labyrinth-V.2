using UnityEngine;
using UnityEngine.UIElements;

public class PrologueSceneScript : MonoBehaviour
{
    public GameObject scroll1, scroll2, scroll3, scroll4, scroll5, scroll6, scroll7, scroll8, scroll9;
    public GameObject currentScroll = null;
    public AudioSource cutsceneAudio; 
    public AudioClip dialogue1, dialogue2, dialogue3, dialogue4, dialogue5, dialogue6, dialogue7, dialogue8;
    public int scene = 0;
    public Vector3 position; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cutsceneAudio.loop = false;
        position = GameObject.Find("Main Camera").transform.position; 
    }

    // Update is called once per frame
    void Update()
    {
        if (cutsceneAudio.isPlaying == false)
        {
            if (scene == 1)
            {
                Prologue2(); 
            }
            else if (scene == 2)
            {
                Prologue3();
            }
            else if (scene == 3)
            {
                Prologue4();
            }
            else if (scene == 4)
            {
                Prologue5();
            }
            else if (scene == 5)
            {
                Prologue6();
            }
            else if (scene == 6)
            {
                Prologue7();
            }
            else if (scene == 7)
            {
                Prologue8();
            }
            else if (scene == 8)
            {
                PrologueEnd();
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        print("in"); 
        Prologue1(); 
    }

    public void Prologue1()
    {
        currentScroll = Instantiate(scroll1, position, Quaternion.identity);
        scene = 1; 
        cutsceneAudio.clip = dialogue1;
        cutsceneAudio.Play();
    }

    public void Prologue2()
    {
        Destroy(currentScroll); 
        currentScroll = Instantiate(scroll2, position, Quaternion.identity);
        scene = 2;
        cutsceneAudio.clip = dialogue2;
        cutsceneAudio.Play();
    }

    public void Prologue3()
    {
        Destroy(currentScroll);
        currentScroll = Instantiate(scroll3, position, Quaternion.identity);
        scene = 3;
        cutsceneAudio.clip = dialogue3;
        cutsceneAudio.Play();
    }

    public void Prologue4()
    {
        Destroy(currentScroll);
        currentScroll = Instantiate(scroll4, position, Quaternion.identity);
        scene = 4;
        cutsceneAudio.clip = dialogue4;
        cutsceneAudio.Play();
    }

    public void Prologue5()
    {
        Destroy(currentScroll);
        currentScroll = Instantiate(scroll5, position, Quaternion.identity);
        scene = 5;
        cutsceneAudio.clip = dialogue5;
        cutsceneAudio.Play();
    }

    public void Prologue6()
    {
        Destroy(currentScroll);
        currentScroll = Instantiate(scroll6, position, Quaternion.identity);
        scene = 6;
        cutsceneAudio.clip = dialogue6;
        cutsceneAudio.Play();
    }

    public void Prologue7()
    {
        Destroy(currentScroll);
        currentScroll = Instantiate(scroll7, position, Quaternion.identity);
        scene = 7;
        cutsceneAudio.clip = dialogue7;
        cutsceneAudio.Play();
    }

    public void Prologue8()
    {
        Destroy(currentScroll);
        currentScroll = Instantiate(scroll8, position, Quaternion.identity);
        scene = 8;
        cutsceneAudio.clip = dialogue8;
        cutsceneAudio.Play();
    }

    public void Prologue9()
    { /*
        Destroy(currentScroll);
        currentScroll = Instantiate(scroll9, transform.parent.transform.position, Quaternion.identity);
        scene = 9;
        cutsceneAudio.clip = dialogue9;
        cutsceneAudio.Play(); */
    }

    public void PrologueEnd()
    {
        Destroy(transform.gameObject);
    }
}

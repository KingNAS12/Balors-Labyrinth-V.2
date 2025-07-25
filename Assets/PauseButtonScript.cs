using UnityEngine;
using UnityEngine.UIElements;

public class PauseButtonScript : MonoBehaviour
{
    public Camera camera;
    public float xOffset = -0.5f;
    public float yOffset = 0.5f;
    public KnightMoveScript KnightMoveScript; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camera = Camera.main;
        float vertExtent = camera.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;
        Vector2 camPos = camera.transform.position;
        //float leftEdge = camPos.x - horzExtent;
        float rightEdge = camPos.x + horzExtent;
        float bottomEdge = camPos.y - vertExtent;
        //float topEdge = camPos.y + vertExtent;
        transform.position = new Vector2(rightEdge + xOffset, bottomEdge + yOffset);
        KnightMoveScript = GameObject.Find("Knight").GetComponent<KnightMoveScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape) == true)
        {
            KnightMoveScript.pauseMovement();
        }
    }

    public void OnButtonClick()
    {
        KnightMoveScript.pauseMovement();
    }
}

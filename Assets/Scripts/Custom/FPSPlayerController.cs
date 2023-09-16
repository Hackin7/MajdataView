using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSPlayerController : MonoBehaviour
{

    // Controlling Player ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Translation //////////////////////////////
    public float speed = 10.0f;
    private float translation, straffe, lift;
    void MovementUpdate()
    {
        // Input.GetAxis() is used to get the user's input
        // You can furthor set it on Unity. (Edit, Project Settings, Input)
        translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        straffe = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Space))
        {
            lift = 1.0f;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            lift = -1.0f;
        }
        lift = lift * speed * Time.deltaTime;

        transform.Translate(straffe, lift, translation);
    }

    // MouseCamLook /////////////////////
    [SerializeField]
    public float sensitivity = 5.0f;
    [SerializeField]
    public float smoothing = 2.0f;
    // the chacter is the capsule
    public GameObject character;
    public GameObject camera;
    // get the incremental value of mouse moving
    private Vector2 mouseLook;
    // smooth the mouse moving
    private Vector2 smoothV;
    void MouseLookUpdate()
    {
        // md is mosue delta
        var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        // the interpolated float result between the two float values
        smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing);
        // incrementally add to the camera look
        mouseLook += smoothV;

        // vector3.right means the x-axis
        camera.transform.localRotation = Quaternion.AngleAxis(Mathf.Clamp(-mouseLook.y, -60f, 60f), Vector3.right);
        character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, Vector3.up);
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    // Start is called before the first frame update
    void Start()
    {
        Pause();
        //character = this.transform.parent.gameObject;
    }

    private bool paused = false;
    //public GameObject Canvas;
    // inspired by https://answers.unity.com/questions/811686/how-do-i-toggle-my-pause-menu-with-escape.html
    private void Pause()
    {
        paused = !paused;
        Time.timeScale = paused ? 0.0f : 1.0f;
        //Canvas.gameObject.SetActive(paused);
        Cursor.visible = paused;
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKeyDown("escape"))
        {
            // turn on the cursor
            Cursor.lockState = CursorLockMode.None;
            Pause();
        }
        if (!paused)
        {
            MovementUpdate();
            MouseLookUpdate();
        }

    }

}

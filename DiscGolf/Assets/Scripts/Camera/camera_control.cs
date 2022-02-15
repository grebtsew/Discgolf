using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum mode
{
    FREE_CAM,
   
    LOCKED
    
}


/*
KEY SET for this camera controller:
(0-9) - Change transform and lock on other camera position in scene
c - move to next camera pos

l - locked focus mode
h - locked mode
v - free mode
b - follow mode


a,s,d,w + arrows - movement
shift - higher speed

e,q,pageup,pagedown - movement up/down

z,x + scroll - zoom in/out

*/
public class camera_control : MonoBehaviour
{

    // TODO: add full mouse controls all modes
    // TODO: zoom z,x in all modes

    public float movementSpeed = 0.5f;
    public float fastMovementSpeed = 5f;
    public float freeLookSensitivity = 3f;
    public float zoomSensitivity = 0.5f;
    public float fastZoomSensitivity = 5f;
    private bool looking = false;
    public string camera_name;
    public mode camera_mode = mode.FREE_CAM;
 
    public float slerpSpeed = 100f;
    private Camera[] cam_list;
    private int camera_index = 0;

    private Camera camera;
    private Vector3 lock_offset;

    private Transform temp_main_camera_transform;
    //private Vector3 temp_main_camera_pos;
    //private Quaternion temp_main_camera_rot;
    private mode temp_main_camera_mode;

    public UI_Listener ui_listener;

    private bool movingToCamera = false;
    private Transform cameraTarget;
    public int initCameraIndex= 0;

    // Start is called before the first frame update
    void Start()
    {
        ui_listener =(UI_Listener)FindObjectsOfType<UI_Listener>()[0];
  
        // Collect a list of all cameras in scene
        cam_list = Camera.allCameras;
        camera = gameObject.GetComponent<Camera>();
        camera_name = camera.name;

        // Initial update
        ui_listener.NotifyAll(camera_name, camera_mode.ToString());
       
        set_camera(initCameraIndex);

    }

    private void handle_free_cam()
    {
        var fastMode = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        var movementSpeed = fastMode ? this.fastMovementSpeed : this.movementSpeed;
        var zoomSensitivity = fastMode ? this.fastZoomSensitivity : this.zoomSensitivity;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position = transform.position + (-transform.right * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.position = transform.position + (transform.right * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            transform.position = transform.position + (transform.forward * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            transform.position = transform.position + (-transform.forward * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.position = transform.position + (transform.up * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.position = transform.position + (-transform.up * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.PageUp))
        {
            transform.position = transform.position + (Vector3.up * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.PageDown))
        {
            transform.position = transform.position + (-Vector3.up * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Z))
        {
            transform.position = transform.position + (-Vector3.forward * zoomSensitivity * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.X))
        {
            transform.position = transform.position + (Vector3.forward * zoomSensitivity * Time.deltaTime);
        }

        if (looking)
        {
            float newRotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * freeLookSensitivity;
            float newRotationY = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * freeLookSensitivity;
            transform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);
        }

        float axis = Input.GetAxis("Mouse ScrollWheel");
        if (axis != 0)
        {
            
            transform.position = transform.position + transform.forward * axis * zoomSensitivity;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            StartLooking();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            StopLooking();
        }
    }

    public void set_next_camera()
    {
        if (camera_index + 1 < cam_list.Length)
        {
            camera_index++;
        }
        else
        {
            camera_index = 0;
        }
        set_camera(camera_index);
    }

    public void set_prev_camera()
    {
        if (camera_index - 1 > 0)
        {
            camera_index--;
        }
        else
        {
            camera_index = cam_list.Length-1;
        }
        set_camera(camera_index);
    }

    private void set_mode(mode mode){
        camera_mode = mode;
        ui_listener.NotifyAll(camera_name, camera_mode.ToString());
    }



    public void set_next_mode()
    {
        if ((int)camera_mode == Enum.GetValues(typeof(mode)).Length-1){
                set_mode( (mode)Enum.GetValues(typeof(mode)).GetValue(0));
        } else {
            set_mode( (mode) Enum.GetValues(typeof(mode)).GetValue((int)camera_mode+1));
        }
    }

    public void set_prev_mode()
    {
         if ((int)camera_mode == 0){
                set_mode((mode)Enum.GetValues(typeof(mode)).GetValue(Enum.GetValues(typeof(mode)).Length-1));
        } else {
         set_mode((mode)Enum.GetValues(typeof(mode)).GetValue((int)camera_mode-1));
        }
    }

    private void set_camera(int i)
    {
        if (i < cam_list.Length)
        {
            if ( camera_mode == mode.FREE_CAM )
            {
                temp_main_camera_transform = this.transform;
                //temp_main_camera_pos = this.transform.position;
                //temp_main_camera_rot = this.transform.rotation;
                temp_main_camera_mode = camera_mode;
            }

            camera_index = i;
            camera_name = cam_list[camera_index].name;
            if (this.name == camera_name)
            {
                cameraTarget = temp_main_camera_transform;
                movingToCamera = true;
                //this.transform.position = temp_main_camera_pos;
                //this.transform.rotation = temp_main_camera_rot;   
                set_mode(temp_main_camera_mode);
            }
            else
            {
                cameraTarget = cam_list[camera_index].transform;
                movingToCamera = true;
                set_mode(mode.LOCKED);
            }
            ui_listener.NotifyAll(camera_name, camera_mode.ToString());
        }
    }


    // Update is called once per frame
    void Update()
    {
        if ((movingToCamera || camera_mode==mode.LOCKED )&& cameraTarget){
        float step = slerpSpeed * Time.deltaTime;

        if (!movingToCamera){
        transform.position = Vector3.MoveTowards(transform.position, cameraTarget.position, 1000*step);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, cameraTarget.rotation, 1000*step);
        } else {
        transform.position = Vector3.MoveTowards(transform.position, cameraTarget.position, step);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, cameraTarget.rotation, step);
        }
        

        if (Vector3.Distance(transform.position, cameraTarget.position) < 0.1f && 
        Quaternion.Angle(transform.rotation, cameraTarget.rotation) < 0.1f){
            movingToCamera = false;
           // cameraTarget = null;
        }
     
        }

        // Mode
        if (Input.GetKeyUp(KeyCode.C))
        {
            set_next_camera();
        }
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            set_camera(0);
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            set_camera(1);
        }
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            set_camera(2);
        }
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            set_camera(3);
        }
        if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            set_camera(4);
        }
        if (Input.GetKeyUp(KeyCode.Alpha6))
        {
            set_camera(5);
        }
        if (Input.GetKeyUp(KeyCode.Alpha7))
        {
            set_camera(6);
        }
        if (Input.GetKeyUp(KeyCode.Alpha8))
        {
            set_camera(7);
        }
        if (Input.GetKeyUp(KeyCode.Alpha9))
        {
            set_camera(8);
        }
        if (Input.GetKeyUp(KeyCode.Alpha0))
        {
            set_camera(9);
        }

        if (Input.GetKeyUp(KeyCode.V))
        {
            if (camera_mode != mode.FREE_CAM)
            {
               
                 set_mode(mode.FREE_CAM);
            }
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            if (camera_mode != mode.LOCKED)
            {
                set_mode(mode.LOCKED);
            }
        }

        
        if (Input.GetKeyUp(KeyCode.H))
        {
            if (camera_mode != mode.LOCKED)
            {
                set_mode(mode.LOCKED);
            }
        }


        // Movement
        if (camera_mode == mode.FREE_CAM)
        {
            handle_free_cam();
        }
        


    }

  


    public void zoomout(){
        
        this.transform.Translate(-Vector3.forward * fastZoomSensitivity * Time.deltaTime);
    }

    public void zoomin(){
        this.transform.Translate(Vector3.forward * fastZoomSensitivity * Time.deltaTime);
    }

    void OnDisable()
    {
        StopLooking();
    }
    public void StopLooking()
    {
        looking = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartLooking()
    {
        looking = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}

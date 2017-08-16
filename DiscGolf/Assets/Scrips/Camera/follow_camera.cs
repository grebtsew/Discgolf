using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class follow_camera : MonoBehaviour
{
    private Vector3 offset;
    private Vector3 stand_offset;
    private Disc_Movement frisbee;
    public camera_mode mode = camera_mode.throw_mode;
    public Text mode_label;
    private List<Transform> targets = new List<Transform>();
    public Transform target;

    private float climbSpeed = 4;
    private float normalMoveSpeed = 10;
    private float slowMoveFactor = 0.25f;
    private float fastMoveFactor = 3;

    void Start()
    {
        frisbee = FindObjectOfType<Disc_Movement>();
        stand_offset = transform.position - frisbee.transform.position;

        target = frisbee.GetComponent<Transform>();
        targets.Add(target);


        foreach (basket_script bs in FindObjectsOfType<basket_script>())
        {
            targets.Add(bs.GetComponent<Transform>());
        }
    }

    /// Change Camera
    /// 
    /// 
    public void toggle_follow()
    {
        switch (mode)
        {
            case camera_mode.throw_mode:
                transform.position = frisbee.transform.position + stand_offset;
                if (frisbee.isThrown)
                {
                    mode = camera_mode.follow_frisbee;
                }
                break;
            case camera_mode.free_camera:
                mode = camera_mode.throw_mode;
                break;
            case camera_mode.follow_frisbee:
                mode = camera_mode.stuck_camera;
                break;
            case camera_mode.stuck_camera:
                mode = camera_mode.free_camera;
                break;
        }
    }
    public void updatePosition()
    {
        transform.position = frisbee.transform.position + stand_offset;
    }

    void Update()
    {
        switch (mode)
        {
            case camera_mode.throw_mode:
                if (frisbee.isThrown)
                {
                    offset = transform.position - frisbee.transform.position;
                    mode = camera_mode.follow_frisbee;
                }
                else
                {

                    throw_mode();
                    transform.LookAt(frisbee.transform);
                }
                break;
            case camera_mode.stuck_camera:
                transform.position = frisbee.discInitialPosition + stand_offset;
                transform.LookAt(frisbee.transform);
                break;
            case camera_mode.free_camera:
                freecamera();
                transform.LookAt(target);
                break;
            case camera_mode.follow_frisbee:
                transform.position = frisbee.transform.position + offset;
                transform.LookAt(frisbee.transform);
                break;

        }

        mode_label.text = mode.ToString();


        buttonaction();
    }

    /// Button listeners
    /// 
    ///
    private void buttonaction()
    {
        // change target
        if (Input.GetKeyDown("f") && mode == camera_mode.free_camera)
        {
            int index = targets.IndexOf(target) + 1;
            if (index < targets.Count)
            {
                target = targets[index];
            }
            else
            {
                target = targets[0];
            }
            transform.position = target.transform.position + stand_offset * 2;

        }

        // change camera
        if (Input.GetKeyDown("c"))
        {
            toggle_follow();
        }

        if (Input.GetKeyDown("r"))
        {
            offset = stand_offset;
        }
    }

    /// Rotate after disc
    /// 
    /// 
    private void throw_mode()
    {
        float playerAngle = AngleOnXZPlane(frisbee.transform);
        float cameraAngle = AngleOnXZPlane(transform);

        // difference in orientations
        float rotationDiff = Mathf.DeltaAngle(cameraAngle, playerAngle);

        // rotate around target by time-sensitive difference between these angles
        transform.RotateAround(frisbee.transform.position, Vector3.up, rotationDiff * Time.deltaTime);
    }

    /// Free Camera 
    /// 
    ///
    private void freecamera()
    {

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        else
        {
            transform.position += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
        }


        if (Input.GetKey(KeyCode.Q)) { transform.position += transform.up * climbSpeed * Time.deltaTime; }
        if (Input.GetKey(KeyCode.E)) { transform.position -= transform.up * climbSpeed * Time.deltaTime; }

        if (Input.GetKeyDown(KeyCode.End))
        {
            Screen.lockCursor = (Screen.lockCursor == false) ? true : false;
        }
    }
    private float AngleOnXZPlane(Transform item)
    {

        // get rotation as vector (relative to parent)
        Vector3 direction = item.rotation * item.parent.forward;

        // return angle in degrees when projected onto xz plane
        return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    }
}

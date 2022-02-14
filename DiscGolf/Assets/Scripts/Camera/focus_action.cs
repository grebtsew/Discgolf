using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class focus_action : MonoBehaviour
{
    private Camera camera;
    private camera_control control;
    private bool focused = true;
    private Vector3 target_focus_pos;

    // Start is called before the first frame update
    void Start()
    {
        camera = gameObject.GetComponent<Camera>();
        control = gameObject.GetComponent<camera_control>();

    }

    private float getMaxElement(Vector3 v3){
        return Mathf.Max(Mathf.Max(v3.x, v3.y), v3.z);
    }


    public void focus() {
        float objectSize = 0.2f;//getMaxElement(control.target.GetComponent<Renderer>().bounds.size); // TODO calculate actual size here!
        var fov = camera.fieldOfView * ( Mathf.PI / 180 ); 
        var distance = Mathf.Abs( objectSize / Mathf.Sin( fov / 2 ) );

        //gameObject.transform.position = control.target.transform.position - Vector3.Normalize(control.target.transform.position)*distance;
       // gameObject.transform.LookAt(control.target.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
}

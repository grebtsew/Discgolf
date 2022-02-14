using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// TODO make 3d
// TODO make transparent!
// TODO: show fov for a camera during camera mode, leave a toggle!
// do this with arrows!

public class show_fov : MonoBehaviour
{
    public int AmountOfRaycasts = 1024;
    public bool DrawDebugLines = false;
    public float Range = 25;
    public float FieldOfView = 90;
    public float DegreeOffset = 270;
    private MeshFilter LOSMeshFilter;
 
    private List<GameObject> objectsInLOS = new List<GameObject>();
    private Material lOSMaterial;
 
    public List<GameObject> ObjectsInLOS {
        get {            
            return objectsInLOS;
        }
    }
 
    public Material LOSMaterial {
        get {
            return lOSMaterial;
        }
 
        set {
            lOSMaterial = value;
        }
    }
 
    void Start() {        
        LOSMeshFilter = GetComponent<MeshFilter>();
        LOSMaterial = GetComponent<MeshRenderer>().material;        
    }
 
    void Update() {
        Vector3[] points = new Vector3[AmountOfRaycasts+2];
        transform.rotation = Quaternion.identity;
        objectsInLOS.Clear();
        for (int i = 1; i <= AmountOfRaycasts + 1; i++) {
 
            RaycastHit hitInfo;
 
            float degrees = (((float)(i-1) / AmountOfRaycasts) * FieldOfView);
            degrees -= FieldOfView / 2;
            degrees -= transform.parent.rotation.eulerAngles.y;
            degrees += DegreeOffset;
 
            if (Physics.Raycast(new Ray(transform.position, RadiansToVector3(degrees*Mathf.Deg2Rad)), out hitInfo, Range)) {
                points[i] = hitInfo.point - transform.position;
                if(!objectsInLOS.Contains(hitInfo.collider.gameObject)) {
                    objectsInLOS.Add(hitInfo.collider.gameObject);
                }
            } else {                
                points[i] = RadiansToVector3(degrees*Mathf.Deg2Rad).normalized * Range;
            }
        }        
        if (DrawDebugLines) {
            for (int i = 0; i < points.Length; i++) {
                Debug.DrawLine(transform.position, transform.position + points[i], Color.red, 0, false);
            }
        }
        LOSMeshFilter.mesh = CreateMeshFromPoints(points);     
    }
 
    Vector3 RadiansToVector3(float degrees) {
        return new Vector3(Mathf.Cos(degrees), 0, Mathf.Sin(degrees));
    }
 
    Mesh CreateMeshFromPoints(Vector3[] points) {
        Mesh m = new Mesh();
        m.name = "LOSMesh";
 
        points[0] = Vector3.zero;
        m.vertices = points;
 
        int[] trianglesArray = new int[(m.vertices.Length-1) * 3];
 
        int count = 1;
        for (int i = 0; i < trianglesArray.Length-3; i+=3) {
            trianglesArray[i] = count;
            trianglesArray[i + 1] = 0;
            trianglesArray[i + 2] = count + 1;
            count++;
        }
        //trianglesArray[trianglesArray.Length-3] = m.vertices.Length-1;
        //trianglesArray[trianglesArray.Length-2] = 0;
        //trianglesArray[trianglesArray.Length-1] = 1;
 
        m.triangles = trianglesArray;       
 
        m.RecalculateNormals();
 
        return m;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{

    //private List<GameObject> Lines = new List<GameObject>(); // trajectory
    //private bool show_lines = false;
     /// Tracker Line methods
   /*
    void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = color;
        lr.startWidth = 0.1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        Lines.Add(myLine);

    }
    void ClearLines()
    {
        foreach (GameObject go in Lines)
        {
            GameObject.Destroy(go);
        }
    }
*/
/*
public void SetShowLine()
    {
        ClearLines();
        if (show_lines)
        {
            show_lines = false;
        }
        else
        {
            show_lines = true;
        }
    }
    */
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

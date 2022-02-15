using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscChange : MonoBehaviour
{
    public GameObject[] discs;
    public discInfoNotifier din;
    private Simulator dm;

    // Start is called before the first frame update
    void Start()
    {
        dm = GetComponent<Simulator>();
    }

    public void changeDisc(){

        // Destroy child
        GameObject  ChildGameObject1 = this.transform.GetChild(0).gameObject;
        DestroyImmediate(ChildGameObject1);

        // instantiate new
        GameObject go = Instantiate(discs[din.index]);
        go.transform.parent = this.transform;
        go.transform.localPosition = discs[din.index].transform.localPosition;
        go.transform.localRotation = discs[din.index].transform.localRotation;
        dm.UpdateDiscComponent();
      
    }
}

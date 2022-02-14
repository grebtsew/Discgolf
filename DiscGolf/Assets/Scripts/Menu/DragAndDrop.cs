using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;

public class DragAndDrop : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler

{

    [SerializeField] private RectTransform dragRectTransform;
    [SerializeField] private Canvas canvas;

    private RectTransform org_pos;
    void Start(){
        org_pos = dragRectTransform;
    }
 
    /// <summary>
    /// This method will be called during the mouse drag
    /// </summary>
    /// <param name="eventData">mouse pointer event data</param>
    public void OnDrag(PointerEventData eventData)
    {
        dragRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
 

    public void OnBeginDrag(PointerEventData eventData){

    }

    public void OnEndDrag(PointerEventData eventData){
        // move back if out of screen
            #if UNITY_EDITOR
        if (Input.mousePosition.x < 0 || Input.mousePosition.y < 0 || Input.mousePosition.x >= Handles.GetMainGameViewSize().x - 1 || Input.mousePosition.y >= Handles.GetMainGameViewSize().y - 1) 
    {
    dragRectTransform.anchoredPosition = org_pos.anchoredPosition;
    }
    #else
        if (Input.mousePosition.x < 0 || Input.mousePosition.y < 0 || Input.mousePosition.x >= Screen.width - 1 || Input.mousePosition.y >= Screen.height - 1) {
            dragRectTransform.anchoredPosition = org_pos.anchoredPosition;
        }
    #endif

    }

    public void OnPointerDown(PointerEventData eventData){
        dragRectTransform.SetAsLastSibling();
    }
 
  
}


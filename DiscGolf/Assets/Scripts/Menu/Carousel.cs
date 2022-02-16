using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class Carousel : MonoBehaviour
{

    public RectTransform[] images;
    public RectTransform view_window;
    public DiscChange discchange;

    private bool  canSwipe;
    private float image_width;
    private float lerpTimer;
    private float lerpPosition;
    private float mousePositionStartX;
    private float mousePositionEndX;
    private float dragAmount;
    private float screenPosition;
    private float lastScreenPosition;
    public discInfoNotifier discInfoNotifier;

    /// <summary>
    /// Space between images.
    /// </summary>
    public  float image_gap             = 30;

    public int swipeThrustHold          = 30;
    
    private int m_currentIndex;

    /// <summary>
    /// The index of the current image on display.
    /// </summary>
    public int CurrentIndex { get { return m_currentIndex; } }

    // Just for display purpose
    //public Text currentIndexLable;

    #region mono
    // Use this for initialization
    void Start () {
      
        image_width = view_window.rect.width;
        for (int i = 1; i < images.Length; i++)
        {
            images[i].anchoredPosition = new Vector2(((image_width + image_gap) * i), 0);
        }
        
    }

    // Update is called once per frame
    void Update () {

        //UpdateCurrentIndexLable();
        UpdateCarouselView();    

        if (Input.GetKey(KeyCode.K) )
        {
            GoToPrev();
        }
        if (Input.GetKey(KeyCode.L) )
        {
            GoToNext();
        }
         
    }
    #endregion


    /*
    void UpdateCurrentIndexLable()
    {
        if(currentIndexLable)
            currentIndexLable.text = m_currentIndex.ToString();
    }
    */

    
    #region private methods
    void UpdateCarouselView()
    {
        lerpTimer = lerpTimer + Time.deltaTime;

        if (lerpTimer < 0.333f)
        {
            screenPosition = Mathf.Lerp(lastScreenPosition, lerpPosition * -1, lerpTimer * 3);
            lastScreenPosition = screenPosition;
        }

        if (Input.GetMouseButtonDown(0))
        {
            canSwipe = true;
            mousePositionStartX = Input.mousePosition.x;
        }


        if (Input.GetMouseButton(0))
        {
            if (canSwipe)
            {
                mousePositionEndX = Input.mousePosition.x;
                dragAmount = mousePositionEndX - mousePositionStartX;
                screenPosition = lastScreenPosition + dragAmount;
            }
        }

        if (Mathf.Abs(dragAmount) > swipeThrustHold && canSwipe)
        {
            canSwipe = false;
            lastScreenPosition = screenPosition;
           // if (m_currentIndex < images.Length)
              //  OnSwipeComplete();
           // else if (m_currentIndex == images.Length && dragAmount < 0)
                lerpTimer = 0;
           // else if (m_currentIndex == images.Length && dragAmount > 0)
               // OnSwipeComplete();
        }

        for (int i = 0; i < images.Length; i++)
        {
            images[i].anchoredPosition = new Vector2(screenPosition + ((image_width + image_gap) * i), 0);
            if (i == m_currentIndex)
            {
                images[i].localScale = Vector3.Lerp(images[i].localScale, new Vector3(1.2f, 1.2f, 1.2f), Time.deltaTime * 5);
            }
            else
            {
                images[i].localScale = Vector3.Lerp(images[i].localScale, new Vector3(0.7f, 0.7f, 0.7f), Time.deltaTime * 5);
            }
        }
    }
    /*
    void OnSwipeComplete()
    {
        lastScreenPosition = screenPosition;

        if (dragAmount > 0)
        {
            if (dragAmount >= swipeThrustHold)
            {
                if (m_currentIndex == 0)
                {
                    lerpTimer = 0; lerpPosition = 0;
                }
                else
                {
                    m_currentIndex--;
                    lerpTimer = 0;
                    if (m_currentIndex < 0)
                        m_currentIndex = 0;
                    lerpPosition = (image_width + image_gap) * m_currentIndex;
                }
            }
            else
            {
                lerpTimer = 0;
            }
        }
        else if (dragAmount < 0)
        {
            if (Mathf.Abs(dragAmount) >= swipeThrustHold)
            {
                if (m_currentIndex == images.Length-1)
                {
                    lerpTimer = 0;
                    lerpPosition = (image_width + image_gap) * m_currentIndex;
                }
                else
                {
                    lerpTimer = 0;
                    m_currentIndex++;
                    lerpPosition = (image_width + image_gap) * m_currentIndex;
                }
            }
            else
            {
                lerpTimer = 0;
            }
        }
        dragAmount = 0;
    }
    */
    #endregion



    #region public methods
    public void GoToNext(){
        if (images.Length-1 == m_currentIndex){
            GoToIndexSmooth(m_currentIndex);
        } else {
             GoToIndexSmooth(m_currentIndex+1);
        }
        
    }

    public void  GoToPrev(){
       if (m_currentIndex == 0){
            GoToIndexSmooth(m_currentIndex);
        } else {
             GoToIndexSmooth(m_currentIndex-1);
        }
    }

    public void GoToIndex(int value)
    {
        m_currentIndex = value;
        lerpTimer = 0;
        lerpPosition = (image_width + image_gap) * m_currentIndex;
        screenPosition = lerpPosition * -1;
        lastScreenPosition = screenPosition;
        for (int i = 0; i < images.Length; i++)
        {
            images[i].anchoredPosition = new Vector2(screenPosition + ((image_width + image_gap) * i), 0);
        }
        discInfoNotifier.notifyAll();
        discchange.changeDisc();
    }

    public void GoToIndexSmooth(int value)
    {
        m_currentIndex = value;
        lerpTimer = 0;
        lerpPosition = (image_width + image_gap) * m_currentIndex;
        discInfoNotifier.notifyAll();
        discchange.changeDisc();
    }
    #endregion
}


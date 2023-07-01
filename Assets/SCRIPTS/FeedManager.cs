using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedManager : MonoBehaviour
{
    public int CurrentPage = 0;
    public int targetpage = 0;

    [Header("UI References")]
    public RectTransform FeedScroller;

    [Header("Snap Settings")]
    [Range(0,1)]
    public float SnapDistance = 0.5f;
    public float SnapSpeed = 0.25f;
    public float DragSpeed = 1.25f;
    public float SwipeThreshold = 0.25f;


    float PageLength = 1920f;
    public float deltaY;

    public bool dragstarted = false;
    public bool snapped = true;
    Vector3 startpos;
    Vector3 endpos;

    void Update()
    {
        //Start Interaction
        if (Input.GetMouseButtonDown(0) && snapped && !dragstarted)
        {
            dragstarted = true;
            startpos = Input.mousePosition;
        }

        //Handle Dragging during Interaction
        if (Input.GetMouseButton(0) && snapped && dragstarted)
        {
            FeedScroller.transform.localPosition = new Vector3(0, (CurrentPage * 1920f) + (Input.mousePosition.y - startpos.y) * DragSpeed, 0);
        }

        //Trigger feed snap after Interaction
        if (Input.GetMouseButtonUp(0) && dragstarted)
        {
            endpos = Input.mousePosition;
            targetpage = CurrentPage;
            // Caluculate target page base on page position or swipe gesture
            if (Mathf.Abs(endpos.y - startpos.y) > SwipeThreshold * PageLength)
            {
                targetpage = endpos.y > startpos.y ? CurrentPage + 1 : CurrentPage - 1;
                Debug.Log("Swipe Registered. New Target: " + targetpage.ToString());
                //Handle out of bounds pages
                if (targetpage < 0)
                {
                    targetpage = 0;
                }
                else if (targetpage > FeedScroller.childCount - 1)
                {
                    targetpage = FeedScroller.childCount - 1;
                }
            }

            snapped = false;
        }

        deltaY = targetpage * PageLength - FeedScroller.transform.localPosition.y;

        //Adjust page based on target
        if (targetpage != CurrentPage)
        {
            int direction = targetpage > CurrentPage ? 1 : -1;
            FeedScroller.transform.localPosition += new Vector3(0, SnapSpeed * direction, 0);
            if(Mathf.Abs(deltaY) < PageLength * SnapDistance || (FeedScroller.transform.localPosition.y > targetpage * PageLength && direction > 0) || (FeedScroller.transform.localPosition.y < targetpage * PageLength && direction < 0))
            {
                CurrentPage = targetpage;
            }
        }
        else if (!snapped)
        {
            CurrentPage = targetpage;
            FeedScroller.transform.localPosition = new Vector3(0, CurrentPage * PageLength, 0);
            snapped = true;
            dragstarted = false;
        }

        //Handle Out of Bounds Events
        if (FeedScroller.transform.localPosition.y < 0)
        {
            CurrentPage = 0;
            targetpage = 0;
            FeedScroller.transform.localPosition = new Vector3(0, 0, 0);
            dragstarted = false;
            snapped = true;
        }
        else if (FeedScroller.transform.localPosition.y > (FeedScroller.childCount - 1) *  PageLength)
        {
            CurrentPage = (FeedScroller.childCount - 1);
            targetpage = (FeedScroller.childCount - 1);
            FeedScroller.transform.localPosition = new Vector3(0, FeedScroller.sizeDelta.y - PageLength, 0);
            dragstarted = false;
            snapped = true;
        }
    }
}

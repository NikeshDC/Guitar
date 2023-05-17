using UnityEngine;
using System.Collections;

public class StrummingInputHandler : GuitarStringsInputHandler
{ 
    private Vector2 currentTouchPos;  //current position of finger touch in screen coordinate
    private Vector2 lastTouchPos;   //the last recorded position of finger touch in screen coordinate
    private float velocity;   //velocity of finger swipe (in vertical direction i.e. y-axis) used for setting volume of string while strumming
                              //rougly equates to how much pixel per second travelled 

    private float updateInterval = 10f / 1000;  //checks for swipe movement every 10ms
    private float volumeFactor = 1.0f / 1000;  //normalize velocity to between 0 and 1 to set volume of string while strumming                                            

    public override void OnBegin(Touch touch)
    {
        currentTouchPos = touch.position;
        lastTouchPos = touch.position;

        StartCoroutine(HandleSwipes());
    }

    public override void OnStationary(Touch touch)
    {
        //do nothing
    }

    public override void OnMove(Touch touch)
    {
        currentTouchPos = touch.position;
    }

    public override void OnEnd(Touch touch)
    {
        StopCoroutine("HandleSwipes");
    }

    public override void OnCancel(Touch touch)
    {
        //do nothing
    }


    private IEnumerator HandleSwipes()
    {//actually responsible for handling swiping motion and then playing sound of correponding string
        while(true)
        {
            if (currentTouchPos.y != lastTouchPos.y)
            {//if there is no vertical movement then no need to perform any operation
                velocity = (currentTouchPos.y - lastTouchPos.y) / updateInterval;
                float stringVolume = Mathf.Abs(velocity * volumeFactor);
                //Debug.Log("Volume: " + stringVolume);

                foreach (MusicStringBehaviour guitarString in this.guitarStrings)
                {
                    //touch points are in screen coordinate so for comparison they need to be in same coordinate space
                    Vector3 guitarStringScreenspacePos = Camera.main.WorldToScreenPoint(guitarString.transform.position);

                    if ((guitarStringScreenspacePos.y <= currentTouchPos.y && guitarStringScreenspacePos.y >= lastTouchPos.y) ||
                        (guitarStringScreenspacePos.y >= currentTouchPos.y && guitarStringScreenspacePos.y <= lastTouchPos.y))
                    {//the swipe line crosses the guitar string
                        guitarString.SetIntensity(stringVolume);
                        guitarString.PlayString();
                    }
                }
            }
            lastTouchPos = currentTouchPos;
            yield return new WaitForSeconds(updateInterval);
        }
    }
}

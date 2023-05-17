using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{//handles touch inputs
    public List<MusicStringBehaviour> guitarStrings;

    public Vector3 strumAndFretAreaSeperator; //the area to right of this x-position is for fret board and left is for strumming the string

    private Dictionary<int, IFingerTouchHandler> fingerTouches;  //fingertouch with its corresponding id as key

   
    void Start()
    {
        fingerTouches = new Dictionary<int, IFingerTouchHandler>();
    }


    void Update()
    {
        if(Input.touchCount > 0)
        {
            foreach(Touch touch in Input.touches)
            {
                if(touch.phase == TouchPhase.Began)
                {//new finger touch has been started
                    RegisterNewTouch(touch);
                }

                if(fingerTouches.ContainsKey(touch.fingerId))
                {
                    CallFingerTouchCallback(fingerTouches[touch.fingerId], touch);
                }
                else
                {
                    Debug.LogError("FingerId " + touch.fingerId + " not stored in begining.");
                }

                if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {//finger touch has ended destroy correponding fingerTouch handlers
                    CleanUpOldTouch(touch);
                }
            }
        }
    }

    private void RegisterNewTouch(Touch touch)
    {
        Vector3 strumAreaSeperatorToScreenSpace = Camera.main.WorldToScreenPoint(strumAndFretAreaSeperator);
        if (touch.position.x <= strumAreaSeperatorToScreenSpace.x)
        {//if the touch begins within the strumming area classify it as strum input
            StrummingInputHandler strumHandle = gameObject.AddComponent<StrummingInputHandler>() as StrummingInputHandler;
            strumHandle.guitarStrings = this.guitarStrings;
            fingerTouches.Add(touch.fingerId, strumHandle);
        }
        else
        {
            FretInputHandler fretHandle = gameObject.AddComponent<FretInputHandler>() as FretInputHandler;
            fretHandle.guitarStrings = this.guitarStrings;
            fingerTouches.Add(touch.fingerId, fretHandle);
        }
    }

    private void CallFingerTouchCallback(IFingerTouchHandler fingerTouch, Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                fingerTouch.OnBegin(touch);
                break;

            case TouchPhase.Stationary:
                fingerTouch.OnStationary(touch);
                break;

            case TouchPhase.Moved:
                fingerTouch.OnMove(touch);
                break;

            case TouchPhase.Ended:
                fingerTouch.OnEnd(touch);
                break;

            case TouchPhase.Canceled:
                fingerTouch.OnCancel(touch);
                break;
        }
    }

    private void CleanUpOldTouch(Touch touch)
    {
        IFingerTouchHandler fTouch = fingerTouches[touch.fingerId];
        fingerTouches.Remove(touch.fingerId);
        if (fTouch.GetType() == typeof(StrummingInputHandler))
        {
            StrummingInputHandler strumHandle = fTouch as StrummingInputHandler;
            Destroy(strumHandle);
        }
        else if (fTouch.GetType() == typeof(FretInputHandler))
        {
            FretInputHandler fretHandle = fTouch as FretInputHandler;
            Destroy(fretHandle);
        }
    }

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{//handles touch inputs

    public GameManager gameManager;

    public Transform strumAndFretAreaSeperator; //the area to right of this transform's x-position is for fret board and left is for strumming the string

    [Range(0f, 0.1f)]
    public float stringSelectionMargin;  //margin from center position of strings which is considered to be touch area for corresponding strings

    Dictionary<int, IFingerTouchHandler> fingerTouches;  //fingertouch with its corresponding id as key

    // Start is called before the first frame update
    void Start()
    {
        fingerTouches = new Dictionary<int, IFingerTouchHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0)
        {
            foreach(Touch touch in Input.touches)
            {
                if(touch.phase == TouchPhase.Began)
                {//new finger touch has been started
                    Vector3 strumAndFretAreaSeperatorToScreenSpace = Camera.main.WorldToScreenPoint(strumAndFretAreaSeperator.position);
                    if (touch.position.x <= strumAndFretAreaSeperatorToScreenSpace.x)
                    {//if the touch begins within the strumming area classify it as strum input
                        StrummingInputHandler strumHandle = gameObject.AddComponent<StrummingInputHandler>() as StrummingInputHandler;
                        strumHandle.gameManager = gameManager;
                        fingerTouches.Add(touch.fingerId, strumHandle);
                    }
                    else
                    {
                        FretInputHandler fretHandle = gameObject.AddComponent<FretInputHandler>() as FretInputHandler;
                        fretHandle.gameManager = gameManager;
                        fingerTouches.Add(touch.fingerId, fretHandle);
                    }
                }

                if(fingerTouches.ContainsKey(touch.fingerId))
                {
                    callFingerTouchCallback(fingerTouches[touch.fingerId], touch);
                }
                else
                {
                    Debug.LogWarning("FingerId " + touch.fingerId + " not stored in begining.");
                }

                if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {//finger touch has ended destroy correponding fingerTouch handlers
                    IFingerTouchHandler fTouch = fingerTouches[touch.fingerId];
                    fingerTouches.Remove(touch.fingerId);
                    if (fTouch.GetType() == typeof(StrummingInputHandler))
                    {
                        StrummingInputHandler strumHandle = fTouch as StrummingInputHandler;
                        Destroy(strumHandle);
                    }
                    else if(fTouch.GetType() == typeof(FretInputHandler))
                    {
                        FretInputHandler fretHandle = fTouch as FretInputHandler;
                        Destroy(fretHandle);
                    }
                }
            }
        }
    }
    
    private void callFingerTouchCallback(IFingerTouchHandler fingerTouch, Touch touch)
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

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FretInputHandler : GuitarStringsInputHandler
{
    static List<FretInputHandler> activeObjects = new List<FretInputHandler>();

    int selectedString = -1;  //index to list of strings provided by gamemanager

    public float stringSelectionMargin = 0.3f;  //margin from center position of strings which is considered to be touch area for corresponding strings

    public float bendThreshold = 0.35f; //after this threshold ony is the string assumed to move

    Vector2 lastTouchPos;
    Vector3 baseTouchPos;

    public override void OnBegin(Touch touch)
    {
        FretInputHandler.activeObjects.Add(this);
        baseTouchPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));
        lastTouchPos = touch.position;
    }

    public override void OnStationary(Touch touch)
    {
        if(selectedString == -1)
        {//no string has been selected
            Vector3 touchToWorld = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));
            selectedString = GetSelectedStringFromTouchPos(touchToWorld.y);
        }
        else
        {
            //selectedStringBasePos = gameManager.guitarStrings[selectedString].transform.position;
            //select fret for the string taking into account other possible touches on the same string
            if (IsBaseTouch())
                SetGuitarStringToTouchPos(touch);
        }
    }

    public override void OnMove(Touch touch)
    {
        if(selectedString != -1)
        {//some string has been selected
         //use y-axis movement to perform bending of the string
         if(IsBaseTouch())
            SetGuitarStringToTouchPos(touch);

         //use x-axis movement for slide
        }
        lastTouchPos = touch.position;
    }

    public override void OnEnd(Touch touch)
    {
        OnTouchRemoved();
    }

    public override void OnCancel(Touch touch)
    {
        OnTouchRemoved();
    }

    private void OnTouchRemoved()
    {//called in either of OnEnd or OnCancel
        if (selectedString != -1)
        {//some string had been selected
            GuitarStringBehaviour gString = this.guitarStrings[selectedString];
            if (!OtherTouchInSameString())
            {
                gString.ResetHold();
            }
        }
        FretInputHandler.activeObjects.Remove(this);
    }

    private Vector3 TouchToWorldPoint(Vector2 touchPositionInScreen)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(touchPositionInScreen.x, touchPositionInScreen.y, 0));
    }

    private void SetGuitarStringToTouchPos(Touch touch)
    {
        Vector3 touchToWorld = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));
        GuitarStringBehaviour gString = this.guitarStrings[selectedString];

        float bendMoveDistance = touchToWorld.y - baseTouchPos.y;
        float bendMoveDistanceAbs = Mathf.Abs(bendMoveDistance);
        bendMoveDistanceAbs = Mathf.Clamp(bendMoveDistanceAbs - bendThreshold, 0f, 1.0f); //only if movement is greater than threshold consider bending
        //bending guitar string based on y position of touch
        if (bendMoveDistanceAbs > 0)
        {
            float stringMoveDist = (Mathf.Sign(bendMoveDistance)) * bendMoveDistanceAbs;
            gString.BendAt(touchToWorld.x, stringMoveDist);
        }
        else if(bendMoveDistanceAbs == 0)
        {
            gString.HoldAt(touchToWorld.x);
        }
    }

    int GetSelectedStringFromTouchPos(float ypos)
    {//based on ypos of the touch(in world coordinates) finds the selected string if any
        int i = -1;
        foreach(GuitarStringBehaviour guitarString in this.guitarStrings)
        {
            i++;
            float guitarStringPos = guitarString.transform.position.y;
            if (ypos <= (guitarStringPos + stringSelectionMargin) && ypos >= (guitarStringPos - stringSelectionMargin))
                return i; 
        }

        return -1;
    }

    bool IsBaseTouch()
    {//returns if this finger touch is the one that should be given priority for fret selction based of its proximity to strumming section
        foreach(FretInputHandler fretInputHandler in activeObjects)
        {
            if(fretInputHandler.selectedString == this.selectedString)
            {
                if(fretInputHandler.lastTouchPos.x < this.lastTouchPos.x)
                {//this object is farther from strumming region so is not a base touch for its string
                    return false;
                }
            }
        }
        return true;
    }

    FretInputHandler FindBaseTouch(int stringId)
    {//returns base touch for the string if it exists
        FretInputHandler baseTouch = null;
        foreach (FretInputHandler fretInputHandler in activeObjects)
        {
            if (fretInputHandler.selectedString == stringId)
            {
                if(baseTouch == null)
                {
                    baseTouch = fretInputHandler;
                }
                if (fretInputHandler.lastTouchPos.x <= baseTouch.lastTouchPos.x)
                {//this object is farther towards strumming region so is more likely to be base touch for its string
                    baseTouch = fretInputHandler;
                }
            }
        }
        return baseTouch;
    }

    bool OtherTouchInSameString()
    {//returns if there are other touches in the same string besides this touch
        foreach (FretInputHandler fretInputHandler in activeObjects)
        {
            if ((this != fretInputHandler) && (fretInputHandler.selectedString == this.selectedString))
            {
                return true;
            }
        }
        return false;
    }
}

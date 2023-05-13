using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FretInputHandler : MonoBehaviour, IFingerTouchHandler
{
    static List<FretInputHandler> activeObjects = new List<FretInputHandler>();

    public GameManager gameManager;  //to get strings 
    int selectedString = -1;  //index to list of strings provided by gamemanager
    //Vector3 selectedStringBasePos;  //keep track of original position to restore it after having moved for bending

    public float stringSelectionMargin = 0.3f;  //margin from center position of strings which is considered to be touch area for corresponding strings

    public float maxStringMoveDistance = 1.0f; //the distance string is allowed to bend from initial position
    public float bendThreshold = 0.5f;


    Vector2 lastTouchPos;
    Vector3 baseTouchPos;

    public void OnBegin(Touch touch)
    {
        FretInputHandler.activeObjects.Add(this);
        baseTouchPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));
        lastTouchPos = touch.position;
    }

    public void OnStationary(Touch touch)
    {
        if(selectedString == -1)
        {//no string has been selected
            Vector3 touchToWorld = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));
            selectedString = getSelectedStringFromTouchPos(touchToWorld.y);
            //Debug.Log("SS: "+ selectedString);
        }
        else
        {
            //selectedStringBasePos = gameManager.guitarStrings[selectedString].transform.position;
            //select fret for the string taking into account other possible touches on the same string
            if (isBaseTouch())
                setGuitarStringToTouchPos(touch);
        }
    }

    public void OnMove(Touch touch)
    {
        if(selectedString != -1)
        {//some string has been selected
         //use y-axis movement to perform bending of the string
         if(isBaseTouch())
            setGuitarStringToTouchPos(touch);

         //use x-axis movement for slide
        }
        lastTouchPos = touch.position;
    }

    public void OnEnd(Touch touch)
    {
        OnTouchRemoved();
    }

    public void OnCancel(Touch touch)
    {
        OnTouchRemoved();
    }

    private void OnTouchRemoved()
    {//called in either of OnEnd or OnCancel
        if (selectedString != -1)
        {//some string has been selected
            GuitarString gString = gameManager.guitarStrings[selectedString].GetComponent<GuitarString>();
            if (!otherTouchInSameString())
            {
                gString.transform.position = gString.basePosition;
                gString.setTension(1.0f);
                gString.selectFret(0);
            }
        }
        FretInputHandler.activeObjects.Remove(this);
    }

    private void setGuitarStringToTouchPos(Touch touch)
    {
        GuitarString gString = gameManager.guitarStrings[selectedString].GetComponent<GuitarString>();
        Vector3 touchToWorld = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));

        float bendMoveDistance = touchToWorld.y - baseTouchPos.y;
        bendMoveDistance = Mathf.Clamp(bendMoveDistance - bendThreshold, 0f, maxStringMoveDistance); //only if movement is greater than threshold consider bending
        //bending guitar string based on y position of touch
        if ( Mathf.Abs(bendMoveDistance) < maxStringMoveDistance)
        {
            gString.transform.position = gString.basePosition + new Vector3(0f, bendMoveDistance, 0f);
            float tensionAmount = 1.0f + Mathf.Abs(bendMoveDistance) * gString.maxTensionFactor / maxStringMoveDistance;
            gString.setTension(tensionAmount);
        }

        //hammer pull off and slide based on x position of touch
        int fretSelected = gString.getFretFromFingerPosition(touchToWorld.x);
        gString.selectFret(fretSelected);
        //Debug.Log("Fret: "+ fretSelected);
    }

    int getSelectedStringFromTouchPos(float ypos)
    {//based on ypos of the touch(in world coordinates) finds the selected string if any
        int i = -1;
        foreach(GameObject guitarStringObj in gameManager.guitarStrings)
        {
            i++;
            float guitarStringPos = guitarStringObj.transform.position.y;
            if (ypos <= (guitarStringPos + stringSelectionMargin) && ypos >= (guitarStringPos - stringSelectionMargin))
                return i; 
        }

        return -1;
    }

    bool isBaseTouch()
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

    bool otherTouchInSameString()
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

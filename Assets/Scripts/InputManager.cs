using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //get Touches
        //classify the touch by which string is selected
        //classify whether the touch is strumming or fretSelection
        //for strumming find the strings to play (and volume)
        //for fretSelection if stationary set fret at given position (pulloff or hammer)
        //else if moved perform bend up or bend down or slide

        //classify the region of touchInput
        //if in fretSelection select a string 
        //if a string is selected and corresponding finger moved up or down then bend up or down

        //find if a touch is already existing finger or not and then send its correponding state to a callback function

        //when touch.begin create new class of Finger touch and register its subscription and call callback
        //when touch.end remove the finger touch and deregister its subscription
        //on other events find the proper finger with its id and then call callback fxn
        //if touch begins in strumming region make it play sound?? else instantiate different object for handling fret selection
    }


}

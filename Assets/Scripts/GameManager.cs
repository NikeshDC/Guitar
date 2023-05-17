using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{//maintains global variables required and sets important game components 

    public List<GameObject> guitarStringObjects; //GameObject containing GuitarStringBehaviour component, expected to be populated in inspector
    [HideInInspector]
    public List<MusicStringBehaviour> guitarStrings; //guitar strings that are actually responsible for playing the sounds

    public InputManager inputManager;

    public GameObject fretPrefab;
    public Transform fretStartPos;
    public Transform fretEndPos;
    public int numberOfFrets = 7;


    void Awake()
    {
        GenerateFretObjects();
        SetGuitarStrings();
        SetInputManager();
    }

    void GenerateFretObjects()
    {
        //construct fret board by placing frets
        float fretInterval = (fretEndPos.position.x - fretStartPos.position.x) / this.numberOfFrets;
        for (int i = 1; i <= this.numberOfFrets; i++)
        {
            float xpos = i * fretInterval + fretStartPos.position.x;
            Vector3 instantiatePos = new Vector3(xpos, 0, 0);
            Instantiate(fretPrefab, instantiatePos, Quaternion.identity);
        }
    }

    void SetGuitarStrings()
    {
        guitarStrings = new List<MusicStringBehaviour>();
        //set variables for each guitar strings
        foreach (GameObject guitarStringObj in guitarStringObjects)
        {
            MusicStringBehaviour gString = guitarStringObj.GetComponent<MusicStringBehaviour>();
            gString.stringStartPos = this.fretStartPos.position.x;
            gString.stringEndPos = this.fretEndPos.position.x;
            gString.numberOfFrets = this.numberOfFrets;
            guitarStrings.Add(gString);
        }
    }

    void SetInputManager()
    {
        inputManager.guitarStrings = this.guitarStrings;
        //Vector3 fretEndPosToScreenSpace = Camera.main.WorldToScreenPoint(fretEndPos.position);
        inputManager.strumAndFretAreaSeperator = fretEndPos.position;
    }

}

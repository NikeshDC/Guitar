using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{//maintains global variables and manages all game components 

    public List<GameObject> guitarStrings; //guitar strings that play the sounds

    public InputManager inputManager;

    public GameObject fretPrefab;

    public Transform fretStartPos;
    public Transform fretEndPos;
    public int MAX_FRETS = 12;

    // Start is called before the first frame update
    void Start()
    {
        //construct fret board by placing frets
        float fretInterval = (fretEndPos.position.x - fretStartPos.position.x) / MAX_FRETS;
        for(int i=1; i<= MAX_FRETS; i++)
        {
            float xpos = i * fretInterval + fretStartPos.position.x;
            Vector3 instantiatePos = new Vector3(xpos, 0, 0);
            Instantiate(fretPrefab, instantiatePos, Quaternion.identity);
        }

        //set variables for each guitar strings
        foreach(GameObject guitarStringObj in guitarStrings)
        {
            GuitarString gString = guitarStringObj.GetComponent<GuitarString>();
            gString.fretStartPos = this.fretStartPos;
            gString.fretEndPos = this.fretEndPos;
            gString.MAX_FRETS = MAX_FRETS;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

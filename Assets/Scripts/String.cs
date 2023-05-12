using UnityEngine;
using UnityEngine.Audio;

public class String : MonoBehaviour
{
    private int selectedFret = 0; //0 stands for open string
    static private int MAX_FRET = 12;

    private float fretPitchIncrementfactor = Mathf.Pow(2.0f, 1.0f/12);  //divde octave into 12 semitones by equal temperament
    private float pitch = 1.0f;

    AudioSource stringPlayer;
    public AudioClip baseNote;

    public Transform fretStartPos;
    public Transform fretEndPos;

    void Start()
    {
        stringPlayer = GetComponent<AudioSource>();
        stringPlayer.clip = baseNote;
    }

    void Update()
    {
    }


    public int getFretFromFingerPosition(float fingerPosition)
    {
        //assuming string extends along x axis
        float normalizedPosition = (fingerPosition - fretStartPos.position.x) / (fretEndPos.position.x - fretStartPos.position.x);
        return (int) Mathf.Ceil(normalizedPosition * MAX_FRET); //Fret 0 is open fret when no finger is pressed in fretboard
    }

    public void resetFret()
    {
        selectedFret = 0;
    }
    public void selectFret(int fret)
    {
        if (fret <= MAX_FRET && fret >= 0)
        {
            selectedFret = fret;
        }
        else
            Debug.LogWarning("Fret greater than " + MAX_FRET + " selected");
    }
    public int getSelectedFret()
    {
        return this.selectedFret;
    }

   
    public void setPitchFromFret()
    {
        this.pitch = Mathf.Pow(fretPitchIncrementfactor, selectedFret);
    }
    public float getPitch()
    {
        return this.pitch;
    }

    public void setAudioPitch()
    {
        stringPlayer.pitch = this.pitch;
    }

    public void playString()
    {
        stringPlayer.PlayDelayed(0f);
    }
  
}

using UnityEngine;
using UnityEngine.Audio;

public class GuitarString : MonoBehaviour
{
    private int selectedFret = 0; //0 stands for open string
    public int MAX_FRET = 12;

    private float fretPitchIncrementfactor = Mathf.Pow(2.0f, 1.0f/12);  //divde octave into 12 semitones by equal temperament
    private float fretPitch = 1.0f;  //the pitch caused only by selected fret and not taking into account string tension
    private float tensionFactor = 1.0f;  //a factor to simulate tension on string
    public float maxTensionFactor = 1.1225f; //two semitones above

    [SerializeField]
    [Range(0.5f, 0.75f)]
    private float minVolume;

    AudioSource stringPlayer;
    public AudioClip baseNote;

    public Transform fretStartPos;
    public Transform fretEndPos;
    public Vector3 basePosition;

    void Start()
    {
        basePosition = transform.position;
        stringPlayer = GetComponent<AudioSource>();
        if (stringPlayer == null)
        {
            stringPlayer = gameObject.AddComponent<AudioSource>() as AudioSource;
        }
        stringPlayer.clip = baseNote;
    }

    public int getFretFromFingerPosition(float fingerPosition)
    {//finger position is in world coordinates
        //assuming string extends along x axis
        float normalizedPosition = (fingerPosition - fretStartPos.position.x) / (fretEndPos.position.x - fretStartPos.position.x);
        return (int) Mathf.Ceil(normalizedPosition * MAX_FRET); //Fret 0 is open fret when no finger is pressed in fretboard
    }

    public void resetString()
    {
        selectFret(0);
    }

    public void selectFret(int fret)
    {
        if (fret <= MAX_FRET && fret >= 0)
        {
            selectedFret = fret;
            setPitchFromFret();
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
        this.fretPitch = Mathf.Pow(fretPitchIncrementfactor, selectedFret);
        setAudioPitch();
    }
    public float getFretPitch()
    {
        return this.fretPitch;
    }

    public void setAudioPitch()
    {
        stringPlayer.pitch = this.fretPitch * this.tensionFactor;
    }

    public void setVolume(float volume)
    {
        stringPlayer.volume = Mathf.Clamp(volume, minVolume, 1.0f);
    }

    public void setTension(float tension)
    {
        if (tension <= maxTensionFactor)
            this.tensionFactor = tension;
        setAudioPitch();
    }
    public void increaseTension(float increment)
    {
        float tension = this.tensionFactor + increment;
        setTension(tension);
    }

    public void playString()
    {
        stringPlayer.PlayDelayed(0f);
    }
  
}

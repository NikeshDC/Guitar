using UnityEngine;

public class EqualTemperamentGuitarString: IMusicString
{
    private int selectedFret = 0; //0 stands for open string
    public int numberOfFrets = 12;

    private float fretPitchIncrementfactor = Mathf.Pow(2.0f, 1.0f / 12);  //divde octave into 12 semitones by equal temperament
    private float fretPitch = 1.0f;  //the pitch caused only by selected fret and not taking into account string tension
    private float tensionFactor = 1.0f;  //a factor to simulate tension on string
    private float tensionIncrementFactor = 0.05946f; //one semitones above
    private float overallPitch = 1.0f;  //pitch obtained by combining tensionFactor and fretPitch

    public EqualTemperamentGuitarString(int numberOfFrets)
    { this.numberOfFrets = numberOfFrets; }
    public EqualTemperamentGuitarString() { }

    public void HoldAt(float holdPosition)
    {//hold the guitar string at given position (between 0 and 1.0f), value near 0 means near start or the first fret
        //cannot select open string in this configuration
        //for selecting open string use resetHold() instead
        this.SetFret((int)Mathf.Ceil(holdPosition * numberOfFrets));
    }
    public void SetFret(int fret)
    {
        fret = (int)Mathf.Clamp(fret, 0, numberOfFrets);
        this.selectedFret = fret;
        SetFretPitch();
    }
    public void ResetHold()
    {//set fret to open string
        this.tensionFactor = 1.0f;
        this.SetFret(0);
    }
    public int GetSelectedFret()
    { return this.selectedFret; }
    private void SetFretPitch()
    {
        this.fretPitch = Mathf.Pow(fretPitchIncrementfactor, selectedFret);
        this.SetOverallPitch();
    }

    private void SetOverallPitch()
    { this.overallPitch = this.fretPitch * this.tensionFactor; }
    public float GetPitch()
    { return this.overallPitch; }

    public void Bend(float bendAmount)
    {//perform guitar bending by increasing tension
        this.tensionFactor = 1.0f + this.tensionIncrementFactor * bendAmount;
        this.SetOverallPitch();
    }
}
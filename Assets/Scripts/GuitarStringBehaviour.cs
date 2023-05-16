using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class GuitarStringBehaviour : MonoBehaviour
{
    AudioSource stringPlayer;
    public AudioClip baseNote;
    [SerializeField]
    private Animator stringAnimator;
    public float animationEndFactor = 0.95f; //after 0.95 * audioclip.length is played stop vibration animation
    public int numberOfFrets;

    public float stringStartPos;
    public float stringEndPos;

    public float minVolume = 0.5f;
    public float maxBendDistance = 0.7f;

    private Vector3 basePosition;  //basePosition of the gameObject

    IMusicString guitarString;  //guitar string that sets its pitch based on function calls for various string events like holding string, bending it, etc and gives pitch under these conditions

    void Start()
    {
        basePosition = transform.position;

        stringPlayer = GetComponent<AudioSource>();
        if (stringPlayer == null)
            stringPlayer = gameObject.AddComponent<AudioSource>() as AudioSource;
        stringPlayer.clip = baseNote;

        guitarString = new EqualTemperamentGuitarString(this.numberOfFrets);
    }

    public void HoldAt(float fingerPositionAlongString)
    {//finger position is in world coordinates
        //assuming string extends along x axis
        float normalizedPosition = (fingerPositionAlongString - stringStartPos) / (stringEndPos - stringStartPos);
        normalizedPosition = Mathf.Clamp(normalizedPosition, 0f, 1.0f);
        guitarString.HoldAt(normalizedPosition);
        stringPlayer.pitch = guitarString.GetPitch();
    }
    public void ResetHold()
    {
        this.transform.position = this.basePosition;
        guitarString.ResetHold();
        stringPlayer.pitch = guitarString.GetPitch();
    }

    public void PlayString()
    {
        stringPlayer.PlayDelayed(0f);
        if(stringAnimator != null)
            stringAnimator.SetBool("vibrate", true);
    }

    public void BendAt(float fingerPositionAlongString, float bendDistance)
    {
        //move this gameObject by bendDistance
        this.transform.position = this.basePosition + new Vector3(0f, bendDistance, 0f);

        //cause guitar string to bend and then set new pitch
        this.HoldAt(fingerPositionAlongString);
        bendDistance = Mathf.Clamp(Mathf.Abs(bendDistance), 0f, this.maxBendDistance);
        float normalizedBendAmount = bendDistance * (1f / maxBendDistance);
        guitarString.Bend(normalizedBendAmount);
        stringPlayer.pitch = guitarString.GetPitch();
    }

    public void Bend(float bendDistance)
    {
        //move this gameObject by bendDistance
        this.transform.position = this.basePosition + new Vector3(0f, bendDistance, 0f);
        bendDistance = Mathf.Clamp(Mathf.Abs(bendDistance), 0f, this.maxBendDistance);
        float normalizedBendAmount = bendDistance * (1f / maxBendDistance);
        guitarString.Bend(normalizedBendAmount);
        stringPlayer.pitch = guitarString.GetPitch();
    }

    public void SetIntensity(float intensity)
    {//set string playing intensity or volume
        stringPlayer.volume = Mathf.Clamp(intensity, this.minVolume, 1.0f);
    }

    void Update()
    {
        //check for audio time and stop animation 
        //stop animation after audio clip is near end as very little or no sound is heard towards end
        if (stringAnimator != null)
            resetVibrateAnimation();
    }

    void resetVibrateAnimation()
    {
        if (stringPlayer.time > baseNote.length * animationEndFactor)
            stringAnimator.SetBool("vibrate", false);
    }

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TouchTest : MonoBehaviour
{
    AudioSource asource;
    // Update is called once per frame
    void Start()
    {
        asource = GetComponent<AudioSource>();
    }

    void Update()
    {
        var fingerCount = 0;
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
            {
                fingerCount++;
            }
        }
        if (fingerCount > 0)
        {
            asource.PlayDelayed(0);
            print("User has " + fingerCount + " finger(s) touching the screen");

            Touch firstT = Input.GetTouch(0);
            print("FingerID: "+ firstT.fingerId + " taps:" + firstT.tapCount + "R: " + firstT.radius + "+-" + firstT.radiusVariance + "Pos:" + firstT.position);
        }

    }
}

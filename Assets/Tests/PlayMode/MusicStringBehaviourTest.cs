using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MusicStringBehaviourTest
{
    MusicStringBehaviour musicStringBehaviour;

    [SetUp]
    public void Setup()
    {
        musicStringBehaviour = new GameObject().AddComponent<MusicStringBehaviour>() as MusicStringBehaviour;
        musicStringBehaviour.numberOfFrets = 12;
        musicStringBehaviour.stringStartPos = 2f;
        musicStringBehaviour.stringEndPos = -2f;
    }

    [UnityTest]
    public IEnumerator hold_changes_audioplayer_pitch()
    {
        yield return null;  //allows start to initialize
        float oldPitch = musicStringBehaviour.GetAudioPlayer().pitch;

        musicStringBehaviour.HoldAt(1.0f);
        float newPitch = musicStringBehaviour.GetAudioPlayer().pitch;

        Assert.AreNotEqual(oldPitch, newPitch);
    }

    [UnityTest]
    public IEnumerator doesnot_bend_beyond_threshold()
    {
        yield return null;  //allows start to initialize

        float bendAmount = musicStringBehaviour.maxBendDistance + 1.0f;
        musicStringBehaviour.BendAt(1.0f, bendAmount);
        float newpos = musicStringBehaviour.transform.position.y;

        Assert.AreEqual(musicStringBehaviour.maxBendDistance, newpos);
    }

    [UnityTest]
    public IEnumerator setintensity_sets_volume_of_audioplayer()
    {
        yield return null;  //allows start to initialize

        musicStringBehaviour.SetIntensity(0.8f);
        float newVolume = musicStringBehaviour.GetAudioPlayer().volume;

        Assert.AreEqual(0.8f, newVolume);
    }

    [UnityTest]
    public IEnumerator bend_causes_pitch_to_change()
    {
        yield return null;  //allows start to initialize
        float oldPitch = musicStringBehaviour.GetAudioPlayer().pitch;

        musicStringBehaviour.Bend(1.0f);
        float newPitch = musicStringBehaviour.GetAudioPlayer().pitch;

        Assert.AreNotEqual(oldPitch, newPitch);
    }

    [UnityTest]
    public IEnumerator reset_hold_resets_audio_pitch_to_one()
    {
        yield return null;  //allows start to initialize

        musicStringBehaviour.BendAt(1.0f, 10f);
        musicStringBehaviour.ResetHold();
        float newPitch = musicStringBehaviour.GetAudioPlayer().pitch;

        Assert.AreEqual(1.0f, newPitch);
    }
}

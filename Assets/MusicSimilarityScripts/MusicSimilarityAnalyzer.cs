using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MusicSimilarityAnalyzer : MonoBehaviour
{
    private float similarityScore;

    private AudioSource audioPlayer1;
    private AudioSource audioPlayer2;

    public AudioMixer audioMixer;

    [SerializeField]
    private AudioClip music1;
    public AudioMixerGroup player1;

    [SerializeField]
    private AudioClip music2;
    public AudioMixerGroup player2;

    private float[] spectrumSamples1;  //spectrum data of played audio for music1
    private float[] spectrumSamples2;  //spectrum data of played audio for music2

    [SerializeField]
    private int numberOfSpectrumSamples = 1024;

    public Text labelText;
    public Text scoreText;

    [Range(1, 10)]
    public int secondAudioPlayWaitTime; //time to wait before playing second audio

    [SerializeField]
    private float analyzeInterval = 10f / 1000f;  //time after which to periodically analyze audio
    
    public BeatsVisualizer beatsVisualizer;

    // Start is called before the first frame update
    void Start()
    {
        numberOfSpectrumSamples = Mathf.NextPowerOfTwo(numberOfSpectrumSamples);

        spectrumSamples1 = new float[numberOfSpectrumSamples];
        spectrumSamples2 = new float[numberOfSpectrumSamples];

        audioPlayer1 = gameObject.AddComponent<AudioSource>() as AudioSource;
        audioPlayer2 = gameObject.AddComponent<AudioSource>() as AudioSource;

        audioPlayer1.outputAudioMixerGroup = player1;
        audioPlayer2.outputAudioMixerGroup = player2;


        audioPlayer1.clip = music1;
        audioPlayer2.clip = music2;

        StartCoroutine(AnalyzeAudio());

        if (beatsVisualizer != null)
            beatsVisualizer.InstantiateBeatObjects(numberOfSpectrumSamples);
    }

    IEnumerator AnalyzeAudio()
    {
        float avgSimilarityScore = 0;
        int nofSamples = 0;

        labelText.text = "Analyzing audio";
        yield return new WaitForSeconds(secondAudioPlayWaitTime);
        labelText.text = "Playing first audio";

        PlayFirstAudio();
        //yield return new WaitForSeconds(music1.length + 0.5f);
        float minAudioLength = Mathf.Min(music1.length, music2.length);
        while (audioPlayer1.isPlaying && audioPlayer1.time < minAudioLength)
        {
            SetSpectrumData();
            if (beatsVisualizer != null)
                beatsVisualizer.SetBeats(spectrumSamples1);
            float currentSimliarity = CompareSpectrumSimilarity(spectrumSamples1, spectrumSamples2);
            this.similarityScore = currentSimliarity;
            //Debug.Log("Time:"+audioPlayer1.time+audioPlayer2.time+"SC: " + this.similarityScore);
            //scoreText.text = this.similarityScore.ToString();
            avgSimilarityScore += currentSimliarity;
            nofSamples++;
            yield return new WaitForSeconds(analyzeInterval);
        }

        if (music1.length > minAudioLength)
            yield return new WaitForSeconds(music1.length - audioPlayer1.time + 0.5f);
        
        
        yield return new WaitForSeconds(secondAudioPlayWaitTime);
        labelText.text = "Playing next audio";

        PlaySecondAudio();
        while (audioPlayer2.isPlaying && audioPlayer2.time < minAudioLength)
        {
            SetSpectrumData();
            if (beatsVisualizer != null)
                beatsVisualizer.SetBeats(spectrumSamples2);
            float currentSimliarity = CompareSpectrumSimilarity(spectrumSamples1, spectrumSamples2);
            this.similarityScore = currentSimliarity;
            //Debug.Log("Time:" + audioPlayer1.time + audioPlayer2.time + "SC: " + this.similarityScore);
            avgSimilarityScore += currentSimliarity;
            nofSamples++;
            yield return new WaitForSeconds(analyzeInterval);
        }

        this.similarityScore = 1 - avgSimilarityScore / nofSamples;
        //Debug.Log("SimilarityScore: " + this.similarityScore);
        labelText.text = "Similarity Score:";
        SetSimilarityScoreText();
    }

    void SetSimilarityScoreText()
    {
        float percentSim = this.similarityScore * 100;
        scoreText.text = percentSim.ToString("0.00");
    }

    void PlayFirstAudio()
    {
        audioMixer.SetFloat("Player1Volume", 0f);
        audioPlayer1.PlayDelayed(0f);
        audioMixer.SetFloat("Player2Volume", -80.0f);
        audioPlayer2.PlayDelayed(0f);
    }

    void PlaySecondAudio()
    {
        audioMixer.SetFloat("Player2Volume", 0f);
        audioPlayer2.PlayDelayed(0f);
        audioMixer.SetFloat("Player1Volume", -80.0f);
        audioPlayer1.PlayDelayed(0f);
    }


    private void SetSpectrumData()
    {
        audioPlayer1.GetSpectrumData(spectrumSamples1, 0, FFTWindow.Blackman);
        audioPlayer2.GetSpectrumData(spectrumSamples2, 0, FFTWindow.Blackman);
    }

    public float CompareSpectrumSimilarity(float[] spectrum1, float[] spectrum2)
    {//returns value from 1 to 0 on how similar are the spectrums; 1 means exactly same 
        if (spectrum1.Length != spectrum2.Length)
        {
            Debug.LogError("Spectrum samples have different size");
            return -1f;
        }
        float diff = 0;
        for (int i = 0; i < spectrum1.Length; i++)
        {
            diff += Mathf.Abs(spectrum1[i] - spectrum2[i]);
        }
        return diff;
    }
}

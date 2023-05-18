using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatsVisualizer : MonoBehaviour
{
    [SerializeField]
    private GameObject beatObjectPrefab;//assume this has object extends 1 units for scale of 1

    public float startPos;
    public float endPos;

    int numberOfBeatIntervals;
    float beatInterval;
    float baseScale;

    private GameObject[] beatObjects;

    float beatScaleFactor = 100f;

    public void InstantiateBeatObjects(int numberOfBeatIntervals)
    {
        DestroyBeatObjects();
        this.numberOfBeatIntervals = numberOfBeatIntervals;
        beatObjects = new GameObject[this.numberOfBeatIntervals];

        beatInterval = (endPos - startPos) / numberOfBeatIntervals;

        for (int i = 0; i < this.numberOfBeatIntervals; i++)
        {
            float xpos = startPos + i * beatInterval;
            GameObject beatObj = Instantiate(beatObjectPrefab, new Vector3(xpos, 0, 0), Quaternion.identity, this.transform) as GameObject;
            baseScale = beatInterval * 0.90f;
            beatObj.transform.localScale = new Vector3(baseScale, baseScale, baseScale);
            beatObjects[i] = beatObj;
        }
    }

    public void SetBeats(float[] beatValues)
    {
        if (beatValues.Length != beatObjects.Length)
            return;

        for (int i = 0; i < beatObjects.Length; i++)
        {
            float newScale = baseScale * beatValues[i] * beatScaleFactor / beatInterval;
            beatObjects[i].transform.localScale = new Vector3(baseScale, newScale, baseScale);
        }
    }

    public void DestroyBeatObjects()
    {
        if (beatObjects != null)
            for (int i = 0; i < beatObjects.Length; i++)
                if (beatObjects[i] != null)
                    Destroy(beatObjects[i]);
    }
}
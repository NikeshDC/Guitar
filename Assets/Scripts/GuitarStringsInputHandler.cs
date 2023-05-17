using System.Collections.Generic;
using UnityEngine;

public abstract class GuitarStringsInputHandler : MonoBehaviour, IFingerTouchHandler
{
    public List<MusicStringBehaviour> guitarStrings;

    public abstract void OnBegin(Touch touch);
    public abstract void OnStationary(Touch touch);
    public abstract void OnMove(Touch touch);
    public abstract void OnEnd(Touch touch);
    public abstract void OnCancel(Touch touch);
}

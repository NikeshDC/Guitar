using UnityEngine;

public interface IFingerTouchHandler
{
    //these methods correspond to the phase of Touch and are expected to be invoked on such events
    void OnBegin(Touch touch);
    void OnStationary(Touch touch);
    void OnMove(Touch touch);
    void OnEnd(Touch touch);
    void OnCancel(Touch touch);
}

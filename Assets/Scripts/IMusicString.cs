public interface IMusicString
{
    void Bend(float amount);
    void HoldAt(float position);
    void ResetHold();
    float GetPitch();
}

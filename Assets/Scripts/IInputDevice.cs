
/// <summary>
/// 
/// </summary>
/// Author James Liu
/// Authored on: 07/01/2015
public interface IRawInputDevice {

    float GetMovementVertical();

    float GetMovementHorizontal();

    bool IsJumping { get; }

    bool IsStandardAttacking { get; }

    bool IsSpecialAttacking { get; }

}

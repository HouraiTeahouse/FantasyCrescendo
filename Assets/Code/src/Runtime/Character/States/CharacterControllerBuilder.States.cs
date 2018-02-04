using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public partial class CharacterControllerBuilder {
  // -----------------------------------------------
  // Ground Movement States
  // -----------------------------------------------
  public CharacterState Idle { get; private set; }
  public CharacterState Walk { get; private set; }

  // Crouch States
  public CharacterState Crouch { get; private set; }
  public CharacterState CrouchStart { get; private set; }
  public CharacterState CrouchEnd { get; private set; }

  public CharacterState Dash { get; private set; }
  public CharacterState Run { get; private set; }
  public CharacterState RunBrake { get; private set; }
  public CharacterState RunTurn { get; private set; }

  // -----------------------------------------------
  // Jump States
  // -----------------------------------------------
  public JumpState Jump { get; private set; }
  public CharacterState JumpStart { get; private set; }
  public JumpState JumpAerial { get; private set; }
  public CharacterState Land { get; private set; }

  // -----------------------------------------------
  // Shield States
  // -----------------------------------------------
  public class ShieldStates {
    public CharacterState On { get; set; }
    public ShieldState Perfect { get; set; }
    public ShieldState Main { get; set; }
    public CharacterState Off { get; set; }
    public CharacterState Broken { get; set; }
    public CharacterState Stunned { get; set; }
  }

  public ShieldStates Shield { get; private set; }

  // Ground Dodge States
  public CharacterState Escape { get; private set; }
  public CharacterState EscapeForward { get; private set; }
  public CharacterState EscapeBackward { get; private set; }

  // -----------------------------------------------
  // Air States 
  // -----------------------------------------------
  public CharacterState Fall { get; private set; }
  public CharacterState FallHelpless { get; private set; }
  public CharacterState EscapeAir { get; private set; }

  // -----------------------------------------------
  // Attacks
  // -----------------------------------------------
  public class SmashAttackStates {
    public SmashChargeState Charge { get; private set; }
    public SmashAttackState Attack { get; private set; }
  }

  // Neutral Combo
  public CharacterState Neutral { get; private set; }

  // Tilt Attacks
  public CharacterState TiltUp { get; private set; }
  public CharacterState TiltSide { get; private set; }
  public CharacterState TiltDown { get; private set; }

  // Smash Attacks
  public SmashAttackStates SmashUp { get; private set; }
  public SmashAttackStates SmashSide { get; private set; }
  public SmashAttackStates SmashDown { get; private set; }

  // Aerial Attacks
  public CharacterState AerialNeutral { get; private set; }
  public CharacterState AerialForward { get; private set; }
  public CharacterState AerialBackward { get; private set; }
  public CharacterState AerialUp { get; private set; }
  public CharacterState AerialDown { get; private set; }
  public CharacterState AerialAttackLand { get; private set; }

  // Special Attacks
  public CharacterState SpecialNeutral { get; private set; }
  public CharacterState SpecialUp { get; private set; }
  public CharacterState SpecialSide { get; private set; }
  public CharacterState SpecialDown { get; private set; }

  // -----------------------------------------------
  // Ledge States
  // -----------------------------------------------
  public CharacterState LedgeGrab { get; private set; }
  public CharacterState LedgeIdle { get; private set; }
  public CharacterState LedgeClimb { get; private set; }
  public CharacterState LedgeEscape { get; private set; }
  public CharacterState LedgeAttack { get; private set; }
  public CharacterState LedgeJump { get; private set; }
  public CharacterState LedgeRelease { get; private set; }

  // -----------------------------------------------
  // Damaged/Launched States
  // -----------------------------------------------
  public CharacterState DamageStun { get; private set; }
  public CharacterState DamageStunAir { get; private set; }
  public CharacterState DamageLaunched { get; private set; }
  public CharacterState DamageFall { get; private set; } // tumble equivalent

  // -----------------------------------------------
  // Grabbed/Thrown States
  // -----------------------------------------------
  // Grabs
  protected CharacterState GrabPull { get; private set; }
  protected CharacterState GrabRunPull { get; private set; }
  protected CharacterState GrabWait { get; private set; }
  protected CharacterState GrabInterrupted { get; private set; }
  protected CharacterState GrabPummel { get; private set; }

  // Throwing
  protected CharacterState ThrowForward { get; private set; }
  protected CharacterState ThrowBackward { get; private set; }
  protected CharacterState ThrowUp { get; private set; }
  protected CharacterState ThrowDown { get; private set; }

  // Grabbed
  protected CharacterState GrabbedPulled { get; private set; }
  protected CharacterState GrabbedWait { get; private set; }
  protected CharacterState GrabbedInterrupt { get; private set; }
  protected CharacterState GrabbedInterruptJump { get; private set; }
  protected CharacterState GrabbedDamaged { get; private set; }

  // Thrown
  protected CharacterState ThrownUp { get; private set; }
  protected CharacterState ThrownDown { get; private set; }
  protected CharacterState ThrownForward { get; private set; }
  protected CharacterState ThrownBackward { get; private set; }

  // -----------------------------------------------
  // Environmental Collision States
  // -----------------------------------------------
  public CharacterState StopCeiling { get; private set; }
  public CharacterState StopWall { get; private set; }
  public CharacterState Crash { get; private set; }

  // Prone
  public CharacterState Prone { get; private set; }
  public CharacterState ProneStand { get; private set; }
  public CharacterState ProneAttack { get; private set; }
  public CharacterState ProneRollLeft { get; private set; }
  public CharacterState ProneRollRight { get; private set; }

  // Teching
  public CharacterState Tech { get; private set; }
  public CharacterState TechRollLeft { get; private set; }
  public CharacterState TechRollRight { get; private set; }
  public CharacterState TechWall { get; private set; }
  public CharacterState TechWallJump { get; private set; }

}

}

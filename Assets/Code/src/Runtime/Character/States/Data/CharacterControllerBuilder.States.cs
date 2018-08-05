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
  public State Idle { get; private set; }
  public State Walk { get; private set; }

  // Crouch States
  public State Crouch { get; private set; }
  public State CrouchStart { get; private set; }
  public State CrouchEnd { get; private set; }

  public State Dash { get; private set; }
  public State Run { get; private set; }
  public State RunBrake { get; private set; }
  public State RunTurn { get; private set; }

  // -----------------------------------------------
  // Jump States
  // -----------------------------------------------
  public JumpState Jump { get; private set; }
  public State JumpStart { get; private set; }
  public JumpState JumpAerial { get; private set; }
  public State Land { get; private set; }

  // -----------------------------------------------
  // Shield States
  // -----------------------------------------------
  public class ShieldStates {
    public ShieldState On { get; set; }
    public ShieldState Perfect { get; set; }
    public ShieldState Main { get; set; }
    public State Off { get; set; }
    public State Broken { get; set; }
    public State Stunned { get; set; }
  }

  public ShieldStates Shield { get; private set; }

  // Ground Dodge States
  public State Escape { get; private set; }
  public State EscapeForward { get; private set; }
  public State EscapeBackward { get; private set; }

  // -----------------------------------------------
  // Air States 
  // -----------------------------------------------
  public State Fall { get; private set; }
  public State FallHelpless { get; private set; }
  public State EscapeAir { get; private set; }

  // -----------------------------------------------
  // Attacks
  // -----------------------------------------------
  public class SmashAttackStates {
    public SmashChargeState Charge { get; private set; }
    public SmashAttackState Attack { get; private set; }
  }

  // Neutral Combo
  public State Neutral { get; private set; }

  // Tilt Attacks
  public State TiltUp { get; private set; }
  public State TiltSide { get; private set; }
  public State TiltDown { get; private set; }

  // Smash Attacks
  public SmashAttackStates SmashUp { get; private set; }
  public SmashAttackStates SmashSide { get; private set; }
  public SmashAttackStates SmashDown { get; private set; }

  // Aerial Attacks
  public State AerialNeutral { get; private set; }
  public State AerialForward { get; private set; }
  public State AerialBackward { get; private set; }
  public State AerialUp { get; private set; }
  public State AerialDown { get; private set; }
  public State AerialAttackLand { get; private set; }

  // Special Attacks
  public State SpecialNeutral { get; private set; }
  public State SpecialUp { get; private set; }
  public State SpecialSide { get; private set; }
  public State SpecialDown { get; private set; }

  // -----------------------------------------------
  // Ledge States
  // -----------------------------------------------
  public State LedgeGrab { get; private set; }
  public State LedgeIdle { get; private set; }
  public State LedgeClimb { get; private set; }
  public State LedgeEscape { get; private set; }
  public State LedgeAttack { get; private set; }
  public State LedgeJump { get; private set; }
  public State LedgeRelease { get; private set; }

  // -----------------------------------------------
  // Damaged/Launched States
  // -----------------------------------------------
  public State DamageStun { get; private set; }
  public State DamageStunAir { get; private set; }
  public State DamageLaunched { get; private set; }
  public State DamageFall { get; private set; } // tumble equivalent

  // -----------------------------------------------
  // Grabbed/Thrown States
  // -----------------------------------------------
  // Grabs
  protected State GrabPull { get; private set; }
  protected State GrabRunPull { get; private set; }
  protected State GrabWait { get; private set; }
  protected State GrabInterrupted { get; private set; }
  protected State GrabPummel { get; private set; }

  // Throwing
  protected State ThrowForward { get; private set; }
  protected State ThrowBackward { get; private set; }
  protected State ThrowUp { get; private set; }
  protected State ThrowDown { get; private set; }

  // Grabbed
  protected State GrabbedPulled { get; private set; }
  protected State GrabbedWait { get; private set; }
  protected State GrabbedInterrupt { get; private set; }
  protected State GrabbedInterruptJump { get; private set; }
  protected State GrabbedDamaged { get; private set; }

  // Thrown
  protected State ThrownUp { get; private set; }
  protected State ThrownDown { get; private set; }
  protected State ThrownForward { get; private set; }
  protected State ThrownBackward { get; private set; }

  // -----------------------------------------------
  // Environmental Collision States
  // -----------------------------------------------
  public State StopCeiling { get; private set; }
  public State StopWall { get; private set; }
  public State Crash { get; private set; }

  // Prone
  public State Prone { get; private set; }
  public State ProneStand { get; private set; }
  public State ProneAttack { get; private set; }
  public State ProneRollLeft { get; private set; }
  public State ProneRollRight { get; private set; }

  // Teching
  public State Tech { get; private set; }
  public State TechRollLeft { get; private set; }
  public State TechRollRight { get; private set; }
  public State TechWall { get; private set; }
  public State TechWallJump { get; private set; }

}

}

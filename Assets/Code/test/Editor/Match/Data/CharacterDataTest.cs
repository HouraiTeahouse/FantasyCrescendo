using HouraiTeahouse;
using HouraiTeahouse.FantasyCrescendo;
using HouraiTeahouse.FantasyCrescendo.Characters;
using HouraiTeahouse.FantasyCrescendo.Players;
using NUnit.Framework;
using System;  
using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;

/// <summary> 
/// Tests for CharacterData instances.
/// </summary>
/// <remarks>
/// Note: These test function as validation for on the data available at build time.
/// If the data is invalid, these tests will fail.
/// </remarks>
[Parallelizable]
internal class CharacterDataTest : AbstractDataTest<CharacterData> {

  public static IEnumerable ComponentCases() => Permutation.Generate(AllData, RequiredTypes);

  static Type[] RequiredTypes => new Type[] {
    typeof(CharacterAnimation),
    typeof(CharacterPhysics),
    typeof(CharacterLedge),
    typeof(CharacterIndicator),
    typeof(CharacterCamera),
    typeof(CharacterRespawn),
    typeof(CharacterStateMachine),
    typeof(CharacterColor),
    typeof(CharacterShield),
    typeof(PlayerActive),
    typeof(CharacterHitboxController),
    typeof(CharacterMovement),
  };

  [Test, TestCaseSource("AllData")]
  public void has_a_prefab(CharacterData character) {
    Assert.NotNull(character.Prefab.Load());
  }

  [Test, TestCaseSource("AllData")]
  public void has_equal_pallete_and_portrait_counts(CharacterData character) {
    var swap = character.Prefab.Load().GetComponent<CharacterColor>();
    Assert.NotNull(swap);
    Assert.AreEqual(swap.Count, character.Portraits.Count);
  }

  [TestCaseSource("ComponentCases")]
  public void has_component(CharacterData character, Type type) {
    var prefab = character.Prefab.Load();
    Assert.IsNotNull(prefab.GetComponentInChildren(type));
  }

  [Test, TestCaseSource("AllData")]
  public void has_valid_portraits(CharacterData character) {
    var portraits = character.Portraits;
    foreach (var portrait in character.Portraits) {
      Assert.NotNull(portrait.Load());
    }
  }

  [Test, TestCaseSource("AllData")]
  public void has_valid_icons(CharacterData character) {
    Assert.NotNull(character.Icon.Load());
  }

  [Test, TestCaseSource("AllData")]
  public void has_valid_home_stage(CharacterData character) {
    Assert.NotNull(character.HomeStage);
  }

  [Test, TestCaseSource("AllData")]
  public void has_valid_victory_theme(CharacterData character) {
    Assert.NotNull(character.VictoryTheme.Load());
  }


}

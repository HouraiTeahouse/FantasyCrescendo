using HouraiTeahouse;
using HouraiTeahouse.FantasyCrescendo;
using HouraiTeahouse.FantasyCrescendo.Characters;
using HouraiTeahouse.FantasyCrescendo.Players;
using NUnit.Framework;
using System;  
using System.Linq;
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

  public static IEnumerable Palletes => Enumerable.Range(0, (int)GameMode.GlobalMaxPlayers);
  public static IEnumerable PalleteCases() => Permutation.Generate(AllData, Palletes);
  public static IEnumerable ComponentCases() => Permutation.GenerateAll(AllData, Palletes, RequiredTypes);

  static Type[] RequiredTypes => new Type[] {
    typeof(CharacterAnimation),
    typeof(CharacterPhysics),
    typeof(CharacterLedge),
    typeof(CharacterIndicator),
    typeof(CharacterRespawn),
    typeof(CharacterStateMachine),
    typeof(CharacterShield),
    typeof(PlayerActive),
    typeof(CharacterHitboxController),
    typeof(CharacterMovement)
  };

  // [Test, TestCaseSource("PalleteCases")]
  // public void every_pallete_has_a_prefab(CharacterData character, int pallete) {
  //   Assert.NotNull(character.GetPallete(pallete).Prefab.Load());
  // }

  // [TestCaseSource("ComponentCases")]
  // public void has_component(CharacterData character, int pallete, Type type) {
  //   var prefab = character.GetPallete(pallete).Prefab.Load();
  //   Assert.IsNotNull(prefab.GetComponentInChildren(type));
  // }

  // [Test, TestCaseSource("PalleteCases")]
  // public void every_pallete_has_a_portrait(CharacterData character, int pallete) {
  //   Assert.NotNull(character.GetPallete(pallete).Portrait.Load());
  // }

  // [Test, TestCaseSource("AllData")]
  // public void has_valid_icons(CharacterData character) {
  //   Assert.NotNull(character.Icon.Load());
  // }

  // [Test, TestCaseSource("AllData")]
  // public void has_valid_home_stage(CharacterData character) {
  //   Assert.NotNull(character.HomeStage);
  // }

  // [Test, TestCaseSource("AllData")]
  // public void has_valid_victory_theme(CharacterData character) {
  //   Assert.NotNull(character.VictoryTheme.Load());
  // }

}

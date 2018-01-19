using HouraiTeahouse.FantasyCrescendo;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

/// <summary> 
/// Tests for CharacterData instances.
/// </summary>
/// <remarks>
/// Note: These test function as validation for on the data available at build time.
/// If the data is invalid, these tests will fail.
/// </remarks>
[Parallelizable]
internal class CharacterDataTest : AbstractDataTest<CharacterData> {

  [Test, TestCaseSource("TestData")]
  public void has_a_prefab(CharacterData character) {
    Assert.NotNull(character.Prefab.Load());
  }

  [Test, TestCaseSource("TestData")]
  public void has_equal_pallete_and_portrait_counts(CharacterData character) {
    var swap = character.Prefab.Load().GetComponent<CharacterColor>();
    Assert.NotNull(swap);
    Assert.AreEqual(swap.Count, character.Portraits.Count);
  }

  [Test, TestCaseSource("TestData")]
  public void has_valid_portraits(CharacterData character) {
    var portraits = character.Portraits;
    foreach (var portrait in character.Portraits) {
      Assert.NotNull(portrait.Load());
    }
  }

  [Test, TestCaseSource("TestData")]
  public void has_valid_icons(CharacterData character) {
    Assert.NotNull(character.Icon.Load());
  }

  [Test, TestCaseSource("TestData")]
  public void has_valid_home_stage(CharacterData character) {
    Assert.NotNull(character.HomeStage);
  }

  [Test, TestCaseSource("TestData")]
  public void has_valid_victory_theme(CharacterData character) {
    Assert.NotNull(character.VictoryTheme.Load());
  }

}

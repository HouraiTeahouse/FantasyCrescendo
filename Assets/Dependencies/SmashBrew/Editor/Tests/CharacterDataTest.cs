using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NUnit.Framework;

namespace HouraiTeahouse.SmashBrew.Tests {
    
    /// <summary>
    /// Tests for CharacterData instances
    /// </summary>
    public class CharacterDataTest {

        private static IEnumerable<CharacterData> data;

        private void LoadCharacterData() {
            if (data == null)
                data = Resources.LoadAll<CharacterData>(string.Empty).Where(d => d != null && d.IsSelectable && d.IsVisible);
        }

        private delegate object AssetFunc(CharacterData data);

        private void CharacterCheck(AssetFunc func) {
            LoadCharacterData();
            foreach(CharacterData character in data)
                Assert.NotNull(func(character));
        }

        [Test]
        public void PrefabTest() {
            // Checks that the Character's prefab is not null
            CharacterCheck(d => d.Prefab.Load());
        }

        [Test]
        public void HasCharacterComponentTest() {
            // Checks that the Character's prefab has a Character script attached
            CharacterCheck(d => d.Prefab.Load().GetComponent<Character>());
        }

        [Test]
        public void RequiredCharacterComponentTest() {
            // Checks for all of the component types marked with RequiredCharacterComponent on all of the CharacterData's prefabs
            LoadCharacterData();
            Type[] requiredTypes = Character.GetRequiredComponents();
            foreach (CharacterData character in data)
                foreach (Type type in requiredTypes) 
                    Assert.NotNull(character.Prefab.Load().GetComponent(type));
        }

        [Test]
        public void PalleteCountTest() {
            // Checks that the pallete count is the same between MaterialSwap and CharacterData
            LoadCharacterData();
            foreach (CharacterData character in data) {
                MaterialSwap swap = character.Prefab.Load().GetComponent<MaterialSwap>();
                Assert.AreEqual(swap.PalleteCount, character.PalleteCount);
            }
        }

        [Test]
        public void PortraitTest() {
            // Checks that all of the portraits for each of the character is not null
            LoadCharacterData();
            foreach (CharacterData character in data)
                for(var i = 0; i < character.PalleteCount; i++)
                    Assert.NotNull(character.GetPortrait(i).Load());
        }

        [Test]
        public void IconTest() {
            // Checks that all of the icons for each character is not null
            CharacterCheck(d => d.Icon.Load());
        }

        [Test]
        public void HomeStageTest() {
            // Check that all of the home stages for each character is not null
            CharacterCheck(d => d.HomeStage.Load());
        }

        [Test]
        public void VictoryThemeTest() {
            // Check that all of the victory theme for each character is not null
            CharacterCheck(d => d.VictoryTheme.Load());
        }

    }

}

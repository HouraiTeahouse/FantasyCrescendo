using System;
using System.Linq;
using UnityEngine;
using NUnit.Framework;

namespace Hourai.SmashBrew.Tests {

    public class CharacterDataTest {

        private static CharacterData[] data;

        private void LoadCharacterData() {
            if (data == null)
                data = Resources.LoadAll<CharacterData>("").Where(d => d != null && d.IsSelectable && d.IsVisible).ToArray();
        }

        private delegate object AssetFunc(CharacterData data);

        private void CharacterCheck(AssetFunc func) {
            LoadCharacterData();
            foreach(CharacterData character in data)
                Assert.NotNull(func(character));
        }

        [Test]
        public void PrefabTest() {
            CharacterCheck(d => d.Prefab.Load());
        }

        [Test]
        public void HasCharacterComponentTest() {
            CharacterCheck(d => d.Prefab.Load().GetComponent<Character>());
        }

        /// <summary>
        /// Checks for all of the component types marked with RequiredCharacterComponent on all of the CharacterData's prefabs
        /// </summary>
        [Test]
        public void RequiredCharacterComponentTest() {
            LoadCharacterData();
            Type[] requiredTypes = Character.GetRequiredComponents();
            foreach (CharacterData character in data)
                foreach (Type type in requiredTypes) 
                    Assert.NotNull(character.Prefab.Load().GetComponent(type));
        }

        [Test]
        public void PortraitTest() {
            LoadCharacterData();
            foreach (CharacterData character in data)
                for(var i = 0; i < character.AlternativeCount; i++)
                    Assert.NotNull(character.GetPortrait(i).Load());
        }

        [Test]
        public void IconTest() {
            CharacterCheck(d => d.Icon.Load());
        }

        [Test]
        public void HomeStageTest() {
            CharacterCheck(d => d.HomeStage.Load());
        }

        [Test]
        public void VictoryThemeTest() {
            CharacterCheck(d => d.VictoryTheme.Load());
        }

    }

}
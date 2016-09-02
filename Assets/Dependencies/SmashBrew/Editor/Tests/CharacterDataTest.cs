using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    internal class AbstractDataTest<T> where T : ScriptableObject, IGameData {

        protected delegate object AssetFunc(T data);

        protected delegate IEnumerable AssetManyFunc(T data);

        protected static IEnumerable<T> data;

        protected void LoadData() {
            if (data == null)
                data = Resources.LoadAll<T>(string.Empty).Where(d => d != null && d.IsSelectable && d.IsVisible);
        }

        protected void Check(AssetFunc func) {
            LoadData();
            foreach (T datum in data) {
                object result = func(datum);
                if (result == null)
                    Log.Info(datum);
                Assert.NotNull(result);
            }
        }

        protected void CheckMany(AssetManyFunc func) {
            LoadData();
            foreach (T datum in data) {
                foreach (object obj in func(datum)) {
                    if (obj == null)
                        Log.Info(datum);
                    Assert.NotNull(obj);
                }
            }
        }

    }

    /// <summary> Tests for CharacterData instances </summary>
    internal class CharacterDataTest : AbstractDataTest<CharacterData> {

        [Test]
        public void PrefabTest() {
            // Checks that the Character's prefab is not null
            Check(d => d.Prefab.Load());
        }

        [Test]
        public void PrefabStatusTest() {
            LoadData();
            foreach (CharacterData character in data) {
                Assert.NotNull(character.Prefab.Load());
                foreach (Status status in character.Prefab.Load().GetComponentsInChildren<Status>())
                    Assert.False(status.enabled);
            }
        }

        [Test]
        public void HasCharacterComponentTest() {
            // Checks that the Character's prefab has a Character script attached
            Check(d => d.Prefab.Load().GetComponent<Character>());
        }

        [Test]
        public void RequiredCharacterComponentTest() {
            // Checks for all of the component types marked with RequiredCharacterComponent on all of the CharacterData's prefabs
            LoadData();
            Type[] requiredTypes = Character.GetRequiredComponents();
            foreach (CharacterData character in data)
                foreach (Type type in requiredTypes)
                    Assert.NotNull(character.Prefab.Load().GetComponent(type));
        }

        [Test]
        public void PalleteCountTest() {
            // Checks that the pallete count is the same between MaterialSwap and CharacterData
            LoadData();
            foreach (CharacterData character in data) {
                var swap = character.Prefab.Load().GetComponent<MaterialSwap>();
                Assert.AreEqual(swap.Count, character.PalleteCount);
            }
        }

        [Test]
        public void PortraitTest() {
            // Checks that all of the portraits for each of the character is not null
            LoadData();
            foreach (CharacterData character in data)
                for (var i = 0; i < character.PalleteCount; i++)
                    Assert.NotNull(character.GetPortrait(i).Load());
        }

        [Test]
        public void IconTest() {
            // Checks that all of the icons for each character is not null
            Check(d => d.Icon.Load());
        }

        [Test]
        public void HomeStageTest() {
            // Check that all of the home stages for each character is not null
            Check(d => d.HomeStage.Load());
        }

        [Test]
        public void VictoryThemeTest() {
            // Check that all of the victory theme for each character is not null
            Check(d => d.VictoryTheme.Load());
        }

    }

}
using UnityEngine;
using NUnit.Framework;
using HouraiTeahouse.SmashBrew.Characters;

namespace HouraiTeahouse.SmashBrew.States {

    public class CharacterControllerBuilderTest {

        [Test]
        public void build_does_not_throw_errors() {
            var instance = ScriptableObject.CreateInstance<CharacterControllerBuilder>();
            instance.BuildCharacterControllerImpl();
            Object.DestroyImmediate(instance);
        }

    }
}


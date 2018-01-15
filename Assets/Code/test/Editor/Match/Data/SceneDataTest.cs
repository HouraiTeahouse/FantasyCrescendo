// using HouraiTeahouse.FantasyCrescendo;
// using UnityEngine;
// using UnityEditor;
// using UnityEngine.TestTools;
// using NUnit.Framework;
// using System.Collections;

// internal class SceneDataTest : AbstractDataTest<SceneData> {

//   [Test, TestCaseSource("TestData")]
//   public void has_valid_preview_image(SceneData scene) {
//     Assert.NotNull(scene.PreviewImage.Load());
//   }

//   [Test, TestCaseSource("TestData")]
//   public void has_valid_icon(SceneData scene) {
//     Assert.NotNull(scene.PreviewImage.Load());
//   }

//   [Test, TestCaseSource("TestData")]
//   public void has_no_null_music(SceneData scene) {
//     foreach (var bgm in scene.Music) {
//       Assert.NotNull(bgm);
//     }
//   }

//   [Test, TestCaseSource("TestData")]
//   public void has_all_valid_music(SceneData scene) {
//     foreach (var bgm in scene.Music) {
//       Assert.NotNull(bgm.Clip.Load());
//     }
//   }

// }

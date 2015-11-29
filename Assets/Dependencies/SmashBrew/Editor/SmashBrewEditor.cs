using System.IO;
using System.Text.RegularExpressions;
using Hourai.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Hourai.SmashBrew.Editor {

    [InitializeOnLoad]
    public static class SmashBrewEditor {

        #region Asset Create Menu Items

        [MenuItem("Assets/Create/SmashBrew/Config")]
        static void CreateConfig() {
            AssetUtil.CreateAssetInProjectWindow<Config>();
        }

        #endregion
      
    }

}
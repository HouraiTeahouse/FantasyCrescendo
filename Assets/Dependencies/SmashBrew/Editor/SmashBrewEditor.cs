using Hourai.Editor;
using UnityEditor;

namespace Hourai.SmashBrew.Editor {

    [InitializeOnLoad]
    public static class SmashBrewEditor {

        #region Asset Create Menu Items

        [MenuItem("Assets/Create/SmashBrew/Config")]
        static void CreateConfig() {
            AssetUtil.CreateAssetInProjectWindow<Config>();
        }

        [MenuItem("Assets/Create/SmashBrew/Character Data")]
        static void CreateCharacterData() {
            AssetUtil.CreateAssetInProjectWindow<CharacterData>();
        }

        [MenuItem("Assets/Create/SmashBrew/Scene Data")]
        static void CreateStageData() {
            AssetUtil.CreateAssetInProjectWindow<SceneData>();
        }

        #endregion
      
    }

}
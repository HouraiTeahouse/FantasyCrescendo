using UnityEngine;
using HouraiTeahouse.Editor;
using UnityEditor;

namespace HouraiTeahouse.SmashBrew.Editor {

    /// <summary>
    /// A custom Editor for CharacterData
    /// </summary>
    [CustomEditor(typeof(CharacterData))]
    internal class CharacterDataEditor : ScriptlessEditor {

        int _previewSelect;
        bool _crop;

        /// <summary>
        /// <see cref="Editor.OnInspectorGUI"/>
        /// </summary>
        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            string message;
            MessageType type;
            if (!AssetUtil.IsResource(target)) {
                message =
                    "This game cannot find this Character Data if it is not in a Resources folder. Please move it to a Resources (sub)folder.";
                type = MessageType.Error;
            } else {
                message = "This Character Data is correctly placed. The game can find it.";
                type = MessageType.Info;
            }
            EditorGUILayout.HelpBox(message, type);
        }

        /// <summary>
        /// <see cref="Editor.HasPreviewGUI"/>
        /// </summary>
        public override bool HasPreviewGUI() {
            if (targets.Length != 1)
                return false;
            var data = target as CharacterData;
            if (data == null || data.PalleteCount <= 0)
                return false;
            if (_previewSelect < 0 || _previewSelect >= data.PalleteCount)
                _previewSelect = 0;
            return data.GetPortrait(_previewSelect).Load() != null;
        }

        /// <summary>
        /// <see cref="Editor.DrawPreview"/>
        /// </summary>
        public override void DrawPreview(Rect previewArea) {
            var data = target as CharacterData;
            if (data == null || data.PalleteCount < 1)
                return;
            using (EditorUtil.Horizontal()) {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("\u25c4", EditorStyles.miniButton))
                    _previewSelect++;
                if (GUILayout.Button(_crop ? "Crop" : "Full", EditorStyles.miniButton))
                    _crop = !_crop;
                if (GUILayout.Button("\u25ba", EditorStyles.miniButton))
                    _previewSelect++;
                GUILayout.FlexibleSpace();
            }
            if (_previewSelect >= data.PalleteCount)
                _previewSelect = 0;
            if (_previewSelect < 0)
                _previewSelect = data.PalleteCount - 1;
            Texture texture = data.GetPortrait(_previewSelect).Load().texture;
            if (_crop) {
                Rect drawRect = previewArea;
                Rect crop = data.CropRect(texture);
                var midPoint = new Vector2(drawRect.x + drawRect.width/2, drawRect.y + drawRect.height/2);
                float drawAspect = drawRect.width/drawRect.height;
                float cropAspect = crop.width/crop.height;
                if (drawAspect > cropAspect) {
                    drawRect.width = drawRect.height*cropAspect;
                    drawRect.x = midPoint.x - drawRect.width/2;
                } else {
                    drawRect.height = drawRect.width/cropAspect;
                    drawRect.y = midPoint.y - drawRect.height/2;
                }
                GUI.DrawTextureWithTexCoords(drawRect, texture, crop);
            } else {
                GUI.DrawTexture(previewArea, texture, ScaleMode.ScaleToFit);
            }
        }

    }

}

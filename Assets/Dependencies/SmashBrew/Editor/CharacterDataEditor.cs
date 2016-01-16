using UnityEngine;
using Hourai.Editor;
using UnityEditor;

namespace Hourai.SmashBrew.Editor {

    [CustomEditor(typeof(CharacterData))]
    public class CharacterDataEditor : UnityEditor.Editor {

        private int _previewSelect;
        private bool _crop;

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

        public override bool HasPreviewGUI() {
            return targets.Length == 1;
        }

        public override void DrawPreview(Rect previewArea) {
            var data = target as CharacterData;
            if (data == null || data.AlternativeCount < 1)
                return;
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("\u25c4", EditorStyles.miniButton))
                _previewSelect++;
            if (GUILayout.Button(_crop ? "Crop" : "Full", EditorStyles.miniButton))
                _crop = !_crop;
            if (GUILayout.Button("\u25ba", EditorStyles.miniButton))
                _previewSelect++;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            if (_previewSelect >= data.AlternativeCount)
                _previewSelect = 0;
            if (_previewSelect < 0)
                _previewSelect = data.AlternativeCount - 1;
            Texture texture = data.GetPortrait(_previewSelect).Load().texture;
            if (_crop) {
                Rect drawRect = previewArea;
                Rect crop = data.CropRect;
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
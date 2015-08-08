using UnityEngine;
using UnityEditor;
using System.Collections;
using Vexe.Editor.Drawers;
using Vexe.Editor.Types;

namespace Crescendo.API.Editor {

    internal class AnimationBoolDrawer : ObjectDrawer<AnimationBool> {

        public override void OnGUI() {
            Debug.Log("Hello");
            EditorMember name = FindRelativeMember("_name");
            name.DisplayText = displayText;
            MemberField(name);
        }

    }

    internal class AnimationFloatDrawer : ObjectDrawer<AnimationFloat> {

        public override void OnGUI() {
            Debug.Log("Hello");
            EditorMember name = FindRelativeMember("_name");
            name.DisplayText = displayText;
            MemberField(name);
        }

    }

    internal class AnimationIntDrawer : ObjectDrawer<AnimationInt> {

        public override void OnGUI() {
            Debug.Log("Hello");
            EditorMember name = FindRelativeMember("_name");
            name.DisplayText = displayText;
            MemberField(name);
        }

    }

    internal class AnimationTirggerDrawer : ObjectDrawer<AnimationTrigger>{

        public override void OnGUI() {
            Debug.Log("Hello");
            EditorMember name = FindRelativeMember("_name");
            name.DisplayText = displayText;
            MemberField(name);
        }

    }

}


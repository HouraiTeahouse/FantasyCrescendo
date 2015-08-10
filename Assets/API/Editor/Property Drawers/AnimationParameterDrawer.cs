using UnityEngine;

namespace Crescendo.API.Editor {

    public class AnimationBoolDrawer : ObjectDrawer<AnimationBool> {

        public override void OnGUI() {
            Debug.Log("Hello");
            EditorMember name = FindRelativeMember("_name");
            name.DisplayText = displayText;
            MemberField(name);
        }

    }

    public class AnimationFloatDrawer : ObjectDrawer<AnimationFloat> {

        public override void OnGUI() {
            Debug.Log("Hello");
            EditorMember name = FindRelativeMember("_name");
            name.DisplayText = displayText;
            MemberField(name);
        }

    }

    public class AnimationIntDrawer : ObjectDrawer<AnimationInt> {

        public override void OnGUI() {
            Debug.Log("Hello");
            EditorMember name = FindRelativeMember("_name");
            name.DisplayText = displayText;
            MemberField(name);
        }

    }

    public class AnimationTirggerDrawer : ObjectDrawer<AnimationTrigger> {

        public override void OnGUI() {
            Debug.Log("Hello");
            EditorMember name = FindRelativeMember("_name");
            name.DisplayText = displayText;
            MemberField(name);
        }

    }

}
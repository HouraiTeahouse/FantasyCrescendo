using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace HouraiTeahouse {

    public static class EditorCommands {

        [MenuItem("Debug/Print Selection %#v")]
        public static void PrintSelection() {
            foreach(Object obj in Selection.objects)
                Log.Info("{0} ({1})", obj.name, obj.GetType());
        }

        [MenuItem("Animator/Zero Transition %#t", true)]
        public static bool ZeroTransitionValidate() {
            return Selection.objects.OfType<AnimatorStateTransition>().Any();
        }

        [MenuItem("Animator/Zero Transition %#t")]
        public static void ZeroTransition() {
            foreach (var transition in Selection.objects.OfType<AnimatorStateTransition>()) {
                transition.exitTime = 1;
                transition.duration = 0;
                transition.offset = 0;
                transition.hasFixedDuration = true;
            }
        }

    }
    
}


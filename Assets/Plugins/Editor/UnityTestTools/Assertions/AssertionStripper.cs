using UnityEditor.Callbacks;
using UnityEngine;
using UnityTest;

public class AssertionStripper {

    [PostProcessScene]
    public static void OnPostprocessScene() {
        if (Debug.isDebugBuild)
            return;
        RemoveAssertionsFromGameObjects();
    }

    private static void RemoveAssertionsFromGameObjects() {
        AssertionComponent[] allAssertions =
            Resources.FindObjectsOfTypeAll(typeof (AssertionComponent)) as AssertionComponent[];
        foreach (AssertionComponent assertion in allAssertions)
            Object.DestroyImmediate(assertion);
    }

}
using System.Collections.Generic;
using UnityEngine;

public class UIInputManager : MonoBehaviour {

    private List<Animator> animators;
    public List<GameObject> availableScreens = null;
    private List<InputInterface> inputInterfaces;
    public ScreenManager screenManager = null;

    // Use this for initialization
    private void Start() {
        animators = new List<Animator>();
        inputInterfaces = new List<InputInterface>();
        foreach (GameObject go in availableScreens) {
            var anim = go.GetComponent<Animator>();
            var inputInterface = go.GetComponent<InputInterface>();

            if (anim == null) {
                Debug.LogError("The " + go.name + " must have an Animator component.");
                Destroy(gameObject);
                return;
            }
            if (inputInterface == null) {
                Debug.LogError("The " + go.name + " must have an InputInterface component.");
                Destroy(gameObject);
                return;
            }

            animators.Add(anim);
            inputInterfaces.Add(inputInterface);
        }
    }

    // Update is called once per frame
    private void Update() {
        Animator openedAnimator = screenManager.getOpenedAnimator();
        if (openedAnimator == null)
            return;

        var i = 0;
        for (i = 0; i < animators.Count; i++) {
            if (animators[i] == openedAnimator) {
                inputInterfaces[i].processInputs();
                return;
            }
        }

        Debug.LogError("The current animator has no available inputInterface");
        Destroy(gameObject);
    }

}
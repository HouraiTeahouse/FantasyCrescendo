using UnityEngine;

public class StageSelectionMenu : InputInterface {

    public Animator nextScreenAnimator = null;
    public ScreenManager screenManager = null;

    // Use this for initialization
    public override void processInputs() {
        if (Input.GetButton("Cancel"))
            screenManager.CloseCurrent();
    }

}
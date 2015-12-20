using UnityEngine;

public class BeforeMenuInputInterface : InputInterface {

    public Animator nextScreenAnimator = null;
    public ScreenManager screenManager = null;

    // Use this for initialization
    public override void processInputs() {
        //if (Input.GetButton("Jump"))
        //    screenManager.OpenPanel(nextScreenAnimator);
    }

}
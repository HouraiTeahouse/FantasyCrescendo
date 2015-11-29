using Hourai;
using Hourai.SmashBrew;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorController : MonoBehaviour {

    public CursorInputModule keyboardControls;
    public StandaloneInputModule mouseControls;
    public float movSpeed = 100.0f;
    public float smooth = 100.0f;
    public float vx;
    public float vy;

    private void OnEnable() {
        mouseControls.enabled = false;
        keyboardControls.enabled = true;
    }

    private void OnDisable() {
        mouseControls.enabled = true;
        keyboardControls.enabled = false;
    }

    // Update is called once per frame
    private void Update() {
        //float inputX = Input.GetAxisRaw ("Horizontal");
        //float inputX = _inputControllerSource.Horizontal.GetAxisValue();
        //float targetSpeed = Mathf.Min(Mathf.Abs(inputX), 1.0f);
        //targetSpeed = movSpeed*targetSpeed;
        //if (inputX < 0.0f)
        //    targetSpeed = -1.0f*targetSpeed;
        //vx = Mathf.Lerp(vx, targetSpeed, smooth*Time.deltaTime);


        ////float inputY = Input.GetAxisRaw ("Vertical");
        //float inputY = _inputControllerSource.Vertical.GetAxisValue();
        //targetSpeed = Mathf.Min(Mathf.Abs(inputY), 1.0f);
        //targetSpeed = movSpeed*targetSpeed;
        //if (inputY < 0.0f)
        //    targetSpeed = -1.0f*targetSpeed;
        //vy = Mathf.Lerp(vy, targetSpeed, smooth*Time.deltaTime);


        //transform.Translate(vx*Time.deltaTime, vy*Time.deltaTime, 0.0f);
    }

}
using HouraiTeahouse;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;

public class StartMatch : MonoBehaviour {

    [SerializeField]
    private InputControlTarget button;

    [SerializeField, Scene]
    private string scene;
	
	// Update is called once per frame
	void Update () {
	    foreach(InputDevice device in InputManager.Devices)
            if(device.GetControl(button).WasPressed)
                SceneManager.LoadScene(scene);
	}
}

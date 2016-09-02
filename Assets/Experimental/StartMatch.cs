using HouraiTeahouse;
using HouraiTeahouse.HouraiInput;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMatch : MonoBehaviour {

    [SerializeField]
    InputTarget button;

    [SerializeField]
    [Scene]
    string scene;

    // Update is called once per frame
    void Update() {
        foreach (InputDevice device in HInput.Devices)
            if (device.GetControl(button).WasPressed)
                SceneManager.LoadScene(scene);
    }

}
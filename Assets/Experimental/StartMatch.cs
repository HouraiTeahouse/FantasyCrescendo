using HouraiTeahouse;
using UnityEngine;
using HouraiTeahouse.HouraiInput;
using UnityEngine.SceneManagement;

public class StartMatch : MonoBehaviour {
    [SerializeField] private InputTarget button;

    [SerializeField, Scene] private string scene;

    // Update is called once per frame
    void Update() {
        foreach (InputDevice device in HInput.Devices)
            if (device.GetControl(button).WasPressed)
                SceneManager.LoadScene(scene);
    }
}

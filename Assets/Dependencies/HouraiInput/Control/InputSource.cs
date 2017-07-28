namespace HouraiTeahouse.HouraiInput {

    public interface InputSource {

        float GetValue(InputDevice inputDevice);
        bool GetState(InputDevice inputDevice);

    }

}
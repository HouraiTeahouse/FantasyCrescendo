namespace Hourai {

    public class InvertedButton : MetaButton {

        public InvertedButton(IInputButton baseButton) : base(baseButton) {}

        public override bool GetButtonValue() {
            return !BaseButton.GetButtonValue();
        }

    }

}
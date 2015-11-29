using System;

namespace Hourai {

    public abstract class MetaButton : IInputButton {

        protected internal IInputButton BaseButton { get; private set; }

        protected MetaButton(IInputButton baseButton) {
            if(baseButton == null)
                throw new ArgumentNullException();
            BaseButton = baseButton;
        }

        public abstract bool GetButtonValue();

    }

    public class AxisButton : MetaButton, IInputAxis {

        public AxisButton(IInputButton baseButton) : base(baseButton) { }

        public override bool GetButtonValue() {
            return BaseButton.GetButtonValue();
        }

        public float GetAxisValue() {
            return (GetButtonValue()) ? 1f : 0f;
        }

    }

    public class InvertedButton : MetaButton {

        public InvertedButton(IInputButton baseButton) : base(baseButton) { }

        public override bool GetButtonValue() {
            return !BaseButton.GetButtonValue();
        }

    }
}

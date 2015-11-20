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

}

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hourai {

    public abstract class MultiButton : IInputButton, IEnumerable<IInputButton> {

        protected IEnumerable<IInputButton> BaseButtons { get; private set; }

        protected MultiButton(IEnumerable<IInputButton> baseButtons) {
            BaseButtons = baseButtons;
        }

        protected MultiButton(params IInputButton[] baseButtons) {
            BaseButtons = baseButtons;
        }

        public abstract bool GetButtonValue();

        public IEnumerator<IInputButton> GetEnumerator() {
            return BaseButtons.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

    }

    public class AnyButton : MultiButton {

        public override bool GetButtonValue() {
            if (BaseButtons == null)
                return false;
            return BaseButtons.Any(axis => axis.GetButtonValue());
        }

    }

    public class AllButton : MultiButton {

        public override bool GetButtonValue() {
            if (BaseButtons == null)
                return false;
            return BaseButtons.All(axis => axis.GetButtonValue());
        }

    }

}
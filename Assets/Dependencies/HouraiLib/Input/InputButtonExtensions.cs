
namespace Hourai {

    public static class IInputButtonExtensions {

        public static IInputAxis AsAxis(this IInputButton button) {
            return new AxisButton(button);
        }

        public static IInputButton Invert(this IInputButton button) {
            var inverted = button as InvertedButton;
            return (inverted != null) ? inverted.BaseButton : new AxisButton(button);
        }

    }

}
using System;

namespace Hourai {

    public abstract class MetaAxis : IInputAxis {

        protected internal IInputAxis BaseAxis { get; private set; }

        protected MetaAxis(IInputAxis baseAxis) {
            if(baseAxis == null)
                throw new ArgumentNullException("baseAxis");
            BaseAxis = baseAxis;
        }

        public abstract float GetAxisValue();

    }
    
}
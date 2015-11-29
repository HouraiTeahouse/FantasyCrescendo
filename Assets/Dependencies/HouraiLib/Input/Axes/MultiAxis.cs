using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hourai {

    public abstract class MultiAxis : IInputAxis, IEnumerable<IInputAxis> {

        protected IEnumerable<IInputAxis> BaseAxes;

        protected MultiAxis(IEnumerable<IInputAxis> baseAxes) {
            BaseAxes = baseAxes;
        }

        protected MultiAxis(params IInputAxis[] baseAxes) {
            BaseAxes = baseAxes;
        }

        public IEnumerator<IInputAxis> GetEnumerator() {
            return BaseAxes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public abstract float GetAxisValue();

    }

    public class AverageAxis : MultiAxis {

        public override float GetAxisValue() {
            if (BaseAxes == null)
                return 0f;

            return BaseAxes.Average(axis => axis.GetAxisValue());
        }

    }

    public class MinAxis : MultiAxis {

        public override float GetAxisValue() {
            if (BaseAxes == null)
                return 0f;
            return BaseAxes.Min(axis => axis.GetAxisValue());
        }

    }

    public class MaxAxis : MultiAxis {

        public override float GetAxisValue() {
            if (BaseAxes == null)
                return 0f;
            return BaseAxes.Max(axis => axis.GetAxisValue());
        }

    }

}
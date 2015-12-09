using System;
using System.Linq;

namespace Hourai {
    
    public delegate float Modifier<T>(T source, float damage);

    public class ModifierList : PriorityList<Func<float, float>> {

        public float Modifiy(float baseValue) {
            if (Count <= 0)
                return baseValue;
            return this.Aggregate(baseValue, (current, mod) => mod(current));
        }

    }


    public class ModifierList<T> : PriorityList<Modifier<T>> {
        
        public float Modifiy(T source, float baseValue) {
            if (Count <= 0)
                return baseValue;
            return this.Aggregate(baseValue, (current, mod) => mod(source, current));
        }

    }
}

using System;

namespace Hourai {
    
    public delegate float Modifier<T>(T source, float damage);

    public class ModifierList : PriorityList<Func<float, float>> {

        public float Modifiy(float baseValue) {
            if (Count <= 0)
                return baseValue;
            float value = baseValue;
            foreach (Func<float, float> mod in this)
                value = mod(value);
            return value;
        }

    }


    public class ModifierList<T> : PriorityList<Modifier<T>> {
        
        public float Modifiy(T source, float baseValue) {
            if (Count <= 0)
                return baseValue;
            float value = baseValue;
            foreach (Modifier<T> mod in this)
                value = mod(source, value);
            return value;
        }

    }
}

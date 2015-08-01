using System;
using UnityEngine;

namespace Crescendo.API {

    [AttributeUsage(AttributeTargets.Field)]
    public class ResourcePathAttribute : PropertyAttribute
    {

        public Type TargetType
        {
            get;
            private set;
        }

        public ResourcePathAttribute(Type assetType)
        {
            if (assetType == null)
                assetType = typeof(UnityEngine.Object);
            TargetType = assetType;
        }

    }

}


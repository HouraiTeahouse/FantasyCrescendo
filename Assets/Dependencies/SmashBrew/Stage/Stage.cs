using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Hourai.SmashBrew {

    /// <summary>
    /// A container behaviour for handling general data about the stage.
    /// </summary>
    /// Author: James Liu
    /// Authored on: 07/01/2015
    public sealed class Stage : Singleton<Stage> {

        [SerializeField]
        private BGMGroup backgroundMusic;

        public static Transform Transform {
            get { return Instance.transform; }
        }

        public static Vector3 Up {
            get { return Transform.up; }
        }

        public static Vector3 Right {
            get { return Transform.right; }
        }

        public static Vector3 Forward {
            get { return Transform.forward; }
        }

    }

}
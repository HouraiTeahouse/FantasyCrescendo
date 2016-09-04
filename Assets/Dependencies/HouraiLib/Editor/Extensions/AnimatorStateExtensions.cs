using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

namespace HouraiTeahouse {

    public static class AnimatorStateExtensions {

        /// <summary> Like GetComponent, except for AnimatorStates A strongly typed </summary>
        /// <typeparam name="T"> the type of the component to look for </typeparam>
        /// <param name="state"> the AnimatorState to query </param>
        /// <returns> the behaviour that was retrieved, or null if none were found </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="state" /> is null </exception>
        public static T GetBehaviour<T>(this AnimatorState state) where T : StateMachineBehaviour {
            StateMachineBehaviour[] behaviours = Argument.NotNull(state).behaviours;
            for (var i = 0; i < behaviours.Length; i++) {
                var test = behaviours[i] as T;
                if (test != null)
                    return test;
            }
            return null;
        }

        /// <summary> Like GetComponent, except for AnimatorStates. </summary>
        /// <param name="state"> the AnimatorState to query </param>
        /// <param name="type"> the type of the behaviour to look for </param>
        /// <returns> the behaviour that was retrieved, or null if none where fun </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="state" /> or <paramref name="type" /> are null </exception>
        public static StateMachineBehaviour GetComponent(this AnimatorState state, Type type) {
            Argument.NotNull(type);
            StateMachineBehaviour[] behaviours = Argument.NotNull(state).behaviours;
            for (var i = 0; i < behaviours.Length; i++)
                if (type.IsInstanceOfType(behaviours[i]))
                    return behaviours[i];
            return null;
        }

        /// <summary> Gets all StateMachineBehaviour that are of a specified type </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="state"> </param>
        /// <returns> </returns>
        public static IEnumerable<T> GetBehaviours<T>(this AnimatorState state) { return state.behaviours.OfType<T>(); }

    }

}
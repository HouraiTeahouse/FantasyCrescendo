using UnityEngine;
using System;

namespace UnityUtilLib {
	/// <summary>
	/// Static game object.
	/// </summary>
	[Serializable]
	public abstract class StaticGameObject<T> : CachedObject where T : StaticGameObject<T> {

		private static T instance;

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static T Instance {
			get { 
				if(instance == null) {
					instance = FindObjectOfType<T>();
				}
				return instance; 
			}
		}

		/// <summary>
		/// The keep between scenes.
		/// </summary>
		public bool keepBetweenScenes;

		/// <summary>
		/// The destroy new instances.
		/// </summary>
		public bool destroyNewInstances;

		/// <summary>
		/// Awake this instance.
		/// </summary>
		public override void Awake () {
			base.Awake ();
			if(instance != null) {
				if(instance.destroyNewInstances) {
					Destroy (gameObject);
					return;
				} else {
					Destroy (instance.GameObject);
				}
			}
			
			instance = (T)this;
			
			if(keepBetweenScenes) {
				DontDestroyOnLoad (gameObject);
			}
		}
	}
}
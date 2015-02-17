using UnityEngine;
using System;
using System.Collections.Generic;

namespace UnityUtilLib {

	/// <summary>
	/// Pooled object.
	/// </summary>
	public abstract class PooledObject<T> : CachedObject where T : MonoBehaviour {
		private IPool parentPool;

		private T prefab;

		/// <summary>
		/// Gets or sets the prefab.
		/// </summary>
		/// <value>The prefab.</value>
		public T Prefab {
			get { 
				return prefab; 
			}
			set {
				prefab = value;
				MatchPrefab(prefab);
			}
		}

		private bool is_active = false;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="UnityUtilLib.PooledObject`1"/> is active.
		/// </summary>
		/// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
		public bool Active {
			get { 
				return is_active; 
			}
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		void Start() {
			GameObject.SetActive (false);
		}

		/// <summary>
		/// Initialize the specified pool.
		/// </summary>
		/// <param name="pool">Pool.</param>
		public void Initialize(IPool pool) {
			//Debug.Log ("initlaized");
			parentPool = pool;
		}

		/// <summary>
		/// Matchs the prefab.
		/// </summary>
		/// <param name="gameObj">Game object.</param>
		public abstract void MatchPrefab (T gameObj);

		/// <summary>
		/// Activate this instance.
		/// </summary>
		public virtual void Activate() {
			is_active = true;
			GameObject.SetActive (true);
		}

		/// <summary>
		/// Deactivate this instance.
		/// </summary>
		public virtual void Deactivate() {
			is_active = false;
			GameObject.SetActive (false);
			parentPool.Return (this);
		}
	}
}
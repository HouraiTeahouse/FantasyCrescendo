using UnityEngine;
using System;
using System.Collections.Generic;

namespace UnityUtilLib {

	/// <summary>
	/// I pool.
	/// </summary>
	public interface IPool {
		void Return (object obj);
	}

	/// <summary>
	/// Game object pool.
	/// </summary>
	public class GameObjectPool<T, P> : CachedObject, IPool where T : PooledObject<P> where P : MonoBehaviour{
		private Queue<T> inactive;
		private bool valid = true;

		/// <summary>
		/// The initial spawn count.
		/// </summary>
		[SerializeField]
		private int initialSpawnCount;

		/// <summary>
		/// The spawn count.
		/// </summary>
		[SerializeField]
		private int spawnCount;

		/// <summary>
		/// The base prefab.
		/// </summary>
		[SerializeField]
		private GameObject basePrefab;

		/// <summary>
		/// The container.
		/// </summary>
		[SerializeField]
		private GameObject container;

		/// <summary>
		/// The active count.
		/// </summary>
		private int activeCount = 0;

		/// <summary>
		/// Awake this instance.
		/// </summary>
		public override void Awake() {
			base.Awake ();
			T[] po = basePrefab.GetComponents<T> ();
			if(po == null || po.Length <= 0) {
				Debug.LogError("The provided prefab must have a subclass of PooledObject attached");
				valid = false;
			} else {
				inactive = new Queue<T>();
				Spawn (initialSpawnCount);
			}
		}

		/// <summary>
		/// Return the specified po.
		/// </summary>
		/// <param name="po">Po.</param>
		public void Return(T po) {
			if(valid) {
				inactive.Enqueue (po);
				activeCount--;
				//Debug.Log(activeCount);
			}
		}

		/// <summary>
		/// Return the specified obj.
		/// </summary>
		/// <param name="obj">Object.</param>
		public void Return (object obj) {
			Return ((T)obj);
		}

		/// <summary>
		/// Get the specified prefab.
		/// </summary>
		/// <param name="prefab">Prefab.</param>
		public PooledObject<P> Get(P prefab = null) {
			if(valid) {
				if(inactive.Count <= 0)
					Spawn (spawnCount);
				T po = inactive.Dequeue ();
				if(prefab != default(P))
					po.Prefab = prefab;
				activeCount++;
				//Debug.Log(active);
				return po;
			}
			return null;
		}

		/// <summary>
		/// Spawn the specified count.
		/// </summary>
		/// <param name="count">Count.</param>
		private void Spawn(int count) {
			Transform parentTrans = container.transform;
			for(int i = 0; i < count; i++) {
				T newPO = ((GameObject)Instantiate(basePrefab)).GetComponent<T>();
				newPO.Transform.parent = parentTrans;
				newPO.Initialize(this);
				inactive.Enqueue(newPO);
			}
		}
	}
}
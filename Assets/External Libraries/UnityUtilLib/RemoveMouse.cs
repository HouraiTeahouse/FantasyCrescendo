using UnityEngine;
using System.Collections;

namespace UnityUtilLib.GUI {
	public class RemoveMouse : MonoBehaviour  {
		void Update () {
			#if UNITY_EDITOR
			DestroyImmediate(this);
			#else
			Screen.lockCursor = true;
			#endif
		}
	}
}
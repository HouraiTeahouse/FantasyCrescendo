#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace PrimitivePlus {
	public class PrimitivePlusEditor : MonoBehaviour {
	
		/* Menu items set to -1 go to the top of the list. This makes it really easy within the editor to select
		"Create > Primitive Plus, etc" right in your hierarchy just as you do with normal primitives. */
		
		#region 3D OBJECTS
		
		// CYLINDER
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/3D Objects/Cylinder", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/3D Objects/Cylinder", false, -1)]
		#endif
		private static void Cylinder(){
			InstantiateObject("Cylinder");
		}
		
		// SEMICYLINDER
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/3D Objects/SemiCylinder", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/3D Objects/SemiCylinder", false, -1)]
		#endif
		private static void SemiCylinder(){
			InstantiateObject("SemiCylinder");
		}
		
		// TUBE
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/3D Objects/Tube", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/3D Objects/Tube", false, -1)]
		#endif
		private static void Tube(){
			InstantiateObject("Tube");
		}
		
		// TUBETHICK
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/3D Objects/TubeThick", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/3D Objects/TubeThick", false, -1)]
		#endif
		private static void TubeThick(){
			InstantiateObject("TubeThick");
		}
		
		// DOME
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/3D Objects/Dome", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/3D Objects/Dome", false, -1)]
		#endif
		private static void Dome(){
			InstantiateObject("Dome");
		}
		
		// ICOSPHERE
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/3D Objects/Icosphere", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/3D Objects/Icosphere", false, -1)]
		#endif
		private static void Icosphere(){
			InstantiateObject("Icosphere");
		}
		
		// TORUS
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/3D Objects/Torus", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/3D Objects/Torus", false, -1)]
		#endif
		private static void Torus(){
			InstantiateObject("Torus");
		}
		
		// CONE
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/3D Objects/Cone", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/3D Objects/Cone", false, -1)]
		#endif
		private static void Cone(){
			InstantiateObject("Cone");
		}
		
		// CONEHALF
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/3D Objects/ConeHalf", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/3D Objects/ConeHalf", false, -1)]
		#endif
		private static void ConeHalf(){
			InstantiateObject("ConeHalf");
		}
		
		// PYRAMID
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/3D Objects/Pyramid", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/3D Objects/Pyramid", false, -1)]
		#endif
		private static void Pyramid(){
			InstantiateObject("Pyramid");
		}
		
		// PYRAMIDTRI
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/3D Objects/PyramidTri", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/3D Objects/PyramidTri", false, -1)]
		#endif
		private static void PyramidTri(){
			InstantiateObject("PyramidTri");
		}
		
		// HEXAGONAL PRISM
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/3D Objects/HexagonalPrism", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/3D Objects/HexagonalPrism", false, -1)]
		#endif
		private static void HexagonalPrism(){
			InstantiateObject("HexagonalPrism");
		}
		
		// TRIANGULAR PRISM
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/3D Objects/TriangularPrism", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/3D Objects/TriangularPrism", false, -1)]
		#endif
		private static void TriangularPrism(){
			InstantiateObject("TriangularPrism");
		}
		
		// WEDGE
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/3D Objects/Wedge", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/3D Objects/Wedge", false, -1)]
		#endif
		private static void Wedge(){
			InstantiateObject("Wedge");
		}
		
		// DIAMOND
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/3D Objects/Diamond", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/3D Objects/Diamond", false, -1)]
		#endif
		private static void Diamond(){
			InstantiateObject("Diamond");
		}
		
		// TUBEDCUBE
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/3D Objects/TubedCube", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/3D Objects/TubedCube", false, -1)]
		#endif
		private static void TubedCube(){
			InstantiateObject("TubedCube");
		}
		
		// CORNER
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/3D Objects/Corner", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/3D Objects/Corner", false, -1)]
		#endif
		private static void Corner(){
			InstantiateObject("Corner");
		}
		
		// HOLLOWCUBE
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/3D Objects/HollowCube", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/3D Objects/HollowCube", false, -1)]
		#endif
		private static void HollowCube(){
			InstantiateObject("HollowCube");
		}
		
		// CROSS3D
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/3D Objects/Cross3D", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/3D Objects/Cross3D", false, -1)]
		#endif
		private static void Cross3D(){
			InstantiateObject("Cross3D");
		}
		
		// STAR3D
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/3D Objects/Star3D", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/3D Objects/Star3D", false, -1)]
		#endif
		private static void Star3D(){
			InstantiateObject("Star3D");
		}
		
		// HEART3D
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/3D Objects/Heart3D", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/3D Objects/Heart3D", false, -1)]
		#endif
		private static void Heart3D(){
			InstantiateObject("Heart3D");
		}
		
		#endregion
		
		#region 2D OBJECTS
		
		// CIRCLE
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/2D Objects/Circle", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/2D Objects/Circle", false, -1)]
		#endif
		private static void Circle(){
			InstantiateObject("Circle");
		}
		
		// SEMICIRCLE
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/2D Objects/SemiCircle", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/2D Objects/SemiCircle", false, -1)]
		#endif
		private static void SemiCircle(){
			InstantiateObject("SemiCircle");
		}
		
		// TRIANGLE
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/2D Objects/Triangle", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/2D Objects/Triangle", false, -1)]
		#endif
		private static void Triangle(){
			InstantiateObject("Triangle");
		}
		
		// HEXAGON
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/2D Objects/Hexagon", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/2D Objects/Hexagon", false, -1)]
		#endif
		private static void Hexagon(){
			InstantiateObject("Hexagon");
		}
		
		// RHOMBUS
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/2D Objects/Rhombus", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/2D Objects/Rhombus", false, -1)]
		#endif
		private static void Rhombus(){
			InstantiateObject("Rhombus");
		}
		
		// CROSS
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/2D Objects/Cross", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/2D Objects/Cross", false, -1)]
		#endif
		private static void Cross(){
			InstantiateObject("Cross");
		}
		
		// STAR
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/2D Objects/Star", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/2D Objects/Star", false, -1)]
		#endif
		private static void Star(){ 
			InstantiateObject("Star");
		}
		
		// HEART
		#if UNITY_4_6
		[MenuItem("GameObject/Primitive Plus/2D Objects/Heart", false, -1)]
		#else
		[MenuItem("GameObject/Create Other/Primitive Plus/2D Objects/Heart", false, -1)]
		#endif
		private static void Heart(){ 
			InstantiateObject("Heart");
		}
		
		#endregion
		
		private static void InstantiateObject(string path){
			GameObject obj = Resources.Load(path, typeof(GameObject)) as GameObject;		// Find the game object in the Resources folder
			GameObject go = Instantiate(obj) as GameObject;									// Instantiate the game object
			
			Selection.activeGameObject = go;												// Select the game object
			Camera sceneCam = SceneView.currentDrawingSceneView.camera;						// Find the editors camera
			Vector3 insPos = sceneCam.ViewportToWorldPoint(new Vector3(0.5f,0.5f,3f));		// Offset a vector from your editors camera
			
			go.name = obj.name;																// Remove the "Clone" from the name
			go.transform.position = insPos;													// Move game object to your new vector
			go.transform.rotation = Quaternion.Euler(Vector3.zero);							// Zero out the rotation
		}
	}
}
#endif
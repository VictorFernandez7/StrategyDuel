using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (RoomGenerator))]
public class RoomGeneratorEditor : Editor {

	public override void OnInspectorGUI ()
	{

		RoomGenerator map = target as RoomGenerator;

		if (DrawDefaultInspector ()) {
			//map.GenerateMap ();
		}

		//if(Application.isPlaying) {
			if (GUILayout.Button("Generate Map")) {
				map.GenerateMap ();
			}
		//}
	}
	
}
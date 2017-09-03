using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogTree))]
public class DialogTreeInspector : Editor {

	public override void OnInspectorGUI() {
		DialogTree myTarget = (DialogTree)target;

		EditorGUILayout.LabelField ("Number of nodes: ", myTarget.nodes.Count.ToString());

		//give a preview of the root node
		EditorGUILayout.LabelField("Root node:");
		//check node exists
		if (myTarget.rootNodeIndex >= 0 && myTarget.nodes.Count > myTarget.rootNodeIndex) {
			EditorGUILayout.LabelField (myTarget.nodes[myTarget.rootNodeIndex].nodeText);
		} else {
			EditorGUILayout.LabelField ("null");
		}

		//add a button to open an editor window
		if (GUILayout.Button ("Edit")) {
			DialogWindow win = EditorWindow.GetWindow<DialogWindow> ();
			win.Init (myTarget, target.name);
		}
	}

}

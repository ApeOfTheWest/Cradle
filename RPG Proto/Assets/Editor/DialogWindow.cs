using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogWindow : EditorWindow {

	private string testString;

	[MenuItem("Window/Dialog Editor")]
	public static void ShowWindow() {
		EditorWindow.GetWindow<DialogWindow> ("Dialog Editor");
	}

	void OnGUI() {
		//window code
		testString = EditorGUILayout.TextField("Name", testString);

		if (GUILayout.Button ("Press me")) {
			Debug.Log ("pressed");
		}
	}
}

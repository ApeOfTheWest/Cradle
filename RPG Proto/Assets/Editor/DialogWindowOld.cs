using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogWindowOld : EditorWindow {
	//the tree to modify
	private DialogTree tree;
	//private SerializedObject serialized;

	//the scroll position of the node display
	private Vector2 nodeScrollPos;
	//scroll position of the window scroll field
	private Vector2 windowScrollPos;
	//scroll pos of responses
	private Vector2 responseScrollPos;

	//the selected node
	private DialogNode selected;

	private string testString;

	[MenuItem("Window/Dialog Editor Old")]
	public static void ShowWindow() {
		EditorWindow.GetWindow<DialogWindowOld> ("Dialog Editor");
	}

	public void Init(DialogTree editTree, string dialogName) {
		tree = editTree;
		//serialized = new SerializedObject (tree);

		//start at the root node if it exists
		if (tree.rootNodeIndex >= 0 && tree.rootNodeIndex < tree.nodes.Count) {
			Select (tree.nodes [tree.rootNodeIndex]);
		}

		this.titleContent.text = dialogName;
	}

	void OnGUI() {
		EditorGUI.BeginChangeCheck ();

		//make sure the tree is non null
		if (tree != null) {
			//update the serialized obj
			//serialized.Update();
			//record object before making changes
			Undo.RecordObject (tree, "Dialog tree changed");

			//organise all the different sections in a set of vertical groups
			EditorGUILayout.BeginVertical();

			windowScrollPos = EditorGUILayout.BeginScrollView (windowScrollPos);

			EditorGUILayout.BeginHorizontal();

			//draw the root node seperate from the scroll pane if it exists
			if (tree.rootNodeIndex >= 0 && tree.rootNodeIndex < tree.nodes.Count) {
				DrawRoot (tree.nodes [tree.rootNodeIndex], tree.rootNodeIndex);
			}

			EditorGUILayout.EndHorizontal ();

			//draw a label for the other nodes
			EditorGUILayout.LabelField("Dialog Tree Nodes:");

			nodeScrollPos = EditorGUILayout.BeginScrollView (nodeScrollPos, GUILayout.Height(130f));
			EditorGUILayout.BeginHorizontal();

			//draw all existing nodes
			for (int i = 0; i < tree.nodes.Count; i++) {
				DrawNodeSummary (tree.nodes [i], i);
			}

			//draw a button to create a new node at the end
			if (GUILayout.Button ("New Node")) {
				//serialized.FindProperty ("nodes");

				tree.AddNode (new DialogNode());
			}
				
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndScrollView ();

			//draw the node editor beneath these
			DrawNodeEditor();

			EditorGUILayout.EndScrollView ();

			EditorGUILayout.EndVertical();

			/*//organize a top bar and display all the nodes in it in order
			EditorGUILayout.BeginVertical();

			//draw the root node seperate from the scroll pane if it exists
			if (tree.rootNodeIndex >= 0 && tree.rootNodeIndex < tree.nodes.Count) {
				DrawRoot (tree.nodes [tree.rootNodeIndex], tree.rootNodeIndex);
			}


			//make it scrollable
			nodeScrollPos = EditorGUILayout.BeginScrollView(nodeScrollPos, false, false); 

			//draw all existing nodes
			for (int i = 0; i < tree.nodes.Count; i++) {
				DrawNodeSummary (tree.nodes [i], i);
			}

			//draw a button to create a new node at the end
			if (GUILayout.Button ("New Node")) {
				tree.AddNode (new DialogNode ());
			}

			EditorGUILayout.EndScrollView ();

			EditorGUILayout.EndVertical();

			//organize a bottom section with a preview of the node, and tools to edit it or add responses
			EditorGUILayout.BeginVertical();
			DrawNodeEditor ();
			EditorGUILayout.EndVertical();*/
		} else {
			EditorGUILayout.LabelField ("null Dialog");
		}

		//window code
		//testString = EditorGUILayout.TextField("Name", testString);

		//if (GUILayout.Button ("Press me")) {
			//Debug.Log ("pressed");
		//}

		//check if any changes were made and mark asset for saving if so
		if (EditorGUI.EndChangeCheck()) {
			Undo.FlushUndoRecordObjects ();
			EditorUtility.SetDirty (tree);
			//AssetDatabase.SaveAssets ();
			//AssetDatabase.Refresh ();
			//serialized.ApplyModifiedProperties();
		}
	}

	//draws the root node of the dialog if it exists
	private void DrawRoot(DialogNode node, int index) {
		//draw a label above the root then draw the root node
		EditorGUILayout.BeginVertical ();

		EditorGUILayout.LabelField ("Root Node:");

		EditorGUILayout.LabelField ("#" + index.ToString ());

		if (GUILayout.Button("Select")) {
			Select (node);
		}
		if (GUILayout.Button("Clear")) {
			tree.rootNodeIndex = -1;
		}

		EditorGUILayout.EndVertical ();
	}

	//this method will draw a node button where it is called, this allows for the selection of a node, the deletion of a node and displays basic info
	private void DrawNodeSummary(DialogNode node, int index) {
		if (selected == node) {
			EditorGUILayout.BeginVertical();
		} else {
			//start a new vertical group for each nodes components
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		}

		EditorGUILayout.LabelField ("#" + index.ToString ());

		//make horizontal for the buttons
		EditorGUILayout.BeginHorizontal();


		//move node to the left
		if (GUILayout.Button("<")) {
			tree.MoveNode (index, index - 1);
		}

		//create node to the left button
		if (GUILayout.Button("<+")) {
			//create node at the index currently occupied
			tree.AddNodeAtIndex(new DialogNode(), index);
		}

		//selection button, highlight if this is current selection
		if (GUILayout.Button("Select")) {
			Select (tree.nodes [index]);
		}

		//deletion button
		if (GUILayout.Button ("X")) {
			tree.RemoveNode (index);
		}

		//create node to the right button
		if (GUILayout.Button("+>")) {
			//create node at the index currently occupied
			tree.AddNodeAtIndex(new DialogNode(), index + 1);
		}

		if (GUILayout.Button(">")) {
			tree.MoveNode (index, index + 1);
		}

		EditorGUILayout.EndHorizontal ();

		//display basic info for the node
		//display two lines of text
		EditorGUILayout.LabelField(node.nodeText, GUILayout.MaxHeight(60));

		EditorGUILayout.EndVertical ();
	}

	//this method will draw the node editor with the selected node, will draw nothing if no node is selected
	private void DrawNodeEditor() {
		//check that there is a selection
		if (selected != null) {
			//get the index of the node
			int index = 0;
			for (int i=0; i <tree.nodes.Count; i++) {
				if (tree.nodes[i] == selected) {
					index = i;
					break;
				}
			}

			//start a vertical group
			EditorGUILayout.BeginVertical ();

			EditorGUILayout.LabelField ("Index: #" + index.ToString ());

			//get the height of the largest sprite
			float largestHeight = 0;
			//and the combined with
			float spriteWidths = 0;
			if (selected.leftPortrait != null && selected.leftPortrait.rect.height > largestHeight) {
				largestHeight = selected.leftPortrait.rect.height;
				spriteWidths += selected.leftPortrait.rect.width;
			}
			if (selected.rightPortrait != null && selected.rightPortrait.rect.height > largestHeight) {
				largestHeight = selected.rightPortrait.rect.height;
				spriteWidths += selected.rightPortrait.rect.width;
			}

			//start horizontal group to hold the sprite previews
			Rect spriteRect = EditorGUILayout.BeginHorizontal (GUILayout.Height(largestHeight), GUILayout.Width(spriteWidths));

			//use label to create space for the sprite draws
			GUILayout.Label("");

			if (selected.leftPortrait != null) {
				spriteRect.width = selected.leftPortrait.rect.width;
				spriteRect.height = selected.leftPortrait.rect.height;

				DrawSprite (spriteRect, selected.leftPortrait, false);

				//move position along for next sprite
				spriteRect.position = new Vector2(spriteRect.position.x + selected.leftPortrait.rect.width + 50, spriteRect.position.y);
			}
			if (selected.rightPortrait != null) {
				spriteRect.width = selected.rightPortrait.rect.width;
				spriteRect.height = selected.rightPortrait.rect.height;

				DrawSprite (spriteRect, selected.rightPortrait, true);
			}
			//draw the left and right sprites
			/*if (selected.leftPortrait != null) {
				GUILayout.Label(selected.leftPortrait.texture);
			}
			if (selected.rightPortrait != null) {
				GUILayout.Label(selected.rightPortrait.texture);
			}*/

			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			//draw the sprite selection
			EditorGUILayout.LabelField("Left portrait:");
			selected.leftPortrait = (Sprite)EditorGUILayout.ObjectField (selected.leftPortrait, typeof(Sprite), false);

			EditorGUILayout.LabelField("Right portrait:");
			selected.rightPortrait = (Sprite)EditorGUILayout.ObjectField (selected.rightPortrait, typeof(Sprite), false);
			EditorGUILayout.EndHorizontal ();

			//draw a textfield for the speaker's name
			selected.speakerName = EditorGUILayout.TextField("Speaker: ", selected.speakerName);

			EditorGUILayout.LabelField ("Text:");
			//draw a textfield to edit the text in as well as preview it
			selected.nodeText = EditorGUILayout.TextArea(selected.nodeText);

			//flags for the node
			selected.nodeFlag = EditorGUILayout.IntField("Node Flag: ", selected.nodeFlag);

			//button to make this the root node
			if (GUILayout.Button ("Make Root")) {
				tree.rootNodeIndex = index;
			}

			EditorGUILayout.BeginHorizontal ();
			//button to taggle whether this ends the dialog
			EditorGUILayout.LabelField("End dialog on this node?");
			//selected.endDialog = EditorGUILayout.Toggle (selected.endDialog);

			EditorGUILayout.EndHorizontal ();
				
			//buttons to create certain types of dialog response
			if (GUILayout.Button("Add Response")) {
				selected.responses.Add(new DialogResponse());
			}

			//scrollview with a list of all the responses, and ways to edit / delete them
			responseScrollPos = EditorGUILayout.BeginScrollView (responseScrollPos);
			EditorGUILayout.BeginHorizontal ();
			//draw responses
			for (int i = 0; i < selected.responses.Count; i++) {
				DrawResponse (selected.responses [i], i);
			}
				
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndScrollView ();

			EditorGUILayout.EndVertical ();
		} else {
			EditorGUILayout.LabelField ("No node selected");
		}
	}

	//draw a sprite at the given position
	private void DrawSprite(Rect position, Sprite sprite, bool flip) {
		Vector2 fullsize = new Vector2 (sprite.texture.width, sprite.texture.height);
		Vector2 size = new Vector2 (sprite.textureRect.width, sprite.textureRect.height);

		Rect coords = sprite.textureRect;
		coords.x /= fullsize.x;
		coords.width /= fullsize.x;
		coords.y /= fullsize.y;
		coords.height /= fullsize.y;

		Vector2 ratio;
		ratio.x = position.width / size.x;
		ratio.y = position.height / size.y;
		float minRatio = Mathf.Min (ratio.x, ratio.y);

		Vector2 center = position.center;
		position.width = size.x * minRatio;
		position.height = size.y * minRatio;
		position.center = center;

		GUI.DrawTextureWithTexCoords (position, sprite.texture, coords); 
	}

	//method to draw the given response (can be of a few different types)
	private void DrawResponse(DialogResponse response, int index) {
		EditorGUILayout.BeginVertical (EditorStyles.helpBox);

		EditorGUILayout.BeginHorizontal();

		//move response to the left
		if (GUILayout.Button("<")) {
			if (index > 0) {
				selected.responses.Remove (response);
				selected.responses.Insert (index-1, response);
			}
		}
			
		//things to draw regardless of the type of response
		//the delete button
		if (GUILayout.Button ("X")) {
			//delete the response from the asset
			//AssetDatabase.DeleteAsset(selected.responses[index]);

			selected.responses.RemoveAt (index);
		}

		if (GUILayout.Button(">")) {
			if (index < selected.responses.Count - 1) {
				selected.responses.Remove (response);
				selected.responses.Insert (index+1, response);
			}
		}
			
		EditorGUILayout.EndHorizontal();
			
		//draw the link node index, with a button to goto it
		EditorGUILayout.BeginHorizontal();

		response.linkNodeIndex = EditorGUILayout.IntField ("Linked Node:", response.linkNodeIndex);
		//if the link is invalid show warning
		if (response.linkNodeIndex < 0 || response.linkNodeIndex > tree.nodes.Count) {
			EditorGUILayout.LabelField ("Note: Link is invalid, dialog will end when selected");
		} else {
			if (GUILayout.Button ("Goto Link")) {
				Select (tree.nodes [response.linkNodeIndex]);
			}
		}

		EditorGUILayout.EndHorizontal ();

		response.responseLabel = EditorGUILayout.TextField ("Response Label:", response.responseLabel);

		response.responseFlag = EditorGUILayout.IntField ("Response Flag:", response.responseFlag);
			
		EditorGUILayout.EndVertical ();
	}

	//called when a node is selected
	private void Select(DialogNode node) {
		GUI.FocusControl (null);

		selected = node;
		//clear focus to prevent old selected fields being dragged into the new one
	}
		
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogWindow : EditorWindow {
	//the tree to modify
	private DialogTree tree;

	//the currently selected node
	//save it as an index so that the index can be easily looked up
	private int selectedNode = -1;

	//the currently selected node being linked from (and response index if applicable)
	private int startLinkNode = -1;
	private int linkResponseIndex = -1;
	//used to join up a response to a node by clicking on one and then the other

	//whether the editor is looking at the node editor or map
	private bool nodeEditView = false;

	//various scroll wheel positions
	//scroll position of the window scroll field
	private Vector2 editorScrollPos;
	//scroll pos of responses
	private Vector2 responseScrollPos;
	//scroll pos of node map
	private Vector2 mapScrollPos;

	//how much space to show around the position of all the nodes, used to expand the scrollpanel to fit the windows
	private const float nodeMapPadding = 500;

	//the rect representing the bounds of the node map that must be visible
	//must be updated when a node is moved and at the start
	private Rect visibleRect = new Rect();

	//class to represent a node on the map for graphics purposes
	private class MapNode {
		public Rect nodeRect = new Rect();

		//list of all the responses in the rectangle
		public List<Rect> responseRects = new List<Rect>();
	}

	//a list of all the nodes currently being represented on the map
	private List<MapNode> mapNodes = new List<MapNode>();

	[MenuItem("Window/Dialog Editor")]
	public static void ShowWindow() {
		EditorWindow.GetWindow<DialogWindow> ("Dialog Editor");
	}

	public void Init(DialogTree editTree, string dialogName) {
		tree = editTree;
		//set the title to the dialog name
		this.titleContent.text = dialogName;

		//setup visible node rectangle
		UpdateVisibleRectangle ();
	}

	void OnGUI() {
		//start recording any possible changes 
		EditorGUI.BeginChangeCheck ();
		//record object before making changes
		Undo.RecordObject (tree, "Dialog tree changed");

		if (nodeEditView) {
			DrawEditorPanel ();
		} else {
			DrawNodalMap ();
		}

		//check if any changes were made and mark asset for saving if so
		if (EditorGUI.EndChangeCheck()) {
			//tell the undo system to mark records any changes for undoing
			Undo.FlushUndoRecordObjects ();
			//and tell the editor this asset must be resaved
			EditorUtility.SetDirty (tree);
			//AssetDatabase.SaveAssets ();
			//AssetDatabase.Refresh ();
		}
	}

	//update the visible rectangle by looping through the current nodes and finding their positions
	private void UpdateVisibleRectangle() {
		//start visible rectangle out with a size of 0 and a position of 0
		visibleRect.position  = Vector2.zero;
		visibleRect.size = Vector2.zero;

		//now loop through the nodes and expand the rect to fit them
		for (int i = 0; i < tree.nodes.Count; i++) {
			Vector2 paddedPosition = tree.nodes [i].nodeEditorPosition;
			paddedPosition += new Vector2(nodeMapPadding, nodeMapPadding);

			//expand the max values if needed
			if (paddedPosition.x > visibleRect.xMax) {
				visibleRect.xMax = paddedPosition.x;
			}
			if (paddedPosition.y > visibleRect.yMax) {
				visibleRect.yMax = paddedPosition.y;
			}
		}
	}

	//used to select a node for editing
	//passing in a non existent node will deselect the current one
	private void SelectNode(int nodeIndex) {
		GUI.FocusControl (null);
		//clear focus to prevent old selected fields being dragged into the new one

		if (NodeExists (nodeIndex)) {
			//if the node exists then switch to the editor panel from the nodal map
			selectedNode = nodeIndex;

			//set node edit view to true on selecting it
			nodeEditView = true;
		} else {
			//set selection to negative to show nothing is selected
			selectedNode = -1;
			//make sure node edit view is false
			nodeEditView = false;
		}
	}

	//call when deleting a node, this should be called to automatically cleanup any gui stuff as well as deleting it
	private void DeleteNode(int nodeIndex) {
		if (NodeExists(nodeIndex)) {
			//delete it from the tree
			tree.RemoveNode(nodeIndex);

			//if the node is currently selected then deselect it
			if (nodeIndex == selectedNode) {
				SelectNode (-1);
			} else {
				//if not then the selected node index must be updated when the nodes are moved around after deletion
				//MUST happen after the deletion takes place
				tree.AdjustIndexDeleted(ref selectedNode, nodeIndex);
			}
				
			//if node is being linked clear the link
			//clear linking if moving to edit mode
			startLinkNode = -1;
			linkResponseIndex = -1;
		}
	}
	//convenience method to check if the node indexed exists in the tree
	private bool NodeExists(int nodeIndex) {
		return (nodeIndex >= 0 && nodeIndex < tree.nodes.Count);
	}

	//draw the dialog nodal map 
	private void DrawNodalMap() {
		//button to go to the node editor
		if (GUILayout.Button ("Node Editor")) {
			nodeEditView = true;
			//clear linking if moving to edit mode
			startLinkNode = -1;
			linkResponseIndex = -1;
		}

		//display the root node index along with a goto root button if it exists
		if (NodeExists (tree.rootNodeIndex)) {
			EditorGUILayout.LabelField ("Root node #" + tree.rootNodeIndex);
			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button("Goto root")) {
				//center the node panel over the root
			}
			if (GUILayout.Button ("Clear root")) {
				//null the root
				tree.rootNodeIndex = -1;
			}

			EditorGUILayout.EndHorizontal ();
		}

		//button to add a new node at the centre position of the scroll pane
		if (GUILayout.Button ("New Node")) {
			//make a new node
			tree.AddNode(new DialogNode());
			//set the editor position to the top left of the viewable area
			tree.nodes[tree.nodes.Count - 1].nodeEditorPosition = mapScrollPos;
		}

		//start a scrollable panel for the nodal map
		mapScrollPos = EditorGUILayout.BeginScrollView (mapScrollPos);

		EditorGUILayout.BeginVertical (GUILayout.Width (visibleRect.width), GUILayout.Height (visibleRect.height));

		//make an empty label of a certain size to scroll the map
		EditorGUILayout.LabelField("");
		EditorGUILayout.EndVertical ();

		//now draw each node inside the scroll view
		//draw the curves first
		for (int i = 0; i < tree.nodes.Count; i++) {
			DrawNodeCurves (tree.nodes [i], i);
		}

		BeginWindows ();
		for (int i = 0; i < tree.nodes.Count; i++) {
			DrawNode(tree.nodes [i], i);
		}
		EndWindows ();

		EditorGUILayout.EndScrollView ();

		//if the mouse was just down and nothing has focus then deselect the current link
		//if (GUI.fo
	}
		
	private void DrawNode(DialogNode node, int nodeIndex) {
		//if theres not enough nodes in the list then update it
		while (nodeIndex > mapNodes.Count - 1) {
			mapNodes.Add (new MapNode ());
		}

		mapNodes [nodeIndex].nodeRect.position = node.nodeEditorPosition;

		//draw this node at the specified point in the window
		mapNodes[nodeIndex].nodeRect = GUILayout.Window(nodeIndex, mapNodes[nodeIndex].nodeRect, DrawNodeWindow, "#" + nodeIndex.ToString());

		//stop position from being negative
		Vector2 newPosition = mapNodes [nodeIndex].nodeRect.position;
		if (newPosition.x < 0) {
			newPosition.x = 0;
		}
		if (newPosition.y < 0) {
			newPosition.y = 0;
		}
		mapNodes [nodeIndex].nodeRect.position = newPosition;

		//if the position has changed then update the visible rect
		if (node.nodeEditorPosition != newPosition) {
			UpdateVisibleRectangle ();
		}

		//save the new position back to the node
		node.nodeEditorPosition = newPosition;
	}

	//draw the nodal window
	private void DrawNodeWindow(int index) {
		//get the node from the id (also index)
		DialogNode node = tree.nodes[index];

		//now start drawing controls for the window

		//first off show a set of essential controls at the top
		EditorGUILayout.BeginHorizontal();

		//if there is a link being made then provide a link to button
		//let a node link to itself
		if (NodeExists (startLinkNode) && GUILayout.Button("Link to")) {
			//check if the node being linked from has responses
			DialogNode linkStart = tree.nodes[startLinkNode];

			if (linkStart.responses.Count > 0) {
				//check if the response index is in bounds and if so set it
				if (linkResponseIndex >= 0 && linkResponseIndex < linkStart.responses.Count) {
					linkStart.responses [linkResponseIndex].linkNodeIndex = index;
				}
			} else {
				//set the index
				linkStart.nodeLink = index;
			}

			//clear the current selection
			startLinkNode = -1;
			linkResponseIndex = -1;
		}

		//button to select the node for editing
		if (GUILayout.Button ("Edit")) {
			SelectNode (index);
		}

		//if node is root show so, if not display button to make it root
		if (index == tree.rootNodeIndex) {
			EditorGUILayout.LabelField ("Root Node");
		} else {
			if (GUILayout.Button("Make Root")) {
				tree.rootNodeIndex = index;
			}
		}

		//button to delete the node
		if (GUILayout.Button ("X")) {
			DeleteNode (index);
		}

		EditorGUILayout.EndHorizontal ();

		//put node preview content and response buttons side by side
		EditorGUILayout.BeginHorizontal();

		//text content
		EditorGUILayout.BeginVertical ();
		//if speaker is non null provide a speaker name
		if (node.speakerName != "") {
			EditorGUILayout.LabelField (node.speakerName + ":");
		}
		//EditorGUILayout.LabelField (node.nodeText);
		GUILayout.Label(node.nodeText, GUILayout.MaxWidth(300), GUILayout.MaxHeight(250));

		EditorGUILayout.EndVertical ();

		//responses
		EditorGUILayout.BeginVertical ();

		//if theres no responses then display a button to link to a node
		if (node.responses.Count == 0 && GUILayout.Button ("Link Node")) {
			//if this node has a link then break it
			node.nodeLink = -1;
			
			//if this node is already being with this will stop the link instead
			if (startLinkNode == index) {
				startLinkNode = -1;
				linkResponseIndex = -1;
			} else {
				startLinkNode = index;
				linkResponseIndex = -1;
			}
		}

		//if there are responses then display an option to delete all of them along with a link button
		for (int i = 0; i < node.responses.Count; i++) {
			DrawResponse (node, index, i);
		}

		//button to add a response, comes after all current responses
		if (GUILayout.Button ("Add Response")) {
			node.responses.Add (new DialogResponse ());
			node.responses[node.responses.Count -1].responseLabel = "Response";

			//the moment a response is added the link node is irrelevant, set it to null
			node.nodeLink = -1;
		}

		EditorGUILayout.EndVertical ();

		EditorGUILayout.EndHorizontal ();

		//enable drag for node 
		GUI.DragWindow ();
	}

	//method to draw response with on node window
	private void DrawResponse(DialogNode responseHolder, int nodeIndex, int responseIndex) {
		//while there aren't enough responses in the map node make another
		while (responseIndex > mapNodes [nodeIndex].responseRects.Count - 1) {
			mapNodes [nodeIndex].responseRects.Add (new Rect ());
		}

		mapNodes [nodeIndex].responseRects[responseIndex] = EditorGUILayout.BeginHorizontal();

		//draw a button to delete the response with
		if (GUILayout.Button ("X")) {
			responseHolder.responses.RemoveAt (responseIndex);
			
					//if any responses on this node are selected for linking cancel it
			if (startLinkNode == nodeIndex) {
						startLinkNode = -1;
						linkResponseIndex = -1;
			}
			//return before trying to draw the rest
			EditorGUILayout.EndHorizontal ();
			return;
		}
		//draw a button to start a nodal link with
		if (GUILayout.Button (responseHolder.responses [responseIndex].responseLabel)) {
			//nullify any current link on the response
			responseHolder.responses [responseIndex].linkNodeIndex = -1;

			//if this response is already selected this will break it instead
			if (startLinkNode == nodeIndex && linkResponseIndex == responseIndex) {
						startLinkNode = -1;
						linkResponseIndex = -1;
			} else {
				startLinkNode = nodeIndex;
				linkResponseIndex = responseIndex;
			}
			
		}

		EditorGUILayout.EndHorizontal ();

	}

	//draw the node curves
	private void DrawNodeCurves(DialogNode node, int index) {
		//get the map node to see where to draw the lines 
		//map nodes may be a frame behind so make sure to check validity before using
		if (index >= 0 && index < mapNodes.Count) {
			MapNode map = mapNodes [index];
			
			//if the node has no responses and a valid link then draw a line to the linked node
			if (node.responses.Count == 0 && NodeExists (node.nodeLink)) {
				if (node.nodeLink >= 0 && node.nodeLink < mapNodes.Count) {
					MapNode toMap = mapNodes [node.nodeLink];

					//if the node is linked to itself then place the start of the link at the right of the window so the link is visible
					if (node.nodeLink == index) {
						DrawCurve (new Vector2(map.nodeRect.xMax, map.nodeRect.yMin + 10), toMap.nodeRect.center);
					} else {
						DrawCurve (map.nodeRect.center, toMap.nodeRect.center);
					}
				}
			} else if (node.responses.Count > 0) {
				//draw response links
				for (int i = 0; i < node.responses.Count; i++) {
					int linkNodeIndex = node.responses [i].linkNodeIndex;

					//check if the node being linked to and the response index exist in the mapnodes
					/*if (NodeExists (linkNodeIndex) && linkNodeIndex < mapNodes.Count && i < map.responseRects.Count) {
						MapNode toMap = mapNodes [linkNodeIndex];
						//cached the vector2
						Vector2 start = new Vector2();
						start.x = map.responseRects [i].xMax;
						start.y = map.responseRects [i].center.y;

						DrawCurve (start, toMap.nodeRect.center);
					}*/

					if (NodeExists (linkNodeIndex) && linkNodeIndex < mapNodes.Count) {
						MapNode toMap = mapNodes [linkNodeIndex];
						//cached the vector2
						Vector2 start = new Vector2(map.nodeRect.xMax, map.nodeRect.yMin);
						//now add a flat amount to the y plus a fixed amount for each response in the index
						start.y += 51;
						start.y += 21 * i;
						//start.x = map.responseRects [i].xMax;
						//start.y = map.responseRects [i].center.y;

						DrawCurve (start, toMap.nodeRect.center);
					}
				}
			}
		}
	}

	//to draw a curve between 2 points
	private void DrawCurve(Vector2 start, Vector2 end) {
		Vector2 startTan = start + Vector2.right * 50;
		Vector2 endTan = end + Vector2.left * 50;

		Handles.DrawBezier (start, end, startTan, endTan, Color.black, null, 3);
	}

	//draw the editor panel used to edit a single node (some variables concern the link between nodes, these are edited in the nodal map not in this panel)
	private void DrawEditorPanel() {
		//button to go back to the nodal map
		if (GUILayout.Button ("Dialog Tree")) {
			nodeEditView = false;
		}

		//check that there is a selection
		if (NodeExists(selectedNode)) {
			//get the selected node
			DialogNode selected = tree.nodes[selectedNode];

			//start a vertical group
			EditorGUILayout.BeginVertical ();
			//place editor in scroll group in case content is too large for the screen
			editorScrollPos = EditorGUILayout.BeginScrollView (editorScrollPos);

			//show if this is the root node
			if (tree.rootNodeIndex == selectedNode) {
				EditorGUILayout.LabelField ("Root Node");
			}
			EditorGUILayout.LabelField ("Index: #" + selectedNode.ToString ());

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

			//if there are no responses on this node and the link is valid then display the node link value
			if (selected.responses.Count == 0 && NodeExists (selected.nodeLink)) {
				//show node link value with button for linking to it
				//dont allow the link to be changed here, but display it and allow the user to selected the linked node if non null
				EditorGUILayout.BeginHorizontal ();

				EditorGUILayout.LabelField("Next Node is #" + selected.nodeLink);
				if (GUILayout.Button ("Follow Link")) {
					SelectNode (selected.nodeLink);
				}

				EditorGUILayout.EndHorizontal ();

			} 

			//buttons to create a dialog response
			if (GUILayout.Button("Add Response")) {
				selected.responses.Add(new DialogResponse());
			}

			if (selected.responses.Count > 0) {
				//scrollview with a list of all the responses, and ways to edit / delete them
				responseScrollPos = EditorGUILayout.BeginScrollView (responseScrollPos);
				EditorGUILayout.BeginHorizontal ();
				//draw responses
				for (int i = 0; i < selected.responses.Count; i++) {
					DrawResponse (selected, selected.responses [i], i);
				}

				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.EndScrollView ();
			}

			EditorGUILayout.EndScrollView ();

			EditorGUILayout.EndVertical ();
		} else {
			EditorGUILayout.LabelField ("No node selected");
		}
	}

	//method to draw the given response (can be of a few different types)
	private void DrawResponse(DialogNode selected, DialogResponse response, int index) {
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

		//if the link is valid then show the link and provide a button to follow it
		if (NodeExists (response.linkNodeIndex)) {
			//show node link value with button for linking to it
			//dont allow the link to be changed here, but display it and allow the user to selected the linked node if non null
			EditorGUILayout.BeginHorizontal ();

			EditorGUILayout.LabelField("Linked Node is #" + response.linkNodeIndex);
			if (GUILayout.Button ("Follow Link")) {
				SelectNode (response.linkNodeIndex);
			}

			EditorGUILayout.EndHorizontal ();

		} 

		response.responseLabel = EditorGUILayout.TextField ("Response Label:", response.responseLabel);

		response.responseFlag = EditorGUILayout.IntField ("Response Flag:", response.responseFlag);

		EditorGUILayout.EndVertical ();
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
}

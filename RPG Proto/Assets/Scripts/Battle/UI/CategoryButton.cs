using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryButton : MonoBehaviour {
	//the button ui component
	private Button button;
	public Button Button {
		get {
			return button;
		}
	}

	//the text to display
	private Text text;
	//the cached category
	private MoveListCategory category = null;
	public MoveListCategory Category {
		get {
			return category;
		}
	}

	// Use this for initialization
	void Awake () {
		button = GetComponent<Button> ();
		text = GetComponentInChildren<Text> ();
	}

	//set the category of the button
	public void SetCategory(MoveListCategory setCategory) {
		category = setCategory;

		//update the text if the category
		if (category != null) {
			text.text = category.categoryName;
		}
	}
}

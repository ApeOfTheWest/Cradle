using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextHolder : MonoBehaviour {
	//the dialog ui, notify it when the text has finished
	public DialogUI dialogUI;

	//the text to display, will be displayed in chunks if too big
	private string displayText;

	//the current starting position in the text
	private int textCharStart;
	//the current ending position in the text
	private int textCharEnd;
	//whether the text scrawl is finished or not for the current chunk of text
	//if the skip button is pressed when this is false the current chunk will be displayed
	//if pressed when this is true the next chunk will be displayed (or if finished responses / next node will be displayed)
	private bool chunkFinished = false;
	//property to update graphic when chunk is finished
	private bool ChunkFinished {
		get {
			return chunkFinished;
		}
		set {
			chunkFinished = value;
			textPrompter.SetActive (chunkFinished);
		}
	}
	//the time until displaying the next character, used to ensure a consisten speed between frames
	private float timeUntilLetter = 0;

	//the current chunk of text to display
	private string currentChunk="";
	//the index the current chunk ends at
	private int chunkEndIndex;

	//the current piece of text being displayed (displayed while typing)
	private string currentText;

	[SerializeField]
	private Text textBox;
	[SerializeField]
	private GameObject textPrompter;

	//the noise to play on typing a character
	[SerializeField]
	private AudioClip typeNoise;
	//the audiosource to play it with
	private AudioSource audioPlayer;

	void Awake() {
		textPrompter.SetActive (false);
		//get the audi player
		audioPlayer = GetComponent<AudioSource>();
		//set to play the typing noise
		audioPlayer.clip = typeNoise;
		//set to loop it while playing
		audioPlayer.loop = true;
	}

	//set the text to display
	public void SetText(string text) {
		displayText = text;
		 
		//initialise the ending of the text chunk to 0
		chunkEndIndex = 0;

		GetNextTextChunk ();
	}

	//method to get the next text chunk ready for displaying
	public void GetNextTextChunk() {
		//textgenerator to use to predict how much text can fit in the textbox
		TextGenerator testGen = new TextGenerator ();
		//get a text generator with the same settings as the text box
		TextGenerationSettings testSetting = textBox.GetGenerationSettings(new Vector2(textBox.rectTransform.rect.width, textBox.rectTransform.rect.height));

		//set the textbox to the full remaining text to start with
		testGen.Populate(displayText.Substring(chunkEndIndex, displayText.Length - chunkEndIndex), testSetting);
		//then find the number of visible characters to find the length that the 1st textchunk should be
		int chunkLength = testGen.characterCountVisible;
		//a quirk of the visible count property is that it will return 1 less than the real number if the text doesn't fit in, adjust for this
		if (chunkEndIndex + chunkLength < displayText.Length - 1) {
			chunkLength++;
		}
		//if length is -1 set to 0
		if (chunkLength < 0) {
			chunkLength = 0;
		}

		currentChunk = displayText.Substring(chunkEndIndex, chunkLength);

		//incriment end index by the length of the next chunk
		chunkEndIndex += chunkLength;

		//clear the current text
		textBox.text = "";
		currentText = "";

		//set start and end to 0
		textCharStart = 0;
		textCharEnd = 0;
		//reset time till next letter
		timeUntilLetter = 0f;
		//chunk finished is false
		ChunkFinished = false;
	}

	//return true if text is finished scrolling after this update
	public bool UpdateText(float speed) {
		//use the speed, delta time, and cooldown to get the number of letters that can be typed
		//use unscaled time in case game is paused
		timeUntilLetter -= speed * Time.unscaledDeltaTime;

		int newLetters = 0;
		while (timeUntilLetter < 0) {
			newLetters++;
			timeUntilLetter += 1f;
		}

		textCharEnd += newLetters;
		//ensure this doesn't go over the max
		if (textCharEnd > currentChunk.Length) {
			textCharEnd = currentChunk.Length;
			//in case chunk length was 0 set the textcharend to 0 if neg
			if (textCharEnd < 0) {
				textCharEnd = 0;
			}
		}

		//append the new letters onto the textbox
		currentText += currentChunk.Substring(textCharStart, textCharEnd - textCharStart);
		textBox.text = currentText;

		//incriment the new start for the next cycle
		textCharStart = textCharEnd;


		//at the end of updating the text return chunk finished if the chunk of text is done
		if (textCharEnd >= currentChunk.Length) {
			//dont stop audio, just stop it looping
			audioPlayer.loop = false;

			//the current chunk has finished
			if (chunkEndIndex >= displayText.Length) {
				//if this chunk was the last chunk then set text finished to true
				//and chunk finished to false so the arrow doesn't show
				return true;
			} else {
				//otherwise set chunk finished to true, indicating text can be scrolled
				ChunkFinished = true;
			}
		} else {
			//if new letters is greater than 1 start text noise over from beggining
			//only play if not already playing
			if (newLetters > 0 && audioPlayer.isPlaying == false) {
				audioPlayer.Play ();
				audioPlayer.loop = true;
			}
		}
		return false;
	}

	//call when skip button is pressed
	public void Skip() {
		if (ChunkFinished) {
			//if the chunk is finished skip to the next chunk
			GetNextTextChunk();
		} else {
			//otherwise finish the chunk, and wait for skip command next frame
			textBox.text = currentChunk;
			textCharEnd = currentChunk.Length - 1;
			//dont stop audio, just stop it looping
			audioPlayer.loop = false;

			//only set chunk finished to true if this chunk is not the last
			if (chunkEndIndex < displayText.Length) {
				ChunkFinished = true;
			}
		}
	}
}

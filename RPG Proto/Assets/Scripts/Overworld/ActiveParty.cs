using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//active party stores the currently active units to be used, along with the selected reserves
public class ActiveParty {
	//the currently active party memebers (max of 3)
	private List<PartyUnit> activeParty;

	//the current reserve of party members (max of 3)
	//no members can be put in reserve until the active slots are filled
	private List<PartyUnit> reserveParty;

	//the positions that party members can be in on the field (normalized)
	private Vector2[] partyPositions;

	//deadzone from top of battle scenery's field, must make up for height of tallest character to avoid their head being out of view
	const float TOP_FIELD_DEADZONE = 0.1f;
	//deadzone from bottom of scenery's field
	const float BOT_FIELD_DEADZONE = 0.1f;
	//vertical space to work with
	const float VERT_FIELD_SPACE = 1f - TOP_FIELD_DEADZONE - BOT_FIELD_DEADZONE;
	//deadzone from left of scenery's field, party members should typically be in the right half of the field so backward dodges don't go out of view on narrow aspect ratios
	const float LEFT_FIELD_DEADZONE = 0.4f;
	//deadzone from right of scenery's field
	const float RIGHT_FIELD_DEADZONE = 0.0f;
	//horizontal space to work with
	const float HORIZ_FIELD_SPACE = 1f - LEFT_FIELD_DEADZONE - RIGHT_FIELD_DEADZONE;

	public const uint MAX_ACTIVE = 3;

	public const uint MAX_RESERVE = 3;

	public ActiveParty() {
		activeParty = new List<PartyUnit>();
		reserveParty = new List<PartyUnit>();

		//make the relevent positions to place each party member in
		partyPositions = new Vector2[MAX_ACTIVE];

		//divide the space given to the part members evenly, take off space for deadzones at the top and bottom of the availiable field space
		Vector2[] listOfPositions = new Vector2[MAX_ACTIVE];

		float vertSpacing = VERT_FIELD_SPACE / (float)MAX_ACTIVE;
		float horizSpacing = HORIZ_FIELD_SPACE / (float)MAX_ACTIVE;

		float currVert = BOT_FIELD_DEADZONE;
		float currHoriz = LEFT_FIELD_DEADZONE;

		for (int i = 0; i < MAX_ACTIVE; i++) {
			//generate even spaced positions
			listOfPositions[i] = new Vector2(currHoriz, currVert);

			currHoriz += horizSpacing;
			currVert += vertSpacing;
		}

		//set the party positions so that the first position is the most centralized
		//use different technique based on whether the position numbers are even or not
		if (MAX_ACTIVE % 2 > 0) {
			//number is uneven

			//start at half of the max minus 1 and go by index
			int midIndex = ((int)(MAX_ACTIVE) - 1) / 2;
			//first position is central
			partyPositions[0] = listOfPositions[midIndex];

			//start bool descending (going towards screen)
			bool descending = true;
			//start the distance from central position to be 1
			int distanceFromCentre = 1;

			//now seek out from the centre
			for (int i = 1; i < MAX_ACTIVE; i++) {
				int index = 0;

				if (descending == true) {
					index = midIndex - distanceFromCentre;
					descending = false;
				} else {
					index = midIndex + distanceFromCentre;
					descending = true;
					//seek further from centre after both sides have been checked
					distanceFromCentre++;
				}

				partyPositions[i] = listOfPositions[index];
			}

		} else {
			//number is even

			//start at half of max minus 1 ( starting on the closer side of the even split)
			int midIndex = ((int)(MAX_ACTIVE) - 1) / 2;
			partyPositions[0] = listOfPositions[midIndex];

			//start bool ascending (going away from screen)
			bool descending = false;
			//start the distance from central position to be 1
			int distanceFromCentre = 1;

			//now seek out from the centre
			for (int i = 1; i < MAX_ACTIVE; i++) {
				int index = 0;

				if (descending == true) {
					index = midIndex - distanceFromCentre;
					descending = false;
					//seek further from centre after both sides have been checked
					distanceFromCentre++;
				} else {
					index = midIndex + distanceFromCentre;
					descending = true;
				}

				partyPositions[i] = listOfPositions[index];
			}
		}

	}

	//add a new unit to the party
	public void AddPartyMember(PartyUnit newMember) {
		//add to the active party if theres a space
		if (activeParty.Count < MAX_ACTIVE) {
			activeParty.Add (newMember);
		} else {
			reserveParty.Add (newMember);
		}
	}

	//swap the position of units by index and activity
	//used extensively by the menu for formation switching
	//update the home position of the unit being swapped if they are active after being swapped
	public void SwapUnitPositions(int firstIndex, bool firstActive,
		int secondIndex, bool secondActive) {

	}

	public PartyUnit[] CollateActiveUnits() {
		//make an array the size of the active members
		PartyUnit[] units = new PartyUnit[activeParty.Count];

		for (int i = 0; i < activeParty.Count; i++) {
			units [i] = activeParty [i];

			//set each units position
			units[i].normHomePosition = new Vector2(partyPositions[i].x, partyPositions[i].y);
		}

		return units;
	}

	public PartyUnit[] CollateReserveUnits() {
		PartyUnit[] units = new PartyUnit[reserveParty.Count];

		for (int i = 0; i < reserveParty.Count; i++) {
			units [i] = reserveParty [i];
		}

		return units;
	}
}

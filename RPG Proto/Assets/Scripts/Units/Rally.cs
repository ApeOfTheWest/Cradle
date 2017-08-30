using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rally {
	//rally has a level
	private int rallyLevel = 0;
	public int RallyLevel {
		get {
			return rallyLevel;
		}
		set {
			rallyLevel = value;

			if (rallyLevel > maxLevel) {
				rallyLevel = maxLevel;
			} else if (rallyLevel < 0) {
				rallyLevel = 0;
			}
		}
	}

	//the amount for the level progress to fill up to
	public const int MAX_LEVEL_PROGRESS = 100;

	//and a percentage filled towards the next level (out of 100)
	//if the progress fills over then the rally level will be incrimented
	private int levelProgress = 0;
	public int LevelProgress {
		get {
			return levelProgress;
		}
		set {
			levelProgress = value;

			//while the level progress is greater than or equal to 100 convert it into points level points
			while (levelProgress >= MAX_LEVEL_PROGRESS) {
				//if the max level has been reached just cap the points at 100 and break
				if (RallyLevel >= maxLevel) {
					levelProgress = MAX_LEVEL_PROGRESS;
					break;
				} else {
					levelProgress -= MAX_LEVEL_PROGRESS;
					RallyLevel += 1;
				}
			}

			//progress cant be negative
			if (levelProgress < 0) {
				levelProgress = 0;
			}
		}
	}

	//the default max level to a rally meter
	public const int DEFAULT_MAX_LEVEL = 3;
	//and a max level (may be different for different teams, so dont make it constant
	private int maxLevel = DEFAULT_MAX_LEVEL;

	//reset the rally meter to its default state
	public void ResetToDefault() {
		rallyLevel = 0;
		levelProgress = 0;
		maxLevel = DEFAULT_MAX_LEVEL;
	}

	//change the rally level by the given amount
	public void ChangeRallyLevel(int amount) {
		RallyLevel += amount;
	}

	//change the rally progress by the given amount
	public void ChangeRallyProgress(int amount) {
		LevelProgress += amount;
	}
}

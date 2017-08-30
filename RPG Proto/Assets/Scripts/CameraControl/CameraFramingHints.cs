using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class takes a selection of camera trick hints and tries to work out the desired position and size of the camera frame
//it is designed for orthographic use but includes a z position for certain perspective zoom effects
//this class doesnt detect what camera hints to use, only how the use the gathered ones
[System.Serializable]
public class CameraFramingHints {
	//class to represent points of interest with
	[System.Serializable]
	public class CameraInterestPoint {
		public CameraInterestPoint(Vector2 pos, float weighting) {
			position = pos;
			this.weighting = weighting;
		}

		//the position of the point of interest (in 2d)
		public Vector2 position = new Vector2();
		//the weighting to give the point
		public float weighting = 1f;
	}

	//the default z position to aim for if no additional rendering hints desire a change in it
	public float defaultZ = 0f;

	//the default size of the camera, the camera may grow bigger to accomidate things that must be kept in frame but wont grow smaller
	public float defaultCameraSize = 5f;

	//rectangles signifying zones that should be kept in frame (such as a player, or important enemy)
	public List<Rect> keepInFrame = new List<Rect>();

	//points of interest combined with what weighting to give them, the higher the weighting the more the camera will swing towards the point
	//these points usually make up the player and other important events
	//the player will usually have a weighting of 1 so anything higher than that is more important and will demand more attention
	public List<CameraInterestPoint> interestPoints = new List<CameraInterestPoint>();
	//IMPORTANT: there should always be at least 1 point of interest or the camera will default to looking at point 0, 0

	//any object references to camera hints in the z direction


	//pass out the calculated target position and size of the camera, this can be interpolated towards using a camera controller
	//or just snapped towards
	//pass in the aspect ratio so that the orthogonal size can be worked out based on it
	public void CalculateTargetCameraConfig(float aspect, out float size, out Vector3 position) {
		size = 0f;
		position = new Vector3 ();

		//find the desired centre of the camera using only points of interest
		//arranged by weighting
		Vector2 interestPosition = new Vector2();

		//add all the positions multiplied by the weights together and divide by the weights
		float denominator = 0f;
		for (int i = 0; i < interestPoints.Count; i++) {
			interestPosition += interestPoints [i].position * interestPoints [i].weighting;
			denominator += interestPoints [i].weighting;
		}
		//check for no denoms
		if (denominator != 0) {
			interestPosition /= denominator;
		}

		//then apply constraints using the framing rectangles, do this by finding a single rectangle which encompasses all the other ones
		//leave a null rect if no framing rectangles are included
		Rect framingRect = new Rect();
		bool useFraming = false;
		if (keepInFrame.Count > 0) {
			useFraming = true;
			//make the rectangle equal to the first rectangle
			framingRect = GetOverallFrame();
		}

		//find the smallest size the camera can be to fit the frame, or the default size if not
		size = defaultCameraSize;

		//finally see if this can fit into the default size, if not upscale until it can
		if (useFraming == true) {
			//the lowest size the camera must be to fit the height of the frame in view
			float heightSizeRestrict = framingRect.height / 2f;
			//the lowest size the camera must be to fit the width of the frame in view (use aspect)
			float widthSizeRestrict = framingRect.width / aspect / 2f;

			//if either of these is higher than the default set it
			if (heightSizeRestrict > size) {
				size = heightSizeRestrict;
			}
			if (widthSizeRestrict > size) {
				size = widthSizeRestrict;
			}
		}

		//now place the position at the point of interest to start with
		position = interestPosition;
		//also add z component at this point
		//set z to default
		position.z = defaultZ;

		//if framing restrictions are in place then adjust away from the interest position until it is in frame
		if (useFraming == true) {
			//find compromise between the point of interest and the frame to ensure the point 
			//is as close as possible to the screen centre without cutting off the frame
			//rectangle

			//first get the height and width of the camera from the size (which is a half height)
			float cameraHalfHeight = size;
			float cameraHalfWidth = cameraHalfHeight * aspect;

			//check the left boundary is in frame, adjust if not until it is in frame
			//because the camera should have its size set to account for the framing one adjustment on each axis should fix it
			if (position.x - cameraHalfWidth > framingRect.xMin) {
				position.x = framingRect.xMin + cameraHalfWidth;
			} else if (position.x + cameraHalfWidth < framingRect.xMax) {
				//otherwise (mutually exclusive) check if right boundary is in frame
				position.x = framingRect.xMax - cameraHalfWidth;
			}

			//and same for verticals
			//bottom boundary
			if (position.y - cameraHalfHeight > framingRect.yMin) {
				position.y = framingRect.yMin + cameraHalfHeight;
			} else if (position.y + cameraHalfHeight < framingRect.yMax) {
				//top boundary
				position.y = framingRect.yMax - cameraHalfHeight;
			}
		}
		//as a bug catch set size to 1 if size is 0
		if (size <= 0f) {
			size = 1f;
		}
	}

	//get a rectangle equivalent to the outer boundaries of all current framing rectangles
	//recturns 0,0,0 rect if no current framing rects exisy
	public Rect GetOverallFrame() {
		if (keepInFrame.Count > 0) {
			Rect overall = new Rect(keepInFrame [0]);

			//and then expand it at its max and min as necessary
			for (int i = 1; i < keepInFrame.Count; i++) {
				if (overall.xMin > keepInFrame [i].xMin) {
					overall.xMin = keepInFrame [i].xMin;
				}
				if (overall.yMin > keepInFrame [i].yMin) {
					overall.yMin = keepInFrame [i].yMin;
				}
				if (overall.xMax < keepInFrame [i].xMax) {
					overall.xMax = keepInFrame [i].xMax;
				}
				if (overall.yMax < keepInFrame [i].yMax) {
					overall.yMax = keepInFrame [i].yMax;
				}
			}

			return overall;
		} else {
			return new Rect();
		}
	}

	//clears all the framing hints stored on the camera
	public void ClearHints() {
		keepInFrame.Clear ();
		interestPoints.Clear ();
	}
}

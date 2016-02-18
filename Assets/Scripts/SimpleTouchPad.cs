using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SimpleTouchPad : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {

	public float smoothing;

	private Vector2 origin;
    private Vector2 currentPosition;
	private Vector2 direction;
	private Vector2 smoothDirection;
	private bool touched;
	private int pointerID;

	void Awake() {
		// initialize 
		direction = Vector2.zero;
		touched = false;
	}

	public void OnPointerDown (PointerEventData data) {
		// Set touch start point
		if (!touched) {
			touched = true;
			pointerID = data.pointerId;
			origin = data.position;
		}
	}

	public void OnDrag (PointerEventData data) {
		// Compare the difference between our start point and current pointer position
		if (data.pointerId == pointerID) {
			currentPosition = data.position;
			Vector2 directionRaw = currentPosition - origin;
			direction = directionRaw.normalized;
        }
	}

	public void OnPointerUp (PointerEventData data) {
		// Reset everything
		if (data.pointerId == pointerID) {
			direction = Vector2.zero;
			touched = false;
		}
	}

	/*public Vector2 GetDirection() {
		smoothDirection = Vector2.MoveTowards(smoothDirection, direction, smoothing);
		return direction;
	}*/

    public Vector2 GetPosition()
    {
        /*Vector3 res = new Vector3(0, 0, 0);
        var originInWorl = Camera.main.ScreenToWorldPoint(new Vector3(origin.x, origin.y, 10.0f));

        if (originInWorl.x < fauconPositionInWorld.x + 1 && originInWorl.x > fauconPositionInWorld.x - 1 && originInWorl.z < fauconPositionInWorld.z + 1 && originInWorl.z > fauconPositionInWorld.z - 1)
            res = currentPosition;

        return res;*/
        return currentPosition;
    }

}
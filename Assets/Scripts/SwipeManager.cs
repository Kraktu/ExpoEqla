using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Swipe { 
    None, 
    Up, 
    Down, 
    Left, 
    Right 
};

public class SwipeManager : MonoBehaviour
{
    public float minSwipeLength = 200f;
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    public static Swipe swipeDirection;

    void Update()
    {
        DetectSwipe();
    }

    public void DetectSwipe()
    {

        if (Input.GetMouseButtonDown(0))
        {
            firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (Input.GetMouseButtonUp(0))
        {
            secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

            // Make sure it was a legit swipe, not a tap
            if (currentSwipe.magnitude < minSwipeLength)
            {
                swipeDirection = Swipe.None;
                return;
            }

            currentSwipe.Normalize();

            // Swipe up
            if (currentSwipe.y > 0 && (currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)) {
                swipeDirection = Swipe.Up;
            // Swipe down
            //Debug.Log("swipe up" + currentSwipe.x);
            } else if (currentSwipe.y < 0 && (currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)) {
                swipeDirection = Swipe.Down;
            // Swipe left
            //Debug.Log("swipe down"+currentSwipe.x);
            } else if (currentSwipe.x < 0 && (currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)) {
                swipeDirection = Swipe.Left;
            // Swipe right
            //Debug.Log("swipe left"+currentSwipe.y);
            } else if (currentSwipe.x > 0 && (currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)) {
                swipeDirection = Swipe.Right;
            //Debug.Log("swipe right"+currentSwipe.y);
            }

            DoAction();
        }
        else
        {
            swipeDirection = Swipe.None;
        }
    }

    public void DoAction()
	{
		switch (swipeDirection)
		{
			case Swipe.None:
				break;
			case Swipe.Up:
                SoundManager.Instance.ReplayLastVoice();
				break;
			case Swipe.Down:
                FlowManager.Instance.PlayPauseClip();
				break;
			case Swipe.Left:
                FlowManager.Instance.CallNextClip(-1);
				break;
			case Swipe.Right:
                FlowManager.Instance.CallNextClip(1);
				break;
			default:
				break;
		}
	}
}

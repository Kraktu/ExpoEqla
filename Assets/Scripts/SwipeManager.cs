using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Swipe { 
    None, 
    Up, 
    Down, 
    Left, 
    Right 
};

public class SwipeManager : MonoBehaviour
{
    [HideInInspector]
    public bool isDetectingControls;
    public float minSwipeLength = 200f;
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    // Double tap, et sécurité
    public float doubleTapDetectTimer=0.2f, resetDetectTimer=5f;
    public int nbrOfActionToReset=5;

    int consecutiveTapNbr = 0;
    int unlockingActionNbr=0;

    Coroutine resetTimerCoroutine,doubleTapDetectionCoroutine;

    //-----------------------------
    [HideInInspector]
    public static Swipe swipeDirection;

    static public SwipeManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if(isDetectingControls)
		{
            DetectSwipe();
		}
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
                DoAction();
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
                CancelReset();
                consecutiveTapNbr++;
                doubleTapDetectionCoroutine= StartCoroutine(CheckingForDoubleTapTiming());
                if(consecutiveTapNbr==2)
				{
                    FlowManager.Instance.PlayPauseClip();
                    consecutiveTapNbr = 0;
					StopCoroutine(doubleTapDetectionCoroutine);
				}
				break;
			case Swipe.Up:
                consecutiveTapNbr = 0;
                CancelReset();
                SoundManager.Instance.ReplayLastVoice();
				break;
			case Swipe.Down:
                consecutiveTapNbr = 0;
                if (unlockingActionNbr==0)
				{
                    resetTimerCoroutine= StartCoroutine(CheckingForResetTiming());
				}
                unlockingActionNbr++;
                if(unlockingActionNbr == nbrOfActionToReset)
				{
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				}
				break;
			case Swipe.Left:
                consecutiveTapNbr = 0;
                CancelReset();
                FlowManager.Instance.VerifyNextClip(-1);
				break;
			case Swipe.Right:
                consecutiveTapNbr = 0;
                CancelReset();
                FlowManager.Instance.VerifyNextClip(1);
				break;
			default:
				break;
		}
	}

    public IEnumerator CheckingForResetTiming()
	{
        float time = 0;
		while (time<=resetDetectTimer)
		{
            time += Time.deltaTime;
            yield return null;
		}
        unlockingActionNbr = 0;
	}

    void CancelReset()
	{
        unlockingActionNbr = 0;
        if(resetTimerCoroutine!=null)
		{
            StopCoroutine(resetTimerCoroutine);
		}
    }

    public IEnumerator CheckingForDoubleTapTiming()
    {
        float time = 0;
        while (time <= doubleTapDetectTimer)
        {
            time += Time.deltaTime;
            yield return null;
        }
        consecutiveTapNbr = 0;
    }
}

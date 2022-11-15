using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct OrderedAudioClips
{
    public int index;
    public string audioClipName;   
}

public class FlowManager : MonoBehaviour
{
    public List<OrderedAudioClips> voicesToPlay = new List<OrderedAudioClips>();

    int CurrentVoiceClipIndex = 0;
    bool isVoiceStopped;
    public float maxDelayBetweenActions=1;
    Coroutine securityCoroutine, ControlVoiceCoroutine;
    public Sprite introImage, expoImage, outroImage;
    public Image background;
    public Button buttonToWebsite, quitStartingScreenButton,quitEndingScreenButton;
    static public FlowManager Instance { get; private set; }

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
    // Start is called before the first frame update
    void Start()
    {
        securityCoroutine=StartCoroutine(SwipeSecuring(true));
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void VerifyNextClip(int _cursorMovement)
	{
        StopCoroutine(securityCoroutine);
     //   if (SoundManager.Instance.aSourceVoice.clip.name=="transition"|| SoundManager.Instance.aSourceVoice.clip==null)
	    //{
            CallNextClip(_cursorMovement);
            securityCoroutine= StartCoroutine(SwipeSecuring(false));

     //   }
  //      else
		//{
  //          SoundManager.Instance.PlayVoiceEffect("transition");
  //          securityCoroutine = StartCoroutine(SwipeSecuring(true));

  //      }
		    
	}

    public void CallNextClip(int _cursorMovement)
	{
        int _TargetIndex = CurrentVoiceClipIndex + _cursorMovement;
        string _voiceToPlay = "";
		for (int i = 0; i < voicesToPlay.Count; i++)
		{
            if(_TargetIndex == voicesToPlay[i].index)
			{
                _voiceToPlay = voicesToPlay[i].audioClipName;
                CurrentVoiceClipIndex = _TargetIndex;
                isVoiceStopped = false;
                break;
			}
		}
        if(_voiceToPlay=="")
		{
            SoundManager.Instance.PlaySoundEffect("bonk");
		}
		else
		{
            SoundManager.Instance.PlayVoiceEffect(_voiceToPlay);
		}
        CheckForImage();
	}
    public void CheckForImage()
	{
        if(CurrentVoiceClipIndex==(voicesToPlay.Count))
		{
            background.sprite = outroImage;
            buttonToWebsite.gameObject.SetActive(true);
            quitStartingScreenButton.gameObject.SetActive(false);
            quitEndingScreenButton.gameObject.SetActive(true);

        }
        else if(CurrentVoiceClipIndex == 0)
		{
            background.sprite = introImage;
            buttonToWebsite.gameObject.SetActive(false);
            quitStartingScreenButton.gameObject.SetActive(true);
            quitEndingScreenButton.gameObject.SetActive(false);
        }
		else
		{
            background.sprite = expoImage;
            buttonToWebsite.gameObject.SetActive(false);
            quitStartingScreenButton.gameObject.SetActive(false);
            quitEndingScreenButton.gameObject.SetActive(false);
        }

	}

    IEnumerator SwipeSecuring(bool _isTransition)
	{
        float _time = 0;
        SwipeManager.Instance.isDetectingControls = false;
		while (_time<maxDelayBetweenActions)
		{
            _time += Time.deltaTime;
            yield return null;
		}
        SwipeManager.Instance.isDetectingControls = true;
		//if (!_isTransition)
		//{
  //          ControlVoiceCoroutine= StartCoroutine(ControlingCurrentVoice());
		//}
	}
    IEnumerator ControlingCurrentVoice()
	{
		while (SoundManager.Instance.aSourceVoice.time!=0)
		{
            yield return null;
		}
        VerifyNextClip(1);
	}

    public void PlayPauseClip()
	{
		if (SoundManager.Instance.aSourceVoice.clip==null||(SoundManager.Instance.aSourceVoice.time==0&&isVoiceStopped==false))
		{
            SoundManager.Instance.PlaySoundEffect("bonk");
		}
        else if(isVoiceStopped)
		{
            SoundManager.Instance.ContinueVoice();
            isVoiceStopped = false;
		}
		else
		{
            SoundManager.Instance.StopVoice();
            isVoiceStopped = true;
		}
	}

    public void GoToWebsite(string _url)
	{
        Application.OpenURL(_url);
	}
    public void QuitAppli()
	{
        Application.Quit();
	}
}

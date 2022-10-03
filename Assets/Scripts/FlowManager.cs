using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    static public FlowManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void VerifyNextClip(int _cursorMovement)
	{
        StopCoroutine(securityCoroutine);
        if (SoundManager.Instance.aSourceVoice.clip.name=="transition"|| SoundManager.Instance.aSourceVoice.clip==null)
	    {
            CallNextClip(_cursorMovement);
            securityCoroutine= StartCoroutine(SwipeSecuring(false));

        }
        else
		{
            SoundManager.Instance.PlayVoiceEffect("transition");
            securityCoroutine = StartCoroutine(SwipeSecuring(true));

        }
		    
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
	}

    IEnumerator SwipeSecuring(bool _isTransition)
	{
        float _time = 0;
        SwipeManager.Instance.isDetectingSwipe = false;
		while (_time<maxDelayBetweenActions)
		{
            _time += Time.deltaTime;
            yield return null;
		}
        SwipeManager.Instance.isDetectingSwipe = true;
		if (!_isTransition)
		{
            ControlVoiceCoroutine= StartCoroutine(ControlingCurrentVoice());
		}
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
		if (SoundManager.Instance.aSourceVoice.clip==null||SoundManager.Instance.aSourceVoice.time==0)
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

}

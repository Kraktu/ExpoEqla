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

    public void CallNextClip(int _cursorMovement)
	{
        int _TargetIndex = CurrentVoiceClipIndex + _cursorMovement;
        string _voiceToPlay = "";
		for (int i = 0; i < voicesToPlay.Count; i++)
		{
            if(_TargetIndex == voicesToPlay[i].index)
			{
                _voiceToPlay = voicesToPlay[i].audioClipName;
                break;
			}
		}
        if(_voiceToPlay=="")
		{
            // Retour au menu principal
		}
        SoundManager.Instance.PlayVoiceEffect(_voiceToPlay);
	}


}

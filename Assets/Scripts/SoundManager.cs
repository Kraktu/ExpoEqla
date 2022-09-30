using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
	[Header("AudioSources")]
	public AudioSource aSourceSFX, aSourceMusic, aSourceMicrophone, aSourceVoice, aSourceAmbiant;
	float _originalpitch = 1f;
	public float sfxVolume, initialMusicVolume;
	static public SoundManager Instance { get; private set; }
	public List<AudioClipStruct> _soundEffects = new List<AudioClipStruct>();
	[Header("Listof sound effect clips")]
	Dictionary<string, AudioClip> _soundEffectsDict = new Dictionary<string, AudioClip>();
	private string _lastPlayedVoice;
	float _currentVoiceClipTime;

	[System.Serializable]
	public struct AudioClipStruct
	{
		public string name;
		public AudioClip clip;
	}
	private void Update()
	{
		//_aSourceSFX.outputAudioMixerGroup.audioMixer.SetFloat("SFXCompleteSFXVolume", sfxVolume);
	}
	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
		GenerateSoundEffectDict();

	}
	#region Changement de musique de fond en fonction de la scene

	//private void OnEnable()
	//{
	//	SceneManager.sceneLoaded += StartBasicSceneMusic;
	//}
	//void StartBasicSceneMusic(Scene scene, LoadSceneMode loadSceneMode)
	//{
	//	switch (scene.buildIndex)
	//	{
	//		case 0:
	//			// Music du main menu si doit démarrer de suite
	//			break;
	//		case 1:
	//			//
	//			break;
	//		case 2:
	//			//
	//			break;
	//		default:
	//			//
	//			break;
	//	}
	//}


	#endregion

	void GenerateSoundEffectDict()
	{
		foreach (AudioClipStruct audioClip in _soundEffects)
		{
			_soundEffectsDict.Add(audioClip.name, audioClip.clip);
		}
	}
	public void StopAmbiant()
	{
		aSourceAmbiant.Stop();
	}
	public void PlayAmbiantSound(string clipName, float pitch = 1, float volume = 0)
	{
		aSourceAmbiant.outputAudioMixerGroup.audioMixer.SetFloat("AmbiantSFXPitch", pitch);
		aSourceAmbiant.outputAudioMixerGroup.audioMixer.SetFloat("AmbiantSFXVolume", volume);
		aSourceAmbiant.PlayOneShot(_soundEffectsDict[clipName]);
	}
	public void PlaySoundEffect(string clipName, float pitch = 1, float volume = 0)
	{
		aSourceSFX.outputAudioMixerGroup.audioMixer.SetFloat("SFXCompleteSFXPitch", pitch);
		aSourceSFX.outputAudioMixerGroup.audioMixer.SetFloat("SFXCompleteSFXVolume", volume);
		aSourceSFX.PlayOneShot(_soundEffectsDict[clipName]);
	}
	// Original function
	//public void PlayVoiceEffect(string clipName, float pitch = 1, float volume = 0)
	//{
	//	_aSourceVoice.outputAudioMixerGroup.audioMixer.SetFloat("VoicesSFXPitch", pitch);
	//	_aSourceVoice.outputAudioMixerGroup.audioMixer.SetFloat("VoicesSFXVolume", volume);
	//	_aSourceVoice.Stop();
	//	_lastPlayedVoice = clipName;
	//	_aSourceVoice.PlayOneShot(_soundEffectsDict[clipName]);
	//}

	public void PlayVoiceEffect(string clipName, float pitch = 1, float volume = 0)
	{
		aSourceVoice.outputAudioMixerGroup.audioMixer.SetFloat("VoicesSFXPitch", pitch);
		aSourceVoice.outputAudioMixerGroup.audioMixer.SetFloat("VoicesSFXVolume", volume);
		aSourceVoice.Stop();
		_lastPlayedVoice = clipName;
		aSourceVoice.clip = _soundEffectsDict[clipName];
		aSourceVoice.Play();
	}
	//Original Function
	//public void ReplayLastVoice()
	//{
	//	_aSourceVoice.Stop();
	//	_aSourceVoice.PlayOneShot(_soundEffectsDict[_lastPlayedVoice]);
	//}
	public void ReplayLastVoice()
	{
		
		if(aSourceVoice.clip==null){
			PlaySoundEffect("bonk");
		}
		else
		{
			aSourceVoice.Stop();
			aSourceVoice.time = 0;
			aSourceVoice.Play();
		}
	}
	// Nouvelle function
	public void StopVoice()
	{
		_currentVoiceClipTime = aSourceVoice.time;
		Debug.Log(_currentVoiceClipTime);
		aSourceVoice.Stop();
	}
	public void ContinueVoice()
	{
		aSourceVoice.Play();
		aSourceVoice.time = _currentVoiceClipTime;
	}
	public void StopMusic()
	{
		aSourceMusic.Stop();
	}
	public void ChangeMusicPitch(float pitch)
	{
		aSourceMusic.outputAudioMixerGroup.audioMixer.SetFloat("MusicCompleteMusicPitch", pitch);
		_originalpitch = pitch;
	}
	public void ChangeMusic(string clipName, float pitch = 1, float volume = 0)
	{
		aSourceMusic.outputAudioMixerGroup.audioMixer.SetFloat("MusicCompleteMusicPitch", pitch);
		aSourceMusic.outputAudioMixerGroup.audioMixer.SetFloat("MusicCompleteMusicVolume", volume);
		aSourceMusic.clip = _soundEffectsDict[clipName];
		aSourceMusic.Play();
	}
	public void ChangeMusicVolume(float volume)
	{
		aSourceMusic.outputAudioMixerGroup.audioMixer.SetFloat("MusicCompleteMusicVolume", volume);
	}
	public void lowerMusicPitch(float duration)
	{
		StartCoroutine(LowerMusicPitchCoroutine(duration));
	}
	IEnumerator LowerMusicPitchCoroutine(float duration)
	{
		float time = 0;

		while (time <= duration)
		{
			float tRatio = time / duration;
			aSourceMusic.outputAudioMixerGroup.audioMixer.SetFloat("MusicCompleteMusicPitch", Mathf.Lerp(_originalpitch, 0, tRatio));
			time += Time.deltaTime;
			yield return null;
		}
		aSourceMusic.outputAudioMixerGroup.audioMixer.SetFloat("MusicCompleteMusicPitch", 0);
	}
	public void ReturnAtOriginalMusicPitch()
	{
		aSourceMusic.outputAudioMixerGroup.audioMixer.SetFloat("MusicCompleteMusicPitch", _originalpitch);
	}
	public void ApplyReverbToMusic(float dryLevel = 0, float room = -10000, float roomHF = 0)
	{
		aSourceMusic.outputAudioMixerGroup.audioMixer.SetFloat("MusicMasterReverbDryLevel", dryLevel);
		aSourceMusic.outputAudioMixerGroup.audioMixer.SetFloat("MusicMasterReverbRoom", room);
		aSourceMusic.outputAudioMixerGroup.audioMixer.SetFloat("MusicMasterReverbRoomHF", roomHF);
	}
	public void SetVoiceVolume(float volume)
	{
		aSourceVoice.volume += volume;
	}
	public void SetSFXVolume(float volume)
	{
		aSourceSFX.volume += volume;
	}
}
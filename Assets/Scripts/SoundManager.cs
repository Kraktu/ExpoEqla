using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
	[Header("AudioSources")]
	public AudioSource _aSourceSFX, _aSourceMusic, _aSourceMicrophone, _aSourceVoice, _aSourceAmbiant;
	float _originalpitch = 1f;
	public float sfxVolume, initialMusicVolume;
	static public SoundManager Instance { get; private set; }
	public List<AudioClipStruct> _soundEffects = new List<AudioClipStruct>();
	[Header("Listof sound effect clips")]
	Dictionary<string, AudioClip> _soundEffectsDict = new Dictionary<string, AudioClip>();
	private string _lastPlayedVoice;
	float currentVoiceClipTime;

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
		_aSourceAmbiant.Stop();
	}
	public void PlayAmbiantSound(string clipName, float pitch = 1, float volume = 0)
	{
		_aSourceAmbiant.outputAudioMixerGroup.audioMixer.SetFloat("AmbiantSFXPitch", pitch);
		_aSourceAmbiant.outputAudioMixerGroup.audioMixer.SetFloat("AmbiantSFXVolume", volume);
		_aSourceAmbiant.PlayOneShot(_soundEffectsDict[clipName]);
	}
	public void PlaySoundEffect(string clipName, float pitch = 1, float volume = 0)
	{
		_aSourceSFX.outputAudioMixerGroup.audioMixer.SetFloat("SFXCompleteSFXPitch", pitch);
		_aSourceSFX.outputAudioMixerGroup.audioMixer.SetFloat("SFXCompleteSFXVolume", volume);
		_aSourceSFX.PlayOneShot(_soundEffectsDict[clipName]);
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
		_aSourceVoice.outputAudioMixerGroup.audioMixer.SetFloat("VoicesSFXPitch", pitch);
		_aSourceVoice.outputAudioMixerGroup.audioMixer.SetFloat("VoicesSFXVolume", volume);
		_aSourceVoice.Stop();
		_lastPlayedVoice = clipName;
		_aSourceVoice.clip = _soundEffectsDict[clipName];
		_aSourceVoice.Play();
	}
	//Original Function
	//public void ReplayLastVoice()
	//{
	//	_aSourceVoice.Stop();
	//	_aSourceVoice.PlayOneShot(_soundEffectsDict[_lastPlayedVoice]);
	//}
	public void ReplayLastVoice()
	{
		_aSourceVoice.Stop();
		_aSourceVoice.time = 0;
		_aSourceVoice.Play();
	}
	// Nouvelle function
	public void StopVoice()
	{
		currentVoiceClipTime = _aSourceVoice.time;
		Debug.Log(currentVoiceClipTime);
		_aSourceVoice.Stop();
	}
	public void ContinueVoice()
	{
		_aSourceVoice.Play();
		_aSourceVoice.time = currentVoiceClipTime;
	}
	public void StopMusic()
	{
		_aSourceMusic.Stop();
	}
	public void ChangeMusicPitch(float pitch)
	{
		_aSourceMusic.outputAudioMixerGroup.audioMixer.SetFloat("MusicCompleteMusicPitch", pitch);
		_originalpitch = pitch;
	}
	public void ChangeMusic(string clipName, float pitch = 1, float volume = 0)
	{
		_aSourceMusic.outputAudioMixerGroup.audioMixer.SetFloat("MusicCompleteMusicPitch", pitch);
		_aSourceMusic.outputAudioMixerGroup.audioMixer.SetFloat("MusicCompleteMusicVolume", volume);
		_aSourceMusic.clip = _soundEffectsDict[clipName];
		_aSourceMusic.Play();
	}
	public void ChangeMusicVolume(float volume)
	{
		_aSourceMusic.outputAudioMixerGroup.audioMixer.SetFloat("MusicCompleteMusicVolume", volume);
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
			_aSourceMusic.outputAudioMixerGroup.audioMixer.SetFloat("MusicCompleteMusicPitch", Mathf.Lerp(_originalpitch, 0, tRatio));
			time += Time.deltaTime;
			yield return null;
		}
		_aSourceMusic.outputAudioMixerGroup.audioMixer.SetFloat("MusicCompleteMusicPitch", 0);
	}
	public void ReturnAtOriginalMusicPitch()
	{
		_aSourceMusic.outputAudioMixerGroup.audioMixer.SetFloat("MusicCompleteMusicPitch", _originalpitch);
	}
	public void ApplyReverbToMusic(float dryLevel = 0, float room = -10000, float roomHF = 0)
	{
		_aSourceMusic.outputAudioMixerGroup.audioMixer.SetFloat("MusicMasterReverbDryLevel", dryLevel);
		_aSourceMusic.outputAudioMixerGroup.audioMixer.SetFloat("MusicMasterReverbRoom", room);
		_aSourceMusic.outputAudioMixerGroup.audioMixer.SetFloat("MusicMasterReverbRoomHF", roomHF);
	}
	public void SetVoiceVolume(float volume)
	{
		_aSourceVoice.volume += volume;
	}
	public void SetSFXVolume(float volume)
	{
		_aSourceSFX.volume += volume;
	}
	public IEnumerator ReadLetterFromString(string stringToRead)
	{
		for (int i = 0; i < stringToRead.Length; i++)
		{
			SoundManager.Instance.PlayVoiceEffect(stringToRead[i].ToString(), 0.95f, 10);
			yield return new WaitForSeconds(1);
		}
	}
}
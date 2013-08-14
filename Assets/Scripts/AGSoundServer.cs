using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BaseSoundFile
{
	protected AudioSource m_audioSource = null;
	public float m_Volume = 1.0f;
		
	public void Initialize (AudioSource audioSource)
	{
		m_audioSource = audioSource;
	}
	
	public virtual void play ()
	{
	}
	
	public virtual string getName ()
	{
		return "";
	}
	
	public virtual AudioClip getClip ()
	{
		return null;
	}
}

[System.Serializable]
public class SingleSoundFile : BaseSoundFile
{
	public AudioClip m_AudioClip = null;
	
	public override void play ()
	{
		m_audioSource.PlayOneShot (m_AudioClip, m_Volume);
//		Print.Log("Playing: " + m_AudioClip.name);
	}
	
	public override string getName ()
	{
		return m_AudioClip.name;
	}
	
	public override AudioClip getClip ()
	{
		return m_AudioClip;
	}
}

[System.Serializable]
public class RandomSoundFiles : BaseSoundFile
{
	public AudioClip[] m_AudioClips = null;
	private AudioClip m_currentClip = null;
	
	public void playRandom ()
	{
		var r = Mathf.FloorToInt (Random.value * m_AudioClips.Length);
		m_currentClip = m_AudioClips [r];
		m_audioSource.PlayOneShot (m_currentClip, m_Volume);
	}
	
	public override void play ()
	{
		playRandom ();
//		Print.Log ("Playing: " + m_currentClip.name);
	}
	
	public override string getName ()
	{
		return m_AudioClips[0].name.Substring(0,m_AudioClips[0].name.Length-1);
	}
}

public class AGSoundServer : MonoBehaviour {
	public AudioSource m_AudioSourcePrefab = null;
	public AudioListener m_AudioListenerPrefab = null;
	public string m_backgroundSound = "Ergo";
	protected AudioSource m_AudioSource = null;
	protected AudioSource m_AudioSourceBackground = null;
	protected AudioListener m_AudioListener = null;
	public SingleSoundFile[] m_singleSoundFiles = null;
	public RandomSoundFiles[] m_randomSoundFiles = null;
	protected List<BaseSoundFile> m_SoundFiles = null;

	public void Initialize ()
	{
		if (m_AudioSourcePrefab == null) {
//			Print.Warning ("m_AudioSourcePrefab is null");
			return;
		} else {
//			Print.Warning ("m_AudioSourcePrefab is here");
		}
		
		if (m_AudioListenerPrefab == null) {
//			Print.Warning ("m_AudioListenerPrefab is null");
			return;
		} else {
//			Print.Warning ("m_AudioListenerPrefab is here");
		}
		
		m_AudioSource = (AudioSource)Instantiate (m_AudioSourcePrefab);
		m_AudioListener = (AudioListener)Instantiate (m_AudioListenerPrefab);
		m_AudioListener.enabled = true;
		m_AudioSourceBackground = (AudioSource)Instantiate (m_AudioSourcePrefab);
		
		m_SoundFiles = new List<BaseSoundFile> (m_singleSoundFiles.Length + m_randomSoundFiles.Length);
		m_SoundFiles.AddRange (m_singleSoundFiles);
		m_SoundFiles.AddRange (m_randomSoundFiles);
		foreach (var sound in m_SoundFiles) {
			sound.Initialize (m_AudioSource);
		}
		
		foreach (var s in m_SoundFiles) {
			if (s.getName ().ToLower (). StartsWith (m_backgroundSound.ToLower ())) {
				m_AudioSourceBackground.clip = s.getClip( );
				m_AudioSourceBackground.volume = s.m_Volume;
				break;
			}
		}
		m_AudioSourceBackground.loop = true;
		m_AudioSourceBackground.Play( );
	}
	
	public void Play (string sound)
	{
//		Print.Log ("Playing: " + sound);
		find(sound);
	}
	
	protected void find (string sound)
	{
		foreach (var s in m_SoundFiles) {
//			Print.Log ("..." + s.getName ().ToLower () + " ... " + sound.ToLower ());
			if (s.getName ().ToLower (). StartsWith (sound.ToLower ())) {
//				Print.Log ("Playing: " + s.getName ());
				s.play();
				return;
			}
		}
	}
	
	public void Un_Mute(bool mute)
	{
		//TODO: change this to volume
		this.m_AudioListener.enabled = !mute;
	}
	
	public void Play (string s1, string s2)
	{
//		Print.Log ("Playing: " + s1 + "_" + s2);
		find (s1 + "_" + s2);
	}

	// Use this for initialization
	void Start () {
		Initialize ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

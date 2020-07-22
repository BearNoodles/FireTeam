using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;

public class VolumeManager : MonoBehaviour
{
	public AudioMixerGroup mixer;
	public Slider master, bgm, sfx;

	void Awake()
	{
		if (PlayerPrefs.HasKey("MasterVol")) master.value = PlayerPrefs.GetFloat("MasterVol");
		if (PlayerPrefs.HasKey("BgmVol")) bgm.value = PlayerPrefs.GetFloat("BgmVol");
		if (PlayerPrefs.HasKey("SfxVol")) sfx.value = PlayerPrefs.GetFloat("SfxVol");
	}

	void Update()
	{
		SetMasterVol(master.value);
		SetBgmVol(bgm.value);
		SetSfxVol(sfx.value);
		PlayerPrefs.Save();
	}

	void SetMasterVol(float lvl)
	{
		float level = (Mathf.Log((lvl / 1.3333333F) + 1, 10) * 80) - 80;
		mixer.audioMixer.SetFloat("masterVol", level);
		master.transform.Find("Label").GetComponent<Text>().text = ((int)lvl).ToString();
		PlayerPrefs.SetFloat("MasterVol", lvl);
	}

	void SetBgmVol(float lvl)
	{
		float level = (Mathf.Log((lvl / 1.3333333F) + 1, 10) * 80) - 80;
		mixer.audioMixer.SetFloat("bgmVol", level);
		bgm.transform.Find("Label").GetComponent<Text>().text = ((int)lvl).ToString();
		PlayerPrefs.SetFloat("BgmVol", lvl);
	}

	void SetSfxVol(float lvl)
	{
		float level = (Mathf.Log((lvl / 1.3333333F) + 1, 10) * 80) - 80;
		mixer.audioMixer.SetFloat("sfxVol", level);
		sfx.transform.Find("Label").GetComponent<Text>().text = ((int)lvl).ToString();
		PlayerPrefs.SetFloat("SfxVol", lvl);
	}
}

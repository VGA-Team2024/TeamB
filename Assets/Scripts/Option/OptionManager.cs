using System.Collections;
using System.Collections.Generic;
using TeamB.GameSystem.Statics;
using TeamB.UI;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
	[SerializeField] List<Slider> slider = new List<Slider>();

	[SerializeField] GameObject option_canvas;
	[SerializeField] GameObject display_canvas;

	[SerializeField] GameObject display_prefab;

	[SerializeField] Transform trans;

	[SerializeField] Button option_button;

	[SerializeField] List<DisplayInfo> displaylist = new List<DisplayInfo>();

	TitleUIView title_uiview;

	enum Audioname
	{
		se,
		voice,
		master
	}


	void Start()
	{
		title_uiview = FindObjectOfType<TitleUIView>();
		slider[(int)Audioname.master].onValueChanged.AddListener(value =>
		{
			GameStatics.AudioInfo.ChangeMasterVolume(value);

			CRIAudioManager.BGM.SetVolume(
				GameStatics.AudioInfo.GetMasterVolume * GameStatics.AudioInfo.BGM.GetBGMVolume);
			CRIAudioManager.SE.SetVolume(
				GameStatics.AudioInfo.GetMasterVolume * GameStatics.AudioInfo.SE.GetSEVolume);
			CRIAudioManager.VOICE.SetVolume(
				GameStatics.AudioInfo.GetMasterVolume * GameStatics.AudioInfo.Voice.GetVoiceVolume);
			CRIAudioManager.BGM.Update();
			CRIAudioManager.SE.Update();
			CRIAudioManager.VOICE.Update();
		});

		slider[(int)Audioname.voice].onValueChanged.AddListener(value =>
		{
			GameStatics.AudioInfo.Voice.VolumeChange(value);
			CRIAudioManager.VOICE.SetVolume(
				GameStatics.AudioInfo.GetMasterVolume * GameStatics.AudioInfo.Voice.GetVoiceVolume);
			CRIAudioManager.VOICE.Update();
		});

		slider[(int)Audioname.se].onValueChanged.AddListener(value =>
		{
			GameStatics.AudioInfo.SE.VolumeChange(value);
			CRIAudioManager.SE.SetVolume(
				GameStatics.AudioInfo.GetMasterVolume * GameStatics.AudioInfo.SE.GetSEVolume);
			CRIAudioManager.SE.Update();
		});

		option_button.onClick.AddListener(
			() =>
			{
				option_canvas.SetActive(true);
				title_uiview.ClickSound();
			});

		DisplayInt();
	}

	public void BackButton()
	{
		option_canvas.SetActive(false);
	}

	public void DisplayBackButton()
	{
		display_canvas.SetActive(false);
		option_canvas.SetActive(true);
	}

	public void DisplayChange()
	{
		display_canvas.SetActive(true);
		option_canvas.SetActive(false);
	}

	void DisplayInt()
	{
		Screen.GetDisplayLayout(displaylist);
		var i = 0;

		foreach (var list in displaylist)
		{
			DisplayPrefab disprefab = Instantiate(display_prefab, trans).GetComponent<DisplayPrefab>();
			disprefab.displayname.text = "ディスプレイ" + i;
			disprefab.displaybutton.onClick.AddListener(() => Screen.MoveMainWindowTo(list, list.workArea.position));
			i++;
		}
	}

	public interface IAudio
	{
		public void VolumeChange(float value);
	}

	public class AudioInfo
	{
		private float MasterVolume;
		public float GetMasterVolume => MasterVolume;
		public BGMInfo BGM = new BGMInfo();
		public SEInfo SE = new SEInfo();
		public VoiceInfo Voice = new VoiceInfo();

		public void ChangeMasterVolume(float value)
		{
			MasterVolume = value;
		}
	}

	public class BGMInfo : IAudio
	{
		private float BGMVolume = 1f;
		public float GetBGMVolume => BGMVolume;


		public void VolumeChange(float value)
		{
			BGMVolume = value;
		}
	}

	public class SEInfo : IAudio
	{
		private float SEVolume = 1f;
		public float GetSEVolume => SEVolume;

		public void VolumeChange(float value)
		{
			SEVolume = value;
		}
	}

	public class VoiceInfo : IAudio
	{
		private float VoiceVolume = 1f;
		public float GetVoiceVolume => VoiceVolume;

		public void VolumeChange(float value)
		{
			VoiceVolume = value;
		}
	}
}

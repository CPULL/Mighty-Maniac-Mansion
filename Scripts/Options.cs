using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Options : MonoBehaviour {
  public Canvas optionsCanvas;
  public Slider MainVolume;
  public TextMeshProUGUI MainVal;
  public Slider MusicVolume;
  public TextMeshProUGUI MusicVal;
  public Slider SoundVolume;
  public TextMeshProUGUI SoundVal;
  public Slider BackSndVolume;
  public TextMeshProUGUI BackSndVal;

  public Slider TextSpeed;
  public TextMeshProUGUI TextVal;
  public Slider WalkSpeed;
  public TextMeshProUGUI WalkVal;
  public TMP_Dropdown Highlights;

  public AudioMixer mixerMusic;

  public void ChangeMainVolume() {
    float vol = 40f * MainVolume.value - 40;
    mixerMusic.SetFloat("MasterVolume", vol);
    MainVal.text = (int)(MainVolume.value * 100) + "%";
  }

  public void ChangeMusicVolume() {
    float vol = 10 * Mathf.Log(1 + MusicVolume.value * .74f) * 14.425f - 80;
    mixerMusic.SetFloat("MusicVolume", vol);
    MusicVal.text = (int)(MusicVolume.value * 100) + "%";
  }

  public void ChangeSoundVolume() {
    float vol = 10 * Mathf.Log(1 + SoundVolume.value * .74f) * 14.425f - 80;
    mixerMusic.SetFloat("SoundsVolume", vol);
    SoundVal.text = (int)(SoundVolume.value * 100) + "%";
  }

  public void ChangeBackSndVolume() {
    float vol = 10 * Mathf.Log(1 + BackSndVolume.value * .74f) * 14.425f - 80;
    mixerMusic.SetFloat("BackSoundsVolume", vol);
    BackSndVal.text = (int)(BackSndVolume.value * 100) + "%";
  }


  public void Activate(bool activate) {
    optionsCanvas.enabled = activate;

    if (activate) {
      float val = PlayerPrefs.GetFloat("MasterVolume", 1);
      MainVolume.SetValueWithoutNotify(val);
      val = PlayerPrefs.GetFloat("MusicVolume", 1);
      MusicVolume.SetValueWithoutNotify(val);
      val = PlayerPrefs.GetFloat("SoundVolume", 1);
      SoundVolume.SetValueWithoutNotify(val);
      val = PlayerPrefs.GetFloat("BackSoundsVolume", 1);
      BackSndVolume.SetValueWithoutNotify(val);
      ChangeMainVolume();
      ChangeMusicVolume();
      ChangeSoundVolume();
      ChangeBackSndVolume();
    }
    else {
      PlayerPrefs.SetFloat("MasterVolume", MainVolume.value);
      PlayerPrefs.SetFloat("MusicVolume", MusicVolume.value);
      PlayerPrefs.SetFloat("SoundVolume", SoundVolume.value);
      PlayerPrefs.SetFloat("BackSoundsVolume", BackSndVolume.value);
    }
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.Escape)) {
      Activate(!optionsCanvas.enabled);
    }
  }
}

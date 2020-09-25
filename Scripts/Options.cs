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

  public void ChangeMaxWalkSpeed() {
    WalkVal.text = (int)(WalkSpeed.value * 100) + "%";
  }

  public void ChangeTextSpeed() {
    TextVal.text = (int)(TextSpeed.value * 100) + "%";
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

      val = PlayerPrefs.GetFloat("WalkSpeed", 1);
      WalkSpeed.SetValueWithoutNotify(val);
      ChangeMaxWalkSpeed();
      val = PlayerPrefs.GetFloat("TextSpeed", 1);
      TextSpeed.SetValueWithoutNotify(val);
      ChangeTextSpeed();
    }
    else {
      PlayerPrefs.SetFloat("MasterVolume", MainVolume.value);
      PlayerPrefs.SetFloat("MusicVolume", MusicVolume.value);
      PlayerPrefs.SetFloat("SoundVolume", SoundVolume.value);
      PlayerPrefs.SetFloat("BackSoundsVolume", BackSndVolume.value);
      PlayerPrefs.SetFloat("TextSpeed", TextSpeed.value);
      PlayerPrefs.SetFloat("WalkSpeed", WalkSpeed.value);
      Controller.walkSpeed = WalkSpeed.value;
      Controller.textSpeed = 1 / TextSpeed.value;
    }
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.Escape)) {
      Activate(!optionsCanvas.enabled);
    }
  }

  internal void GetOptions() {
    float vol = 40f * PlayerPrefs.GetFloat("MasterVolume", 1) - 40;
    mixerMusic.SetFloat("MasterVolume", vol);

    vol = 10 * Mathf.Log(1 + PlayerPrefs.GetFloat("MusicVolume", 1) * .74f) * 14.425f - 80;
    mixerMusic.SetFloat("MusicVolume", vol);

    vol = 10 * Mathf.Log(1 + PlayerPrefs.GetFloat("SoundsVolume", 1) * .74f) * 14.425f - 80;
    mixerMusic.SetFloat("SoundsVolume", vol);

    vol = 10 * Mathf.Log(1 + PlayerPrefs.GetFloat("BackSoundsVolume", 1) * .74f) * 14.425f - 80;
    mixerMusic.SetFloat("BackSoundsVolume", vol);

    Controller.walkSpeed = PlayerPrefs.GetFloat("WalkSpeed", 1);
    Controller.textSpeed = PlayerPrefs.GetFloat("TalkSpeed", 1);
  }
}

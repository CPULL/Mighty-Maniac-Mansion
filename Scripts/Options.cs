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

  public static Options opts;


  public TextMeshProUGUI DBG;

  private void Awake() {
    opts = this;
  }

  public static bool IsActive() {
    if (opts == null) return false;
    return opts.optionsCanvas.enabled;
  }

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


  CursorTypes prevCursor = CursorTypes.None;

  public static void Activate(bool activate) {
    if (opts == null) return;
    opts.ActualActivation(activate);
  }

  private void ActualActivation(bool activate) {
    optionsCanvas.enabled = activate;

    Controller.PauseMusic();
    if (activate) {
      prevCursor = Controller.GetCursor();
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
      Controller.SetCursor(prevCursor);
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
    if (Input.GetKeyDown(KeyCode.F11)) {
      if (Screen.fullScreen) {
        Screen.SetResolution(1920, 1080, false);
      }
      else
        Screen.fullScreen = true;
    }
  }

  internal static void GetOptions() {
    if (opts == null) return;

    float vol = 40f * PlayerPrefs.GetFloat("MasterVolume", 1) - 40;
    opts.mixerMusic.SetFloat("MasterVolume", vol);

    vol = 10 * Mathf.Log(1 + PlayerPrefs.GetFloat("MusicVolume", 1) * .74f) * 14.425f - 80;
    opts.mixerMusic.SetFloat("MusicVolume", vol);

    vol = 10 * Mathf.Log(1 + PlayerPrefs.GetFloat("SoundsVolume", 1) * .74f) * 14.425f - 80;
    opts.mixerMusic.SetFloat("SoundsVolume", vol);

    vol = 10 * Mathf.Log(1 + PlayerPrefs.GetFloat("BackSoundsVolume", 1) * .74f) * 14.425f - 80;
    opts.mixerMusic.SetFloat("BackSoundsVolume", vol);

    Controller.walkSpeed = PlayerPrefs.GetFloat("WalkSpeed", 1);
    Controller.textSpeed = PlayerPrefs.GetFloat("TalkSpeed", 1);
  }

  public void QuitGame() {
    // FIXME confirm.
    Application.Quit(0);
  }

}

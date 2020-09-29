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

  public Slider TextSpeed;
  public TextMeshProUGUI TextVal;
  public Slider WalkSpeed;
  public TextMeshProUGUI WalkVal;
  public TMP_Dropdown Highlights;

  public AudioMixer mixerMusic;

  public TextMeshProUGUI TextFontLabel;
  public Toggle TextFontTG;
  public GameObject FontsContainer;
  public TMP_FontAsset[] FontAssets;
  public Material[] FontMaterials;
  public string[] FontNames;

  public Button RestartIntro;
  public Button RestartNewChars;
  public Button RestartSameChars;

  private void Awake() {
    GD.opts = this;
  }

  public static bool IsActive() {
    if (GD.opts == null) return false;
    return GD.opts.optionsCanvas.enabled;
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

  public void ChangeMaxWalkSpeed() {
    WalkVal.text = (int)(WalkSpeed.value * 100) + "%";
  }

  public void ChangeTextSpeed() {
    TextVal.text = (int)(TextSpeed.value * 100) + "%";
  }


  public void ChangeFontTG() {
    FontsContainer.SetActive(TextFontTG.isOn);
  }


  public void SelectFont(int num) {
    FontsContainer.SetActive(false);
    TextFontTG.SetIsOnWithoutNotify(false);

    TextFontLabel.font = FontAssets[num];
    TextFontLabel.fontSharedMaterial = FontMaterials[num];
    TextFontLabel.text = FontNames[num];

    UpdateFonts(num);
    PlayerPrefs.SetInt("Font", num);
  }

  CursorTypes prevCursor = CursorTypes.None;

  public static void Activate(bool activate) {
    if (GD.opts == null) return;
    GD.opts.ActualActivation(activate);
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
      ChangeMainVolume();
      ChangeMusicVolume();
      ChangeSoundVolume();

      val = PlayerPrefs.GetFloat("WalkSpeed", 1);
      WalkSpeed.SetValueWithoutNotify(val);
      ChangeMaxWalkSpeed();
      val = PlayerPrefs.GetFloat("TextSpeed", 1);
      TextSpeed.SetValueWithoutNotify(val);
      ChangeTextSpeed();

      RestartIntro.interactable = GD.status == GameStatus.IntroVideo || GD.status == GameStatus.CharSelection || GD.status == GameStatus.Cutscene || GD.status == GameStatus.NormalGamePlay || GD.status == GameStatus.StartGame;
      RestartNewChars.interactable = GD.status == GameStatus.Cutscene || GD.status == GameStatus.NormalGamePlay || GD.status == GameStatus.StartGame;
      RestartSameChars.interactable = GD.status == GameStatus.Cutscene || GD.status == GameStatus.NormalGamePlay || GD.status == GameStatus.StartGame;
    }
    else {
      Controller.SetCursor(prevCursor);
      PlayerPrefs.SetFloat("MasterVolume", MainVolume.value);
      PlayerPrefs.SetFloat("MusicVolume", MusicVolume.value);
      PlayerPrefs.SetFloat("SoundVolume", SoundVolume.value);
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
    if (GD.opts == null) return;

    float vol = 40f * PlayerPrefs.GetFloat("MasterVolume", 1) - 40;
    GD.opts.mixerMusic.SetFloat("MasterVolume", vol);

    vol = 10 * Mathf.Log(1 + PlayerPrefs.GetFloat("MusicVolume", 1) * .74f) * 14.425f - 80;
    GD.opts.mixerMusic.SetFloat("MusicVolume", vol);

    vol = 10 * Mathf.Log(1 + PlayerPrefs.GetFloat("SoundsVolume", 1) * .74f) * 14.425f - 80;
    GD.opts.mixerMusic.SetFloat("SoundsVolume", vol);

    Controller.walkSpeed = PlayerPrefs.GetFloat("WalkSpeed", 1);
    Controller.textSpeed = PlayerPrefs.GetFloat("TalkSpeed", 1);

    GD.opts.UpdateFonts(PlayerPrefs.GetInt("Font", 3));
  }

  public void QuitGame() {
    Confirm.Show("Are you sure you want to quit?", 0);
  }

  public void RestartGameNew() {
    Confirm.Show("Are you sure you want restart?\n<size=72>You will select new characters</size>", 2);
  }

  public void RestartGameSame() {
    Confirm.Show("Are you sure you want restart?\n<size=72>You will use the same party</size>", 1);
  }

  public void RestartFromIntro() {
    Confirm.Show("Are you sure you want restart?\n<size=72>From the intro sequence</size>", 3);
  }


  private void UpdateFonts(int num) {
    GD.charSel.ActorDescription.font = FontAssets[num];
    GD.charSel.ActorDescription.fontSharedMaterial = FontMaterials[num];

    GD.b.text.font = FontAssets[num];
    GD.b.text.fontSharedMaterial = FontMaterials[num];


  }
}

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

  private string GetStringValue(float val) {
    if (val == 0) return "<i>disabled</i>";
    if (val == 1) return "25%";
    if (val == 2) return "50%";
    if (val == 3) return "75%";
    if (val == 4) return "80%";
    if (val == 5) return "90%";
    if (val == 6) return "100%";
    if (val == 7) return "110%";
    if (val == 8) return "125%";
    if (val == 9) return "150%";
    if (val == 10) return "200%";
    if (val == 11) return "300%";
    if (val == 12) return "400%";
    return val + " ???";
  }

  private static float GetFloatValue(float val) {
    if (val == 0) return 0;
    if (val == 1) return .25f;
    if (val == 2) return .50f;
    if (val == 3) return .75f;
    if (val == 4) return .80f;
    if (val == 5) return .90f;
    if (val == 6) return 1;
    if (val == 7) return 1.10f;
    if (val == 8) return 1.25f;
    if (val == 9) return 1.50f;
    if (val == 10) return 2.00f;
    if (val == 11) return 3.00f;
    if (val == 12) return 4.00f;
    return val;
  }

  private string GetStringValueD(float val) {
    if (val <= 1) return "25%";
    if (val == 2) return "50%";
    if (val == 3) return "75%";
    if (val == 4) return "80%";
    if (val == 5) return "90%";
    if (val == 6) return "100%";
    if (val == 7) return "110%";
    if (val == 8) return "125%";
    if (val == 9) return "150%";
    if (val == 10) return "200%";
    if (val == 11) return "300%";
    if (val == 12) return "400%";
    return val + " ???";
  }

  private static float GetFloatValueD(float val) {
    if (val <= 1) return .25f;
    if (val == 2) return .50f;
    if (val == 3) return .75f;
    if (val == 4) return .80f;
    if (val == 5) return .90f;
    if (val == 6) return 1;
    if (val == 7) return 1.10f;
    if (val == 8) return 1.25f;
    if (val == 9) return 1.50f;
    if (val == 10) return 2.00f;
    if (val == 11) return 3.00f;
    if (val == 12) return 4.00f;
    return val;
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
    WalkVal.text = GetStringValueD(WalkSpeed.value);
  }

  public void ChangeTextSpeed() {
    TextVal.text = GetStringValueD(TextSpeed.value);
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

      val = PlayerPrefs.GetFloat("WalkSpeed", 6);
      WalkSpeed.SetValueWithoutNotify(val);
      ChangeMaxWalkSpeed();
      val = PlayerPrefs.GetFloat("TextSpeed", 6);
      TextSpeed.SetValueWithoutNotify(val);
      ChangeTextSpeed();

      UpdateFonts(PlayerPrefs.GetInt("Font", 3));

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
      Controller.walkSpeed = GetFloatValueD(WalkSpeed.value);
      Controller.textSpeed = GetFloatValueD(TextSpeed.value);
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

    Controller.walkSpeed = GetFloatValueD(PlayerPrefs.GetFloat("WalkSpeed", 6));
    Controller.textSpeed = GetFloatValueD(PlayerPrefs.GetFloat("TalkSpeed", 6));

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

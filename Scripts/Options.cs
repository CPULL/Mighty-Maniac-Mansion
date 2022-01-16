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
  public Slider BackgroundVolume;
  public TextMeshProUGUI BackgroundVal;

  public TMP_Dropdown UseC64ColorsDD;
  public TMP_Dropdown PixelizeDD;
  public Toggle OutlineTG;
  public Slider OutlineSizeSL;
  public TextMeshProUGUI OutlineSizeVal;
  public Slider OutlineStrenghtSL;
  public TextMeshProUGUI OutlineStrenghtVal;

  public Toggle ScanlinesTG;
  public Slider ScanlinesStrenghtSL;
  public TextMeshProUGUI ScanlinesStrenghtVal;
  public Slider ScanlinesFreqSL;
  public TextMeshProUGUI ScanlinesFreqVal;
  public Slider ScanlinesSpeedSL;
  public TextMeshProUGUI ScanlinesSpeedVal;
  public Slider ScanlinesNoiseSL;
  public TextMeshProUGUI ScanlinesNoiseVal;
  public TMP_Dropdown ScanlinesDir;
  public Toggle InterlaceTG;
  public TMP_Dropdown InventoryMode;


  public Button[] Tabs;
  public GameObject[] Parts;

  public void ClickTab(int b) {
    ColorBlock colors;
    for (int i = 0; i < 3; i++) {
      colors = Tabs[i].colors;
      colors.normalColor = new Color32(255, 255, 255, 255);
      Tabs[i].colors = colors;
      Parts[i].SetActive(false);
    }
    colors = Tabs[b].colors;
    colors.normalColor = new Color32(236, 242, 252, 255);
    colors.selectedColor = new Color32(236, 242, 252, 255);
    Tabs[b].colors = colors;
    Parts[b].SetActive(true);
  }



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
    float val = MainVolume.value;
    if (Mathf.Abs(val - 1) < .05f) {
      MainVolume.SetValueWithoutNotify(1);
      val = 1;
    }

    float vol = 40f * val - 40;
    mixerMusic.SetFloat("MasterVolume", vol);
    MainVal.text = (int)(val * 100) + "%";
  }

  public void ChangeMusicVolume() {
    float vol = 10 * Mathf.Log(1 + MusicVolume.value * .74f) * 14.425f - 80;
    mixerMusic.SetFloat("MusicVolume", vol);
    if (MusicVolume.value < .01f)
      MusicVal.text = "<i>disabled</i>";
    else
      MusicVal.text = (int)(MusicVolume.value * 100) + "%";
  }

  public void ChangeSoundVolume() {
    float vol = 10 * Mathf.Log(1 + SoundVolume.value * .74f) * 14.425f - 80;
    mixerMusic.SetFloat("SoundsVolume", vol);
    if (SoundVolume.value < .01f)
      SoundVal.text = "<i>disabled</i>";
    else
      SoundVal.text = (int)(SoundVolume.value * 100) + "%";
  }

  public void ChangeBackgroundVolume() {
    float vol = 10 * Mathf.Log(1 + BackgroundVolume.value * .74f) * 14.425f - 80;
    mixerMusic.SetFloat("BackgroundVolume", vol);
    if (BackgroundVolume.value < .01f)
      BackgroundVal.text = "<i>disabled</i>";
    else
      BackgroundVal.text = (int)(BackgroundVolume.value * 100) + "%";
  }

  public void ChangeMaxWalkSpeed() {
    WalkVal.text = GetStringValueD(WalkSpeed.value);
  }

  public void ChangeTextSpeed() {
    TextVal.text = GetStringValueD(TextSpeed.value);
  }

  public void ChangeInventoryMode() {
    PlayerPrefs.SetInt("InventoryMode", InventoryMode.value);
    Controller.inventoryMode = InventoryMode.value;
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

  public void ChangePixels() {
    PlayerPrefs.SetInt("Pixelization", PixelizeDD.value);
    ApplyC64Options();
  }

  public void ChangeC64Colors() {
    PlayerPrefs.SetInt("C64Colors", UseC64ColorsDD.value);
    ApplyC64Options();
  }

  public void ChangeOutline() {
    OutlineSizeSL.interactable = OutlineTG.isOn;
    OutlineStrenghtSL.interactable = OutlineTG.isOn;
    if (OutlineTG.isOn && OutlineSizeSL.value == 0) OutlineSizeSL.SetValueWithoutNotify(1);
    PlayerPrefs.SetInt("OutlineSize", OutlineTG.isOn ? Mathf.RoundToInt(OutlineSizeSL.value) : 0);
    PlayerPrefs.SetFloat("OutlineStrenght", OutlineStrenghtSL.value);
    ApplyC64Options();
  }

  public void ChangeOutlineSizeStrenght() {
    PlayerPrefs.SetInt("OutlineSize", OutlineTG.isOn ? Mathf.RoundToInt(OutlineSizeSL.value) : 0);
    PlayerPrefs.SetFloat("OutlineStrenght", OutlineStrenghtSL.value);
    OutlineSizeVal.text = ((int)OutlineSizeSL.value).ToString();
    OutlineStrenghtVal.text = (int)(100 * OutlineStrenghtSL.value) + "%";
    ApplyC64Options();
  }

  public void ChangeScanlines() {
    ScanlinesStrenghtSL.interactable = ScanlinesTG.isOn;
    ScanlinesFreqSL.interactable = ScanlinesTG.isOn;
    ScanlinesSpeedSL.interactable = ScanlinesTG.isOn;
    ScanlinesNoiseSL.interactable = ScanlinesTG.isOn;
    ScanlinesDir.interactable = ScanlinesTG.isOn;

    PlayerPrefs.SetInt("Scanlines", (ScanlinesTG.isOn ? 1 : 0) + (ScanlinesDir.value == 1 ? 2 : 0) + (InterlaceTG.isOn ? 4 : 0));
    ApplyC64Options();
  }

  public void ChangeScanlineFreq() {
    PlayerPrefs.SetFloat("ScanlinesFreq", ScanlinesFreqSL.value);
    ScanlinesFreqVal.text = ((int)(100 * ScanlinesFreqSL.value)) + "%";
    ApplyC64Options();
  }

  public void ChangeScanlineSpeed() {
    PlayerPrefs.SetFloat("ScanlinesSpeed", ScanlinesSpeedSL.value);
    ScanlinesSpeedVal.text = ((int)(100 * ScanlinesSpeedSL.value)) + "%";
    ApplyC64Options();
  }

  public void ChangeScanlineNoise() {
    PlayerPrefs.SetFloat("ScanlinesNoise", ScanlinesNoiseSL.value);
    ScanlinesNoiseVal.text = (((int)(100 * ScanlinesNoiseSL.value)) / 100f).ToString();
    ApplyC64Options();
  }

  public void ChangeScanlineStrenght() {
    PlayerPrefs.SetFloat("ScanlinesStrenght", ScanlinesStrenghtSL.value);
    ScanlinesStrenghtVal.text = ((int)(100 * ScanlinesStrenghtSL.value)) + "%";
    ApplyC64Options();
  }


  public static void Activate(bool activate) {
    if (GD.opts == null) return;
    GD.opts.ActualActivation(activate);
  }

  private void ActualActivation(bool activate) {
    optionsCanvas.enabled = activate;

    Controller.PauseMusic();
    if (activate) {
      ClickTab(0);
      CursorHandler.SaveCursor();

      // Audio
      float val = PlayerPrefs.GetFloat("MasterVolume", 1);
      MainVolume.SetValueWithoutNotify(val);
      val = PlayerPrefs.GetFloat("MusicVolume", .75f);
      MusicVolume.SetValueWithoutNotify(val);
      val = PlayerPrefs.GetFloat("SoundVolume", .80f);
      SoundVolume.SetValueWithoutNotify(val);
      val = PlayerPrefs.GetFloat("BackgroundVolume", .70f);
      BackgroundVolume.SetValueWithoutNotify(val);
      ChangeMainVolume();
      ChangeMusicVolume();
      ChangeSoundVolume();
      ChangeBackgroundVolume();

      // Gameplay
      val = PlayerPrefs.GetFloat("WalkSpeed", 6);
      WalkSpeed.SetValueWithoutNotify(val);
      ChangeMaxWalkSpeed();
      val = PlayerPrefs.GetFloat("TextSpeed", 6);
      TextSpeed.SetValueWithoutNotify(val);
      ChangeTextSpeed();
      InventoryMode.SetValueWithoutNotify(PlayerPrefs.GetInt("InventoryMode", 1));

      // Video
      UpdateFonts(PlayerPrefs.GetInt("Font", 3));

      PixelizeDD.SetValueWithoutNotify(PlayerPrefs.GetInt("Pixelization", 0));
      UseC64ColorsDD.SetValueWithoutNotify(PlayerPrefs.GetInt("C64Colors", 0));

      int outline = PlayerPrefs.GetInt("OutlineSize", 0);
      if (outline == 0) {
        OutlineTG.SetIsOnWithoutNotify(false);
        OutlineSizeSL.SetValueWithoutNotify(0);
      }
      else {
        OutlineTG.SetIsOnWithoutNotify(true);
        OutlineSizeSL.SetValueWithoutNotify(outline);
      }
      OutlineStrenghtSL.SetValueWithoutNotify(PlayerPrefs.GetFloat("OutlineStrenght", .5f));
      OutlineSizeVal.text = ((int)OutlineSizeSL.value).ToString();
      OutlineStrenghtVal.text = Mathf.RoundToInt(100 * OutlineStrenghtSL.value) + "%";

      int scan = PlayerPrefs.GetInt("Scanlines", 0);
      ScanlinesTG.SetIsOnWithoutNotify((scan & 1) == 1);
      ScanlinesDir.SetValueWithoutNotify((scan & 2) == 2 ? 1 : 0);
      InterlaceTG.SetIsOnWithoutNotify((scan & 4) == 4);
      ScanlinesFreqSL.value = PlayerPrefs.GetFloat("ScanlinesFreq", .5f);
      ChangeScanlineFreq();
      ScanlinesSpeedSL.value = PlayerPrefs.GetFloat("ScanlinesSpeed", .5f);
      ChangeScanlineSpeed();
      ScanlinesNoiseSL.value = PlayerPrefs.GetFloat("ScanlinesNoise", 0);
      ChangeScanlineNoise();
      ScanlinesStrenghtSL.value = PlayerPrefs.GetFloat("ScanlinesStrenght", .5f);
      ChangeScanlineStrenght();

      ApplyC64Options();


      RestartIntro.interactable = GD.theStatus == GameStatus.IntroVideo || GD.theStatus == GameStatus.CharSelection || GD.theStatus == GameStatus.NormalGamePlay || GD.theStatus == GameStatus.StartGame;
      RestartNewChars.interactable = GD.theStatus == GameStatus.NormalGamePlay || GD.theStatus == GameStatus.StartGame;
      RestartSameChars.interactable = GD.theStatus == GameStatus.NormalGamePlay || GD.theStatus == GameStatus.StartGame;
    }
    else {
      CursorHandler.ResetCursor();
      PlayerPrefs.SetFloat("MasterVolume", MainVolume.value);
      PlayerPrefs.SetFloat("MusicVolume", MusicVolume.value);
      PlayerPrefs.SetFloat("SoundVolume", SoundVolume.value);
      PlayerPrefs.SetFloat("BackgroundVolume", BackgroundVolume.value);
      PlayerPrefs.SetFloat("TextSpeed", TextSpeed.value);
      PlayerPrefs.SetFloat("WalkSpeed", WalkSpeed.value);
      Controller.walkSpeed = GetFloatValueD(WalkSpeed.value);
      Controller.textSpeed = GetFloatValueD(TextSpeed.value);
      Controller.inventoryMode = PlayerPrefs.GetInt("InventoryMode", 1);
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

    vol = 10 * Mathf.Log(1 + PlayerPrefs.GetFloat("BackgroundVolume", 1) * .74f) * 14.425f - 80;
    GD.opts.mixerMusic.SetFloat("BackgroundVolume", vol);

    if (PlayerPrefs.GetFloat("WalkSpeed", 6) <= 1) PlayerPrefs.SetFloat("WalkSpeed", 6);
    if (PlayerPrefs.GetFloat("TalkSpeed", 6) <= 1) PlayerPrefs.SetFloat("TalkSpeed", 6);
    Controller.walkSpeed = GetFloatValueD(PlayerPrefs.GetFloat("WalkSpeed", 6));
    Controller.textSpeed = GetFloatValueD(PlayerPrefs.GetFloat("TalkSpeed", 6));
    Controller.inventoryMode = PlayerPrefs.GetInt("InventoryMode", 0);
    GD.opts.InventoryMode.SetValueWithoutNotify(Controller.inventoryMode);

    GD.opts.UpdateFonts(PlayerPrefs.GetInt("Font", 3));

    GD.opts.UseC64ColorsDD.SetValueWithoutNotify(PlayerPrefs.GetInt("C64Colors", 0));
    GD.opts.PixelizeDD.SetValueWithoutNotify(PlayerPrefs.GetInt("Pixelization", 0));

    int outline = PlayerPrefs.GetInt("OutlineSize", 0);
    if (outline == 0) {
      GD.opts.OutlineTG.SetIsOnWithoutNotify(false);
      GD.opts.OutlineSizeSL.SetValueWithoutNotify(1);
    }
    else {
      GD.opts.OutlineTG.SetIsOnWithoutNotify(true);
      GD.opts.OutlineSizeSL.SetValueWithoutNotify(outline);
    }
    GD.opts.OutlineStrenghtSL.SetValueWithoutNotify(PlayerPrefs.GetFloat("OutlineStrenght", .5f));
    GD.opts.OutlineSizeVal.text = ((int)GD.opts.OutlineSizeSL.value).ToString();
    GD.opts.OutlineStrenghtVal.text = Mathf.RoundToInt(100 * GD.opts.OutlineStrenghtSL.value) + "%";

    int scan = PlayerPrefs.GetInt("Scanlines", 0);
    GD.opts.ScanlinesTG.SetIsOnWithoutNotify((scan & 1) == 1);
    GD.opts.ScanlinesDir.SetValueWithoutNotify((scan & 2) == 2 ? 1 : 0);
    GD.opts.InterlaceTG.SetIsOnWithoutNotify((scan & 4) == 4);
    GD.opts.ScanlinesFreqSL.value = PlayerPrefs.GetFloat("ScanlinesFreq", .5f);
    GD.opts.ChangeScanlineFreq();
    GD.opts.ScanlinesSpeedSL.value = PlayerPrefs.GetFloat("ScanlinesSpeed", .5f);
    GD.opts.ChangeScanlineSpeed();
    GD.opts.ScanlinesNoiseSL.value = PlayerPrefs.GetFloat("ScanlinesNoise", 0);
    GD.opts.ChangeScanlineNoise();
    GD.opts.ScanlinesStrenghtSL.value = PlayerPrefs.GetFloat("ScanlinesStrenght", .5f);
    GD.opts.ChangeScanlineStrenght();

    GD.opts.ApplyC64Options();
  }

  public void QuitGame() {
    ActualActivation(false);
    Confirm.Show("Are you sure you want to quit?", 0);
  }

  public void RestartGameNew() {
    ActualActivation(false);
    Confirm.Show("Are you sure you want restart?\n<size=72>You will select new characters</size>", 2);
  }

  public void RestartGameSame() {
    ActualActivation(false);
    Confirm.Show("Are you sure you want restart?\n<size=72>You will use the same party</size>", 1);
  }

  public void RestartFromIntro() {
    ActualActivation(false);
    Confirm.Show("Are you sure you want restart?\n<size=72>From the intro sequence</size>", 3);
  }


  private void UpdateFonts(int num) {
    TextFontLabel.font = FontAssets[num];
    TextFontLabel.fontSharedMaterial = FontMaterials[num];
    TextFontLabel.text = FontNames[num];

    GD.charSel.ActorDescription.font = FontAssets[num];
    GD.charSel.ActorDescription.fontSharedMaterial = FontMaterials[num];

    GD.b.text.font = FontAssets[num];
    GD.b.text.fontSharedMaterial = FontMaterials[num];
  }


  public void ApplyC64Options() {
    GD.SetC64Mode(UseC64ColorsDD.value, PixelizeDD.value,
      (OutlineTG.isOn ? (int)OutlineSizeSL.value : 0), OutlineStrenghtSL.value,
      PlayerPrefs.GetInt("Scanlines", 0), ScanlinesFreqSL.value, ScanlinesSpeedSL.value, ScanlinesNoiseSL.value, ScanlinesStrenghtSL.value
    );
  }
}

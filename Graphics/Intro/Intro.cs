using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
  public Canvas IntroCanvas;
  public Image IntroBlackFade;
  public TextMeshProUGUI IntroTitle;
  public TextMeshProUGUI IntroCredits;
  public RectTransform IntroTitleRT;
  public AudioSource introEffects;
  public AudioSource meteorSound;
  public AudioClip Crickets;
  public AudioClip MeteorSwing;
  public AudioClip LightsSound;
  public AudioClip IntroMusic;
  public AudioClip ExplosionSound;
  public RectTransform MeteorRT;
  public RectTransform PanelRT;
  public GameObject Explosion;
  public GameObject MansionLights;
  public GameObject PurpleGlow;
  public Material PurpleGlowMat;
  public GameObject TitleIntro1;
  public GameObject TitleIntro2;
  public GameObject TitleIntro3;
  public Material TitleIntro1mat;
  public Material TitleIntro2mat;
  public Material TitleIntro3mat;
  public Image logoCPU;
  public Image LogoOllie;
  public Image logoOLC;
  public Image logoScummvm;
  public Image[] flash;

  public float musicLightTime = 0;

  enum IntroStep {
    Init, Wait1,
    FirstText, FadeIn, HideText,
    Meteor, Explosion, Wait2, MansionLights, MusicStart,
    MMTitle1, MMTitle2, MMTitle3, Wait3,
    Credits1, Credits2, Credits3, Credits4, Credits5, Credits6, Credits7, Credits8, Credits9, Credits10, Credits11, Credits12
  };
  IntroStep istep = IntroStep.Init;
  float introTime = 0;
  bool glowing = false;
  float glowingTime = 0;
  bool musicLight = false;
  int note = 0;
  readonly float[] flashing = new float[6];


  public void Init() {
    glowing = false;
    IntroBlackFade.color = new Color32(0, 0, 0, 255);
    MeteorRT.anchoredPosition = new Vector2(2200, 1024);
    IntroTitleRT.sizeDelta = new Vector2(0, 120);
    PanelRT.anchoredPosition = new Vector2(-1920, 240);
    MansionLights.SetActive(false);
    Explosion.SetActive(false);
    PurpleGlow.SetActive(false);
    introEffects.Stop();
    meteorSound.Stop();
    TitleIntro1mat.SetFloat("_OffsetUvX", 1);
    TitleIntro1mat.SetFloat("_OffsetUvY", 0);
    TitleIntro2mat.SetFloat("_OffsetUvX", 1);
    TitleIntro2mat.SetFloat("_OffsetUvY", 0);
    TitleIntro3mat.SetFloat("_OffsetUvX", -.75f);
    TitleIntro3mat.SetFloat("_OffsetUvY", -.75f);
    TitleIntro1.SetActive(false);
    TitleIntro2.SetActive(false);
    TitleIntro3.SetActive(false);
    IntroTitle.text = "";
    IntroCredits.text = "";
    logoCPU.enabled = false;
    LogoOllie.enabled = false;
    logoOLC.enabled = false;
    logoScummvm.enabled = false;
    note = 0;
    flash[0].enabled = false;
    flash[1].enabled = false;
    flash[2].enabled = false;
    flash[3].enabled = false;
    flash[4].enabled = false;
    flash[5].enabled = false;
  }


  public bool PlayIntro(float deltaTime) {
    introTime += deltaTime;
    glowingTime += deltaTime;

    if (glowing) {
      PurpleGlowMat.SetFloat("_ColorChangeLuminosity", Mathf.Sin(glowingTime * 6) * .5f + .5f);
    }

    if (musicLight) {
      musicLightTime += Time.deltaTime;

      if (musicLightTime >= notes[note].time) {
        // Flash
        flash[notes[note].note].enabled = true;
        flashing[notes[note].note] = .25f;
        note++;
        if (note >= notes.Length) note = 0;
      }
      for (int i = 0; i < 6; i++) {
        if (flashing[i] > 0) {
          flashing[i] -= Time.deltaTime;
          if (flashing[i] <= 0)
            flash[i].enabled = false;
        }
      }
    }

    float step;
    switch (istep) {
      case IntroStep.Init: {
        Init();
        introEffects.clip = Crickets;
        introEffects.volume = .75f;
        introEffects.Play();
        istep = IntroStep.Wait1;
      }
      break;

      case IntroStep.Wait1: {
        if (introTime > .5f) {
          introTime = 0;
          istep = IntroStep.FirstText;
        }
      }
      break;

      case IntroStep.FirstText: {
        IntroTitle.text = "A few years ago...";
        step = introTime * 3;
        if (step > 1) step = 1;
        IntroTitleRT.sizeDelta = new Vector2(step * 1600, 120);
        if (introTime > 1.5f) {
          introTime = 0;
          istep = IntroStep.FadeIn;
        }
      }
      break;


      case IntroStep.FadeIn: {
        step = 255 - introTime * 512;
        if (step < 0) step = 0;
        if (step > 255) step = 255;
        IntroBlackFade.color = new Color32(0, 0, 0, (byte)step);
        if (introTime > .5f) {
          introTime = 0;
          istep = IntroStep.HideText;
        }
      }
      break;

      case IntroStep.HideText: {
        step = (1 - introTime) * 3;
        if (step > 1) step = 1;
        if (step < 0) step = 0;
        IntroTitleRT.sizeDelta = new Vector2(step * 1600, 120);
        if (introTime > 1.5f) {
          introTime = 0;
          IntroTitleRT.sizeDelta = Vector2.zero;
          istep = IntroStep.Meteor;
          introEffects.volume = .5f;
          meteorSound.clip = MeteorSwing;
          meteorSound.Play();
        }
      }
      break;


      case IntroStep.Meteor: {
        step = introTime * 1.33333f;
        MeteorRT.anchoredPosition = new Vector2(
          -200f * step + 2200 * (1 - step),
          74f * step + 1024 * (1 - step));
        PanelRT.anchoredPosition = new Vector2(-1000 * step - 1920 * (1 - step), 240);
        if (introTime > .75f) {
          introTime = 0;
          istep = IntroStep.Explosion;
          Explosion.SetActive(true);
          MeteorRT.anchoredPosition = new Vector2(1200, 1200);
          meteorSound.clip = ExplosionSound;
          meteorSound.Play();
        }
      }
      break;

      case IntroStep.Explosion: {
        step = introTime;
        PanelRT.anchoredPosition = new Vector2(-16 * step - 1000 * (1 - step), 240);
        if (introTime > 1f) {
          introTime = 0;
          PurpleGlow.SetActive(true);
          istep = IntroStep.Wait2;
        }
      }
      break;

      case IntroStep.Wait2: {
        glowing = true;
        // Fade the critters out
        step = .5f - introTime;
        if (step < 0) step = 0;
        introEffects.volume = step;
        if (introTime > .5f) {
          introTime = 0;
          istep = IntroStep.MansionLights;
          MansionLights.SetActive(true);
          introEffects.clip = LightsSound;
          introEffects.volume = 1;
          introEffects.Play();
        }
      }
      break;

      case IntroStep.MansionLights: {
        if (introTime > 2f) {
          introTime = 0;
          istep = IntroStep.MusicStart;
          introEffects.clip = IntroMusic;
          introEffects.Play();
        }
      }
      break;

      case IntroStep.MusicStart: {
        if (introTime >= 2) {
          introTime = 0;
          istep = IntroStep.MMTitle1;
          TitleIntro1mat.SetFloat("_PixelateSize", 4);
          TitleIntro1mat.SetFloat("_OffsetUvX", 1);
          TitleIntro1.SetActive(true);
        }
      }
      break;


      case IntroStep.MMTitle1: {
        step = 1 - 2 * introTime;
        TitleIntro1mat.SetFloat("_OffsetUvX", step);
        if (introTime >= .5f) {
          TitleIntro1mat.SetFloat("_OffsetUvX", 0);
          introTime = 0;
          istep = IntroStep.MMTitle2;
          TitleIntro2mat.SetFloat("_OffsetUvX", 1);
          TitleIntro2.SetActive(true);
        }
      }
      break;

      case IntroStep.MMTitle2: {
        step = 1 - 2 * introTime;
        TitleIntro2mat.SetFloat("_OffsetUvX", step);
        if (introTime >= .5f) {
          TitleIntro2mat.SetFloat("_OffsetUvX", 0);
          introTime = 0;
          istep = IntroStep.MMTitle3;
          TitleIntro3mat.SetFloat("_OffsetUvX", -.75f);
          TitleIntro3mat.SetFloat("_OffsetUvY", -.75f);
          TitleIntro3.SetActive(true);
        }
      }
      break;

      case IntroStep.MMTitle3: {
        step = 2 * introTime;
        TitleIntro3mat.SetFloat("_OffsetUvX", -.75f * (1 - step));
        TitleIntro3mat.SetFloat("_OffsetUvY", -.75f * (1 - step));
        if (introTime >= .5f) {
          TitleIntro2mat.SetFloat("_OffsetUvX", 0);
          TitleIntro2mat.SetFloat("_OffsetUvY", 0);
          introTime = 0;
          istep = IntroStep.Wait3;
        }
      }
      break;

      case IntroStep.Wait3: {
        if (introTime > 2f) {
          introTime = 0;
          istep = IntroStep.Credits1;
          IntroCredits.text = "A fanmade game by CPU";
          logoCPU.enabled = true;
        }
      }
      break;

      case IntroStep.Credits1: {
        if (introTime > 6f) {
          introTime = 0;
          istep = IntroStep.Credits2;
          IntroCredits.text = "Original game from\n    <b>Lucasfilm</b>  -  <i>1984</i>";
          logoCPU.enabled = false;
        }
      }
      break;

      case IntroStep.Credits2: {
        if (introTime > 6f) {
          introTime = 0;
          istep = IntroStep.Credits3;
          IntroCredits.text = "Original developers:\n<size=60>  <i>Ron Gilbert, Gary Winnick, \n  Chris Grigg, and David Lawrence</i></size>";
        }
      }
      break;

      case IntroStep.Credits3: {
        if (introTime > 8f) {
          introTime = 0;
          istep = IntroStep.Credits4;
          IntroCredits.text = " ";
        }
      }
      break;

      case IntroStep.Credits4: {
        if (introTime > 1f) {
          introTime = 0;
          istep = IntroStep.Credits5;
          IntroCredits.text = "Remake credits:";
        }
      }
      break;

      case IntroStep.Credits5: {
        if (introTime > 2.5f) {
          introTime = 0;
          istep = IntroStep.Credits6;
          IntroCredits.text = "Remake credits:\n Code - <b>CPU</b>";
        }
      }
      break;

      case IntroStep.Credits6: {
        if (introTime > 5f) {
          introTime = 0;
          istep = IntroStep.Credits7;
          IntroCredits.text = "Remake credits:\n Graphics - <b>CPU</b>";
        }
      }
      break;

      case IntroStep.Credits7: {
        if (introTime > 5f) {
          introTime = 0;
          istep = IntroStep.Credits8;
          IntroCredits.text = "Remake credits:\n Intro Music - <b>BG Ollie</b>";
          LogoOllie.enabled = true;
        }
      }
      break;

      case IntroStep.Credits8: {
        if (introTime > 5f) {
          introTime = 0;
          istep = IntroStep.Credits9;
          IntroCredits.text = "Remake credits:\n Sound effects - <b>CPU</b>";
          LogoOllie.enabled = false;
        }
      }
      break;

      case IntroStep.Credits9: {
        if (introTime > 5f) {
          introTime = 0;
          istep = IntroStep.Credits10;
          IntroCredits.text = "Remake credits:\n Testing - <i>nobody yet</i>";
        }
      }
      break;

      case IntroStep.Credits10: {
        if (introTime > 5f) {
          introTime = 0;
          istep = IntroStep.Credits11;
          IntroCredits.text = "Thanks to:\n<size=52> - <b>One Lone Coder community</b>\n - <b>ScummVM team</b></size>";
          logoOLC.enabled = true;
          logoScummvm.enabled = true;
        }
      }
      break;

      case IntroStep.Credits11: {
        if (introTime > 14f) {
          introTime = 0;
          istep = IntroStep.Wait3;
          IntroCredits.text = " ";
          logoOLC.enabled = false;
          logoScummvm.enabled = false;
          musicLight = true;
        }
      }
      break;
    }

    return musicLightTime > 54f;
  }

  readonly Note[] notes = new Note[] {
    new Note(5, 2.241287f),
    new Note(4, 2.41436f),
    new Note(3, 2.62439f),
    new Note(3, 2.861239f),
    new Note(5, 3.165506f),
    new Note(3, 3.331381f),
    new Note(5, 4.589778f),
    new Note(4, 4.742952f),
    new Note(3, 4.900778f),
    new Note(3, 5.302287f),
    new Note(5, 5.576581f),
    new Note(3, 5.743541f),
    new Note(5, 7.106363f),
    new Note(4, 7.275028f),
    new Note(3, 7.428267f),
    new Note(3, 7.795907f),
    new Note(5, 8.067598f),
    new Note(3, 8.246093f),
    new Note(3, 9.49279f),
    new Note(4, 9.726915f),
    new Note(5, 9.916821f),
    new Note(5, 10.28577f),
    new Note(3, 10.53478f),
    new Note(5, 10.75935f),
    new Note(5, 12.2146f),
    new Note(4, 12.33119f),
    new Note(3, 12.47521f),
    new Note(3, 12.78737f),
    new Note(5, 13.07114f),
    new Note(3, 13.26289f),
    new Note(5, 14.57162f),
    new Note(4, 14.78723f),
    new Note(3, 14.94212f),
    new Note(3, 15.30718f),
    new Note(5, 15.60359f),
    new Note(3, 15.79114f),
    new Note(5, 17.12395f),
    new Note(4, 17.31835f),
    new Note(3, 17.45218f),
    new Note(3, 17.81434f),
    new Note(5, 18.09273f),
    new Note(3, 18.27664f),
    new Note(3, 19.62356f),
    new Note(4, 19.79116f),
    new Note(5, 19.99757f),
    new Note(5, 20.27835f),
    new Note(3, 20.58154f),
    new Note(5, 20.76598f),
    new Note(2, 22.40664f),
    new Note(2, 23.89531f),
    new Note(2, 24.35887f),
    new Note(2, 24.6838f),
    new Note(1, 26.28421f),
    new Note(2, 26.71968f),
    new Note(0, 27.26314f),
    new Note(2, 28.85518f)
  };

}

public struct Note {
  public int note;
  public float time;

  public Note(int n, float t) {
    note = n;
    time = t;
  }
}

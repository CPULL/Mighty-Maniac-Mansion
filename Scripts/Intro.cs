using System.Collections;
using System.Collections.Generic;
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

  enum IntroStep {
    Init, Wait1,
    FirstText, FadeIn, HideText,
    Meteor, Explosion, Wait2, MansionLights, MusicStart,
    MMTitle1, MMTitle2, MMTitle3, Wait3,
    XXX1, XXX2, XXX3, XXX4, XXX5, XXX6, XXX7, XXX8, XXX9, XXX10, XXX11, XXX12
  };
  IntroStep istep = IntroStep.Init;
  bool glowing = false;
  float glowingTime = 0;

  float introTime = 0;
  public void PlayIntro(float deltaTime) {
    introTime += deltaTime;
    glowingTime += deltaTime;

    if (glowing) {
      PurpleGlowMat.SetFloat("_ColorChangeLuminosity", Mathf.Sin(glowingTime * 6) * .5f + .5f);
    }

    float step;
    switch (istep) {
      case IntroStep.Init: {
        glowing = false;
        IntroBlackFade.color = new Color32(0, 0, 0, 255);
        MeteorRT.anchoredPosition = new Vector2(2200, 1024);
        IntroTitleRT.sizeDelta = new Vector2(0, 120);
        PanelRT.anchoredPosition = new Vector2(-1920, 240);
        MansionLights.SetActive(false);
        Explosion.SetActive(false);
        PurpleGlow.SetActive(false);
        introEffects.clip = Crickets;
        introEffects.volume = .75f;
        introEffects.Play();
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
          istep = IntroStep.XXX1;
          IntroCredits.text = "A fanmade game by CPU";
        }
      }
      break;

      case IntroStep.XXX1: {
        if (introTime > 5f) {
          introTime = 0;
          istep = IntroStep.XXX2;
          IntroCredits.text = "Original game from\n    <b>Lucasfilm</b>  -  <i>1984</i>";
        }
      }
      break;

      case IntroStep.XXX2: {
        if (introTime > 5f) {
          introTime = 0;
          istep = IntroStep.XXX3;
          IntroCredits.text = "Original developers:\n  <i>Ron Gilbert, Gary Winnick, \n  Chris Grigg, and David Lawrence</i>";
        }
      }
      break;

      case IntroStep.XXX3: {
        if (introTime > 5f) {
          introTime = 0;
          istep = IntroStep.XXX4;
          IntroCredits.text = " ";
        }
      }
      break;

      case IntroStep.XXX4: {
        if (introTime > 1f) {
          introTime = 0;
          istep = IntroStep.XXX5;
          IntroCredits.text = "Remake credits:";
        }
      }
      break;

      case IntroStep.XXX5: {
        if (introTime > 2.5f) {
          introTime = 0;
          istep = IntroStep.XXX6;
          IntroCredits.text = "Remake credits:\n Code - <b>CPU</b>";
        }
      }
      break;

      case IntroStep.XXX6: {
        if (introTime > 2.5f) {
          introTime = 0;
          istep = IntroStep.XXX7;
          IntroCredits.text = "Remake credits:\n Graphics - <b>CPU</b>";
        }
      }
      break;

      case IntroStep.XXX7: {
        if (introTime > 3f) {
          introTime = 0;
          istep = IntroStep.XXX8;
          IntroCredits.text = "Remake credits:\n Intro Music - <b>BG Ollie</b>";
        }
      }
      break;

      case IntroStep.XXX8: {
        if (introTime > 2.5f) {
          introTime = 0;
          istep = IntroStep.XXX9;
          IntroCredits.text = "Remake credits:\n Sound effects - <b>CPU</b>";
        }
      }
      break;

      case IntroStep.XXX9: {
        if (introTime > 2.5f) {
          introTime = 0;
          istep = IntroStep.XXX10;
          IntroCredits.text = "Remake credits:\n Testing - <i>nobody yet</i>";
        }
      }
      break;

      case IntroStep.XXX10: {
        if (introTime > 2.5f) {
          introTime = 0;
          istep = IntroStep.XXX11;
          IntroCredits.text = "Thanks to:\n - <b>One Lone Coder community</b>\n - <b>ScummVM team</b>";
        }
      }
      break;

      case IntroStep.XXX11: {
        if (introTime > 10f) {
          introTime = 0;
          istep = IntroStep.Wait3;
          IntroCredits.text = " ";
        }
      }
      break;
    }

  }



}

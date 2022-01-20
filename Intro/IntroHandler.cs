using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroHandler : MonoBehaviour {
  public Camera cam;
  public Animator anim;
  public Canvas IntroCanvas;
  public Image IntroBlackFade;
  public AudioSource IntroEffects;
  public AudioSource MeteorSound;
  public AudioSource ExplosionSound;
  public AudioClip IntroMusic;

  private void Awake() {
    GD.intro = this;
  }

  public void Init() {
    StarsBlink.Disable(true);
    CursorHandler.Set();
    if (GD.gs.AutoSkipDebug) {
      Stop();
      return;
    }

    cam.transform.position = new Vector3(1000, 1000, -2.296684f);
    IntroCanvas.enabled = true;
    IntroBlackFade.enabled = true;
    IntroBlackFade.color = new Color32(0, 0, 0, 255);
    anim.enabled = true;
    anim.Play("Intro Animation");
  }

  void Stop() {
    anim.StopPlayback();
    anim.enabled = false;
    Controller.StopMusic();
    IntroEffects.Stop();
    MeteorSound.Stop();
    ExplosionSound.Stop();
    GD.theStatus = GameStatus.CharSelection;
    IntroCanvas.enabled = false;
    StarsBlink.Disable(false);
  }

  public void AnimTriggerStartMusic() {
    IntroEffects.Stop();
    Controller.PlayMusic(IntroMusic);
  }

  public Animator TentacleAnimator;

  public void AnimTriggerTentacleOn() {
    TentacleAnimator.enabled = true;
  }
  public void AnimTriggerTentacleOff() {
    TentacleAnimator.enabled = false;
  }

  public void AnimTriggerCompleteIntro() {
    Stop();
  }


  public void Update() {
    if (GD.theStatus != GameStatus.IntroVideo) return;
    if (Options.IsActive()) return;
    if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space)) Stop();
  }

}





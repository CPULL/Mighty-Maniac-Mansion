using UnityEngine;

public class GameOver : MonoBehaviour {
  private static GameOver go;
  public Canvas canvas;
  public Animator ExplosionAnim;
  public AudioSource Audio;
  public AudioClip CricketsAudioClip;
  public AudioClip ExplosionAudioClip;
  public AudioClip ManiacMansionMusic;
  public GameObject Title;
  public GameObject Buttons;

  float time = -1f;
  GameOverStatus status = GameOverStatus.No;

  private void Awake() {
    go = this;
  }

  public static void Reset() {
    go.canvas.enabled = false;
    go.Audio.Stop();
    go.Audio.clip = null;
    go.status = GameOverStatus.No;
  }

  public static void RunGameOver(bool nuclear = true) {
    GD.status = GameStatus.GameOver;
    if (nuclear)
      go.GameOverNuclear();
  }

  void GameOverNuclear() {
    canvas.enabled = true;
    Title.SetActive(false);
    Buttons.SetActive(false);
    Audio.clip = CricketsAudioClip;
    Audio.Play();
    time = 2f;
    status = GameOverStatus.PreTime;
  }

  private void Update() {
    if (status == GameOverStatus.No) return;

    time -= Time.deltaTime;
    if (time <= 0) {
      if (status == GameOverStatus.PreTime) {
        status = GameOverStatus.PreSound;
        ExplosionAnim.Play("Nuclear Explosion");
        time = .5f;
      }
      else if (status == GameOverStatus.PreSound) {
        status = GameOverStatus.NuclearExplosion;
        Audio.clip = ExplosionAudioClip;
        Audio.Play();
        time = 5f;
      }
      else if (status==GameOverStatus.NuclearExplosion) {
        status = GameOverStatus.GameOverTitle;
        Title.SetActive(true);
        Buttons.SetActive(true);
        time = 10f;
      }
      else if (status==GameOverStatus.GameOverTitle) {
        status = GameOverStatus.No;
        Audio.clip = ManiacMansionMusic;
        Audio.Play();
      }
    }
  }

  enum GameOverStatus {
    No,
    PreTime,
    PreSound,
    NuclearExplosion,
    GameOverTitle
  }
}


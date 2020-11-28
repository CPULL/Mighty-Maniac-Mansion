using System.Collections;
using TMPro;
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

  public GameObject PlayerGrave;
  public SpriteRenderer PlayerPortrait;
  public TextMeshPro PlayerName;
  public Woods woods;
  public Sprite Grave;
  public Room NoRoom;

  float time = -1f;
  GameOverStatus status = GameOverStatus.No;
  Room oldCurrentRoom;

  private void Awake() {
    go = this;
  }

  public static void Reset() {
    go.canvas.enabled = false;
    go.Audio.Stop();
    go.Audio.clip = null;
    go.status = GameOverStatus.No;
    go.PlayerGrave.SetActive(false);
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
    CursorHandler.SetBoth(CursorTypes.Normal);
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
        time = 5f;
      }
      else if (status==GameOverStatus.GameOverTitle) {
        status = GameOverStatus.No;
        Audio.clip = ManiacMansionMusic;
        Audio.Play();
      }



      else if (status==GameOverStatus.PlayerDeath1) {
        Fader.FadeOut();
        status = GameOverStatus.PlayerDeath2;
        time = 5f;
      }
      else if (status==GameOverStatus.PlayerDeath2) {
        Fader.FadeIn();
        oldCurrentRoom = GD.c.currentRoom;
        GD.c.currentRoom = null;
        StarsBlink.SetWoods(0);
        time = 1f;
        status = GameOverStatus.PlayerDeath3;
      }
      else if (status==GameOverStatus.PlayerDeath3) {
        Fader.FadeOut();
        go.woods.gameObject.SetActive(false);
        status = GameOverStatus.No;
        GD.c.currentRoom = oldCurrentRoom;
        GD.c.cam.transform.position = new Vector3((oldCurrentRoom.minL + oldCurrentRoom.maxR) * .5f, oldCurrentRoom.CameraGround, -10);
        GD.status = GameStatus.NormalGamePlay;
      }
    }
  }

  public static void PlayerDeath(Actor a) {
    if (a == GD.c.actor1) {
      GD.c.ActorPortrait1.portrait.sprite = go.Grave;
    }
    else if (a == GD.c.actor2) {
      GD.c.ActorPortrait2.portrait.sprite = go.Grave;
    }
    else if (a == GD.c.actor3) {
      GD.c.ActorPortrait3.portrait.sprite = go.Grave;
    }
    a.currentRoom = go.NoRoom;
    a.transform.position = new Vector3(100, 100, 0);
    a.IsVisible = false;
    go.StartCoroutine(go.DrawCemetery(a.name, a.id));
  }

  IEnumerator DrawCemetery(string name, Chars id) {
    Fader.FadeIn();
    yield return null;
    go.PlayerPortrait.sprite = GD.c.Portraits[(int)id - 10];
    go.PlayerName.text = name;
    go.PlayerGrave.SetActive(true);
    go.woods.gameObject.SetActive(true);
    yield return new WaitForSeconds(.25f);
    go.woods.Generate(false, false, -2);
    GD.status = GameStatus.GameOver;
    // Stop game, fade to cemetery, generate cemetery, add lightnights, stay 5 seconds, hide cemetery go back to gameplay
    go.status = GameOverStatus.PlayerDeath1;
    go.woods.gameObject.SetActive(true);
    StarsBlink.SetWoods(8);
    go.time = 1;
    yield return null;
    GD.c.cam.transform.position = new Vector3(-80, -20, -10);
  }

  enum GameOverStatus {
    No,
    PreTime,
    PreSound,
    NuclearExplosion,
    GameOverTitle,

    PlayerDeath1,
    PlayerDeath2,
    PlayerDeath3,

  }
}


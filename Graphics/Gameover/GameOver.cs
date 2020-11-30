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
  public GameObject Blackbackground;
  public GameObject Panel;

  public GameObject Player1Grave;
  public SpriteRenderer Player1Portrait;
  public TextMeshPro Player1Name;
  bool Player1Dead = false;
  public GameObject Player2Grave;
  public SpriteRenderer Player2Portrait;
  public TextMeshPro Player2Name;
  public Woods woods;
  public Sprite Grave;
  public Room NoRoom;
  public Room PoolBottom;

  float time = -1f;
  GameOverStatus status = GameOverStatus.No;
  bool triggerGameOver = false;
  Room oldCurrentRoom;

  private void Awake() {
    go = this;
  }

  public static void Reset() {
    go.canvas.enabled = false;
    go.Audio.Stop();
    go.Audio.clip = null;
    go.status = GameOverStatus.No;
    go.Player1Grave.SetActive(false);
    go.Player2Grave.SetActive(false);
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
    Blackbackground.SetActive(true);
    Panel.SetActive(true);
    Audio.clip = CricketsAudioClip;
    Audio.Play();
    time = 2f;
    status = GameOverStatus.PreTime;
    CursorHandler.Set();
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
        time = 8f;
      }
      else if (status==GameOverStatus.PlayerDeath2) {
        if (triggerGameOver) {
          triggerGameOver = false;
          status = GameOverStatus.GameOverTitle;
          Title.SetActive(true);
          Buttons.SetActive(true);
          Blackbackground.SetActive(false);
          Panel.SetActive(false);
          canvas.enabled = true;
          time = 5f;
          CursorHandler.Set();
          return;
        }

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

  public static void PlayerDeath() {
    // Let's find who is dead.
    // In case of a single player just do the normal sequence
    // In case of a second one or two at the same time do a final gameover
    Actor dead1 = null;
    Actor dead2 = null;
    if (!GD.c.actor1.dead && GD.c.actor1.currentRoom == go.PoolBottom) {
      GD.c.ActorPortrait1.portrait.sprite = go.Grave;
      dead1 = GD.c.actor1;
    }
    if (!GD.c.actor2.dead && GD.c.actor2.currentRoom == go.PoolBottom) {
      GD.c.ActorPortrait2.portrait.sprite = go.Grave;
      if (dead1 == null)      
        dead1 = GD.c.actor2;
      else
        dead2 = GD.c.actor2;
    }
    if (!GD.c.actor3.dead && GD.c.actor3.currentRoom == go.PoolBottom) {
      GD.c.ActorPortrait3.portrait.sprite = go.Grave;
      if (dead1 == null)
        dead1 = GD.c.actor3;
      else
        dead2 = GD.c.actor3;
    }

    if (dead1 != null) {
      dead1.currentRoom = go.NoRoom;
      dead1.transform.position = new Vector3(100, 100, 0);
      dead1.IsVisible = false;
    }
    if (dead2 != null) {
      dead2.currentRoom = go.NoRoom;
      dead2.transform.position = new Vector3(100, 100, 0);
      dead2.IsVisible = false;
    }

    go.StartCoroutine(go.DrawCemetery(dead1, dead2));
  }

  IEnumerator DrawCemetery(Actor dead1, Actor dead2) {
    Fader.FadeIn();
    yield return null;

    if (dead1 != null && dead2 != null) {
      // Both graves togheter, then GameOver
      go.Player1Portrait.sprite = GD.c.Portraits[(int)dead1.id - 10];
      go.Player1Name.text = dead1.name;
      go.Player1Grave.SetActive(true);
      go.Player2Portrait.sprite = GD.c.Portraits[(int)dead2.id - 10];
      go.Player2Name.text = dead2.name;
      go.Player2Grave.SetActive(true);
      triggerGameOver = true;
    }
    else {
      if (!go.Player1Dead) {
        // Single grave
        go.Player1Portrait.sprite = GD.c.Portraits[(int)dead1.id - 10];
        go.Player1Name.text = dead1.name;
        go.Player1Grave.SetActive(true);
        go.Player1Dead = true;
      }
      else {
        // Fill second grave and then GameOver
        go.Player2Portrait.sprite = GD.c.Portraits[(int)dead1.id - 10];
        go.Player2Name.text = dead1.name;
        go.Player2Grave.SetActive(true);
        triggerGameOver = true;
      }
    }
    while (Fader.IsFading())
      yield return null;

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


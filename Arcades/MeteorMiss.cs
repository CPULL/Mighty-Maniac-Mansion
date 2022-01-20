using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MeteorMiss : MonoBehaviour {
  public Image[] Hearts;
  public RectTransform ShipRT;
  public RectTransform[] Stars;
  public RectTransform[] Meteors;
  public Image Ship;
  public TextMeshProUGUI Score;
  public AudioSource SoundShip;
  public AudioSource SoundExtra;
  public AudioClip ShipSound, MeteorScoreSound, ExplosionSound;
  public Animator shipAnim;
  public Collider2D[] meteorColliders;

  private void Start() {
    Init();
  }

  public void Init() {
    CursorHandler.Set(CursorTypes.Hidden);
    Score.text = "Score: 0";
    Ship.enabled = true;
    ShipRT.anchoredPosition = new Vector2(0, -400);
    Hearts[0].enabled = true;
    Hearts[1].enabled = true;
    Hearts[2].enabled = true;
    lives = 3;
    score = 0;
    playing = true;
    for (int i = 0; i < 8; i++)
      ms[i] = new Vector3(Random.Range(-800, 800), 670, 1 + i * 1.5f);
    SoundShip.enabled = true;
    SoundShip.clip = ShipSound;
    SoundShip.Play();
    hit = false;
  }


  int lives = 3;
  int score = 0;
  bool playing = false;
  float time = 0;
  float acc = 0;

  Vector2[] ss = new Vector2[3];
  Vector3[] ms = new Vector3[8];

  private void Update() {
    if (!playing) {
      HighScores();
      return;
    }

    // Movement
    float mx = Input.GetAxis("Mouse X");
    float hx = Input.GetAxis("Horizontal");
    float sw = Screen.width * .425f;
    if (hx < 0 || mx < 0) {
      if (acc > 0) acc -= Time.deltaTime * 100;
      else acc -= Time.deltaTime * 75;
    }
    if (hx > 0 || mx > 0) {
      if (acc < 0) acc += Time.deltaTime * 100;
      else acc += Time.deltaTime * 75;
    }
    if (acc > 50) acc = 50;
    if (acc < -50) acc = -50;

    Vector2 pos = ShipRT.anchoredPosition;
    pos.x += acc * 5 * Time.deltaTime;
    if (pos.x < -sw) { pos.x = -sw; acc = 0; }
    if (pos.x > sw) { pos.x = sw; acc = 0; }
    ShipRT.anchoredPosition = pos;


    // Background stars with very basic parallax
    ss[0].x = -pos.x * .1f;
    ss[1].x = -pos.x * .05f;
    ss[2].x = -pos.x * .025f;
    ss[0].y = ((time * .025f) % 1f) * (-495 - 1136) + 495;
    ss[1].y = ((time * .0125f) % 1f) * (-495 - 1136) + 495;
    ss[2].y = ((time * .0075f) % 1f) * (-495 - 1136) + 495;
    for (int i = 0; i < 3; i++)
      Stars[i].anchoredPosition = ss[i];


    // Meteors
    for (int i = 0; i < 8; i++) {
      if (ms[i].y == 670) {
        if (ms[i].z > 0) ms[i].z -= Time.deltaTime * i;
        else {
          ms[i].z = Random.Range(-.5f, .5f);
          ms[i].x = Random.Range(-800, 800);
          ms[i].y = 669.9f;
        }
      }
      ms[i].y -= Time.deltaTime * (i + 4) * (i + 4) * 3.5f * (1 + time * .025f);
      ms[i].x += ms[i].z * Time.deltaTime;
      Meteors[i].anchoredPosition = ms[i];
      if (ms[i].y < -670) {
        ms[i].x = Random.Range(-800, 800);
        ms[i].y = 670; // Restart
        ms[i].z = Random.Range(1, 3f);
        score++;
        Score.text = "Score: " + score;
        SoundExtra.clip = MeteorScoreSound;
        SoundExtra.Play();
      }
    }

    // Collisions

    if (!hit) {
      Collider2D coll = Physics2D.OverlapCircle(ShipRT.transform.position, 48);
      if (coll != null) {
        hit = true;
        StartCoroutine(ShipHit());
      }
    }


    // Score
    time += Time.deltaTime;
  }

  bool hit = false;

  void HighScores() {
    Score.text = "Game Over!";
  }


  IEnumerator ShipHit() {
    yield return null;
    shipAnim.Play("Ship Explosion");
    lives--;
    Hearts[2].enabled = lives > 2;
    Hearts[1].enabled = lives > 1;
    Hearts[0].enabled = lives > 0;
    if (lives == 0) {
      playing = false;
      yield break;
    }
    yield return new WaitForSeconds(2);
    shipAnim.Play("Ship Idle");
    ShipRT.anchoredPosition = new Vector2(0, -400);
    acc = 0;
    hit = false;
  }
}

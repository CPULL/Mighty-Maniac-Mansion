using UnityEngine;

public class Actor : MonoBehaviour {
  public Sprite[] facesF;
  public Sprite[] facesB;
  public Sprite[] facesL;
  public Sprite[] facesR;
  public SpriteRenderer Face;
  Animator anim;
  Vector3 destination = Vector2.zero;
  bool walking = false;
  Dir dir = Dir.F;

  private void Awake() {
    anim = GetComponent<Animator>();
  }

  public void WalkTo(Vector2 dest) {
    destination = dest;
    walking = true;
  }

  private void Update() {
    if (isSpeaking) {
      speakt += Time.deltaTime;
      if (speakt > .15f) {
        speakt = 0;
        faceNum = Random.Range(2, 5);
      }
    }

    switch (dir) {
      case Dir.F: Face.sprite = facesF[faceNum]; break;
      case Dir.B: Face.sprite = facesB[faceNum]; break;
      case Dir.L: Face.sprite = facesL[faceNum]; break;
      case Dir.R: Face.sprite = facesR[faceNum]; break;
    }
    if (!walking) {
      anim.Play("Idle" + dir);
      return;
    }

    Vector3 wdir = destination - transform.position;
    float speed = 5;
    if (Mathf.Abs(wdir.y) > Mathf.Abs(wdir.x) * 1.05f) {
      if (wdir.y > 0) dir = Dir.B;
      if (wdir.y < 0) dir = Dir.F;
    }
    else {
      if (wdir.x > 0) dir = Dir.R;
      if (wdir.x < 0) dir = Dir.L;
      speed = 7.5f;
    }

    anim.Play("Walk" + dir);
    wdir.z = 0;
    if (wdir.sqrMagnitude < .25f) {
      walking = false;
      return;
    }

    wdir.Normalize();
    transform.position += wdir * speed * Time.deltaTime;
  }

  bool isSpeaking = false;
  int faceNum = 0;
  float speakt = 0;


  public void Say(string message) {
    isSpeaking = true;
    faceNum = 0;
    speakt = 0;
    Balloon.Show(message, transform, CompleteSpeaking);
  }

  public void CompleteSpeaking() {
    isSpeaking = false;
    faceNum = 0;
  }
}


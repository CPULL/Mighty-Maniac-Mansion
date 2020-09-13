using UnityEngine;

public class Actor : MonoBehaviour {
  public Sprite[] facesF;
  public Sprite[] facesB;
  public Sprite[] facesL;
  public Sprite[] facesR;
  public SpriteRenderer Face;
  public Room currentRoom;
  Animator anim;
  Vector3 destination = Vector2.zero;
  System.Action callBack = null;
  bool walking = false;
  Dir dir = Dir.F;

  private void Awake() {
    anim = GetComponent<Animator>();
  }

  public void WalkTo(Vector3 dest, System.Action action = null) {
    callBack = action;
    destination = dest;
    destination.y = transform.position.y;
    walking = true;

    Vector3 wdir = destination - transform.position;
    float angle = Mathf.Atan2(transform.forward.z - wdir.z, transform.forward.x - wdir.x) * Mathf.Rad2Deg;
    if (60 < angle && angle < 120) { dir = Dir.F; }
    if (120 <= angle && angle <= 240) { dir = Dir.R; }
    if (240 < angle || angle < -60) { dir = Dir.B; }
    if (-60 <= angle && angle <= 60) { dir = Dir.L; }
    anim.Play("Walk" + dir);
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

    Vector3 wdir = (destination - transform.position).normalized;
    float dist = Vector3.Distance(transform.position, destination);
    float speed = Mathf.Clamp(dist, 2.5f, 5f);
    Vector3 np = transform.position + wdir * speed * Time.deltaTime;

    if (Vector3.Dot(np, destination) < 0 || dist < .15f) {
      transform.position = destination;
      walking = false;
      callBack?.Invoke();
      callBack = null;
      return;
    }
    transform.position = np;
  }

  bool isSpeaking = false;
  int faceNum = 0;
  float speakt = 0;
  GameAction fromAction = null;

  public void Say(string message, GameAction action = null) {
    isSpeaking = true;
    faceNum = 0;
    speakt = 0;
    fromAction = action;
    Balloon.Show(message, transform, CompleteSpeaking);
  }

  public void CompleteSpeaking() {
    isSpeaking = false;
    faceNum = 0;
    if (fromAction != null) fromAction.Complete();
  }

  internal void SetDirection(Dir d) {
    dir = d;
  }

  internal void SetExpression(Expression exp) {
    faceNum = (int)exp;
  }
}


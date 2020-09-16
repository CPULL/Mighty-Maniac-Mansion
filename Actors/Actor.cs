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
  Path path = null;
  Dir dir = Dir.F;
  private AudioSource audios;

  private void Awake() {
    anim = GetComponent<Animator>();
    audios = GetComponent<AudioSource>();
  }

  public void WalkStairsTo(Vector3 dest, Dir d, System.Action action = null) {
    callBack = action;
    destination = dest;
    walking = true;

    Vector3 wdir = destination - transform.position;
    if (wdir != Vector3.zero) {
      dir = d;
      anim.Play("Walk" + dir);
    }
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
    float speed = Mathf.Clamp(dist, Input.GetMouseButton(0) ? 5f : 2.5f, 5f);
    wdir.z = wdir.y;
    Vector3 np = transform.position + wdir * speed * Time.deltaTime;

    float ty = transform.position.y;
    float sh = path.maxY - path.minY;
    float th = path.maxSize - path.minSize;
    float scaley = (ty - path.minY) / sh;
    scaley = (1 - scaley) * path.minSize + scaley * path.maxSize;
    transform.localScale = new Vector3(scaley, scaley, 1);

    if (wdir.sqrMagnitude < .1f || dist < .15f) {
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
    if (d == Dir.None) return;
    dir = d;
  }

  internal void SetExpression(Expression exp) {
    faceNum = (int)exp;
  }

  internal bool IsWalking() {
    return walking;
  }

  void OnMouseEnter() {
    Controller.SendActorEventData(this, false);
  }

  void OnMouseDown() {
    Controller.SendActorEventData(this, true);
  }

  public void Stop() {
    isSpeaking = false;
    walking = false;
  }

  internal void WalkTo(Vector2 dest, Dir d, Path p, System.Action action = null) {
    callBack = action;
    destination = dest;
    destination.z = destination.y;
    path = p;
    dir = d;
    walking = true;

    Vector3 wdir = destination - transform.position;
    if (wdir != Vector3.zero) {
      anim.Play("Walk" + dir);
    }
  }

  internal void PlaySound(AudioClip audioClip) {
    audios.clip = audioClip;
    audios.Play();
  }
}


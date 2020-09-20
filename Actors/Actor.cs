using System.Collections.Generic;
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
  System.Action<Actor, Item> callBack = null;
  Item callBackItem = null;
  bool walking = false;
  Path path = null;
  Dir dir = Dir.F;
  private AudioSource audios;
  public List<Item> inventory;
  public Chars id;
  public List<Skill> skills;

  private void Awake() {
    anim = GetComponent<Animator>();
    audios = GetComponent<AudioSource>();
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
    float speed = Mathf.Clamp(dist, Input.GetMouseButton(0) ? 5f : 2.5f, 5f) * Controller.walkSpeed;
    wdir.z = wdir.y;
    Vector3 np = transform.position + wdir * speed * Time.deltaTime;

    float ty = transform.position.y;
    if (ty < path.minY) ty = path.minY;
    if (ty > path.maxY) ty = path.maxY;
    float scaley = -0.044f * ty + 0.4f;
    transform.localScale = new Vector3(scaley, scaley, 1);

    if (wdir.sqrMagnitude < .1f || dist < .15f) {
      transform.position = destination;
      walking = false;
      callBack?.Invoke(this, callBackItem);
      callBack = null;
      return;
    }
    transform.position = np;
  }

  internal string HasItem(ItemEnum item) {
    foreach (Item i in inventory)
      if (i.Item == item) return null;
    return "Missing " + item;
  }

  public string HasSkill(Skill skill) {
    foreach (Skill s in skills)
      if (s == skill) return null;
    switch(skill) {
      case Skill.Strenght: return "I am not strong enough!";
      case Skill.Courage: return "It is scary!\nI will not do it!";
      case Skill.Chef: return "I am not a chef";
      case Skill.Handyman: return "I don't know how to do it";
      case Skill.Geek: return "I am not a geek";
      case Skill.Nerd: return "I am not a nerd";
      case Skill.Music: return "I don't know how to play";
      case Skill.Writing: return "I am not a writer";
    }
    return "I cannot do it";
  }

  public void AddSkill(Skill skill) {
    foreach (Skill s in skills)
      if (s == skill) return;
    skills.Add(skill);
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

  internal void WalkTo(Vector2 dest, Dir d, Path p, System.Action<Actor, Item> action = null, Item item = null) {
    callBack = action;
    callBackItem = item;
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


﻿using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour {
  public Sprite[] facesF;
  public Sprite[] facesB;
  public Sprite[] facesL;
  public Sprite[] facesR;
  public SpriteRenderer Face;
  public SpriteRenderer Arms;
  public SpriteRenderer Legs;
  public Material Normal;
  public Material Outline;
  public Room currentRoom;
  public float actorSpeed = 4f;
  Animator anim;
  readonly Parcour3 destination = new Parcour3(Vector3.zero, null);
  System.Action<Actor, Item> callBack = null;
  Item callBackItem = null;
  bool walking = false;
  Dir dir = Dir.F;
  private AudioSource audios;
  bool isTentacle = false;
  public List<Item> inventory;
  public Chars id;
  public float mainScale = 1f;
  public List<Skill> skills;
  List<Parcour> parcour;
  [TextArea(3, 12)] public string Description;
  FloorType floor = FloorType.None;
  FloorType prevFloor = FloorType.None;
  string idle;
  string walk;
  bool IAmNPC = true;
  public AudioClip TentacleSteps;
  public List<Behavior> Behaviors;
  bool block = false;

  public void Player() {
    IAmNPC = false;
  }

  private void Awake() {
    anim = GetComponent<Animator>();
    audios = GetComponent<AudioSource>();

    isTentacle = id == Chars.GreenTentacle || id == Chars.PurpleTentacle || id == Chars.BlueTentacle;
    if (isTentacle) audios.clip = TentacleSteps;

    if (currentRoom != null) {
      Vector3 startpos = new Vector3((currentRoom.maxR + currentRoom.minL) / 2, (currentRoom.maxY + currentRoom.minY) / 2, 0);
      if (Vector3.Distance(transform.position, startpos) > 10)
        SetScaleAndPosition(startpos);
      else
        SetScaleAndPosition(transform.position);
    }

    idle = id.ToString() + " Idle";
    walk = id.ToString() + " Walk";
  }

  internal bool HasItem(ItemEnum item) {
    foreach (Item i in inventory)
      if (i.Item == item) return true;
    return false;
  }

  public string HasSkill(Skill skill) {
    foreach (Skill s in skills)
      if (s == skill) return null;
    switch (skill) {
      case Skill.Strenght: return "I am not strong enough!";
      case Skill.Courage: return "It is scary!\nI will not do it!";
      case Skill.Chef: return "I am not a chef";
      case Skill.Handyman: return "I don't know how to do it";
      case Skill.Geek: return "I am not a geek";
      case Skill.Nerd: return "I am not a nerd";
      case Skill.Music: return "I don't know how to play";
      case Skill.Writing: return "I am not a writer";
      case Skill.None:
        break;
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

  public bool Say(string message, GameAction action = null) {
    if (message == null) return true;
    isSpeaking = true;
    faceNum = 0;
    speakt = 0;
    fromAction = action;
    Balloon.Show(message, transform, CompleteSpeaking);
    return false;
  }

  public void CompleteSpeaking() {
    isSpeaking = false;
    faceNum = 0;
    if (fromAction != null) fromAction.Complete();
  }

  internal void SetDirection(Dir d) {
    if (d == Dir.None) return;
    if (d != Dir.L && d != Dir.R && d != Dir.F && d != Dir.B) {
      Debug.LogError("Found the culprit: " + d);
      return;
    }
    dir = d;
  }

  internal void SetExpression(Expression exp) {
    faceNum = (int)exp;
  }

  internal bool IsWalking() {
    return walking;
  }

  void OnMouseEnter() {
    Controller.OverActor(this);
    Face.material = Outline;
    Arms.material = Outline;
    Legs.material = Outline;
  }

  void OnMouseExit() {
    Controller.OverActor(null);
    Face.material = Normal;
    Arms.material = Normal;
    Legs.material = Normal;
  }

  public void Stop() {
    isSpeaking = false;
    walking = false;
  }

  Dir CalculateDirection(Vector3 point) {
    Vector3 ap = transform.position;
    Vector3 mp = point;

    float dx = ap.x - mp.x;
    float dy = ap.y - mp.y;

    // Vert or Horiz?
    if (Mathf.Abs(dx) * 1.2f < Mathf.Abs(dy)) {
      // Vert
      if (dy < 0) return Dir.B;
      return Dir.F;
    }
    else {
      // Horiz
      if (dx < 0) return Dir.R;
      return Dir.L;
    }
  }


  internal void WalkTo(Vector2 dest, PathNode p, System.Action<Actor, Item> action = null, Item item = null) {
    if (block) return;

    destination.pos = dest;
    destination.node = p;
    destination.pos.z = transform.position.z;
    Vector2 wdir = destination.pos - transform.position;
    float dist = Vector2.Distance(transform.position, dest);
    if (wdir.sqrMagnitude < .1f || dist < .15f) {
      Debug.Log("Too close to dest");
      action?.Invoke(this, item);
      return;
    }

    // Calculate the path
    parcour = p.parent.PathFind(transform.position, dest);
    if (parcour == null) {
      destination.pos = dest;
      prevFloor = FloorType.None;
      floor = p.floorType;
      if (!isTentacle) audios.clip = Sounds.GetStepSound(floor);
      audios.Play();
    }
    else {
      destination.pos = parcour[1].pos;
      destination.node = parcour[1].node;
      floor = parcour[1].node.floorType;
      parcour.RemoveRange(0, 2);

      if (floor != prevFloor || !audios.isPlaying) {
        prevFloor = floor;
        if (!isTentacle) audios.clip = Sounds.GetStepSound(floor);
        audios.Play();
      }
    }
    destination.pos.z = (destination.pos.y - currentRoom.CameraGround) / 10f;

    callBack = action;
    callBackItem = item;
    dir = CalculateDirection(destination.pos);
    anim.Play(walk + dir);
    walking = true;
  }

  float nextBehaviorCheck = .5f;
  Behavior currentBehavior = null;
  BehaviorAction currentAction = null;

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



    // Check if we have a behavior to play, but only every .5 seconds
    nextBehaviorCheck -= Time.deltaTime;
    if (IAmNPC && nextBehaviorCheck < 0) { // Behaviors checking
      nextBehaviorCheck = .25f;

      // Get the high priority behavior, if one is valid. Check every 1/4 of a second
      Behavior valid = null;
      foreach (Behavior b in Behaviors) {
        if (b.IsValid(this)) {
          valid = b;
          break;
        }
      }
      if (valid != null) {
        if (valid != currentBehavior) {
          Debug.Log(valid?.name + " <= " + currentBehavior?.name);
          currentBehavior = valid;
          currentAction = currentBehavior.GetNextAction(null);
          Debug.Log("Runnig behavior: " + currentBehavior.name);
        }
      }

      // Play the action, if any
      if (currentAction != null) {
        // Play it until completed, and the behavior is still valid
        if (currentAction.status == BehaviorActonStatus.NotStarted) {
          currentAction.status = BehaviorActonStatus.Running;
          PlayAction();
        }
        else if (currentAction.status == BehaviorActonStatus.Running) {
          PlayAction();
        }
        else if (currentAction.status == BehaviorActonStatus.WaitingToCompleteAsync) {
          // Just wait
        }
        else if (currentAction.status == BehaviorActonStatus.Completed) { // Completed, get next
          currentAction = currentBehavior.GetNextAction(currentAction);
          if (currentAction == null) {
            currentBehavior = null;
            nextBehaviorCheck = 0;
          }
          else
            currentAction.status = BehaviorActonStatus.NotStarted;
        }
      }
    }


    // Normal movement from here

    if (!walking || block) {
      if (dir == Dir.None) dir = Dir.F;
      anim.Play(idle + dir);
      if (audios.isPlaying) audios.Stop();
      return;
    }

    if (!audios.isPlaying) {
      audios.Play();
    }
    anim.speed = Controller.walkSpeed * .8f;
    Vector2 walkDir = (destination.pos - transform.position);
    Vector3 wdir = walkDir.normalized;
    wdir.y *= .65f;
    Vector3 np = transform.position + wdir * actorSpeed * Controller.walkSpeed * Time.deltaTime;
    np.z = 0;

    float ty = transform.position.y;
    if (ty < currentRoom.minY) ty = currentRoom.minY;
    if (ty > currentRoom.maxY) ty = currentRoom.maxY;
    float scaley = -.05f * (ty - currentRoom.minY - 1.9f) + .39f;
    if (!destination.node.isStair) {
      scaley *= currentRoom.scalePerc;
      transform.localScale = new Vector3(scaley, scaley, 1);
      int zpos = (int)(scaley * 10000);
      Face.sortingOrder = zpos + 1;
      Arms.sortingOrder = zpos + 2;
      Legs.sortingOrder = zpos;
    }

    if (walkDir.sqrMagnitude < .05f) {
      if (parcour == null || parcour.Count == 0) {
        transform.position = destination.pos;
        walking = false;
        callBack?.Invoke(this, callBackItem);
        callBack = null;
        audios.Stop();
        prevFloor = FloorType.None;
        return;
      }
      destination.pos = parcour[0].pos;
      destination.node = parcour[0].node;
      floor = parcour[0].node.floorType;
      if (floor != prevFloor) {
        if (!isTentacle) audios.clip = Sounds.GetStepSound(floor);
        audios.Play();
      }
      parcour.RemoveAt(0);
      dir = CalculateDirection(destination.pos);
      anim.Play(walk + dir);
    }
    transform.position = np;
  }

  private void PlayAction() {
    switch (currentAction.type) {
      case BehaviorActionType.Teleport:
        break;
      case BehaviorActionType.MoveToSpecificSpot: {
        // Get the pathnode of where we are (at least the closest one)
        PathNode p = currentRoom.GetPathNode(transform.position);
        if (p == null) {
          Debug.Log("BUMMER! no path node");
          currentAction.status = BehaviorActonStatus.Completed;
          return;
        }
        currentAction.status = BehaviorActonStatus.WaitingToCompleteAsync;
        WalkTo(currentAction.pos, p,
          new System.Action<Actor, Item>((actor, item) => {
            if (currentAction != null) currentAction.status = BehaviorActonStatus.Completed;
          }));
      }
      break;

      case BehaviorActionType.MoveToActor: {
        // Get the pathnode of where we are (at least the closest one)
        PathNode p = currentRoom.GetPathNode(transform.position);
        if (p == null) {
          Debug.Log("BUMMER! no path node");
          currentAction.status = BehaviorActonStatus.Completed;
          return;
        }
        Actor act = Controller.GetActor((Chars)currentAction.val1);
        if (act == null) {
          Debug.Log("No actor: " + (Chars)currentAction.val1);
          currentAction.status = BehaviorActonStatus.Completed;
          return;
        }
        Vector2 pos = act.transform.position;
        if (transform.position.x - pos.x < 0)
          pos.x -= 2f;
        else
          pos.x += 2f;
        currentAction.status = BehaviorActonStatus.WaitingToCompleteAsync;
        WalkTo(pos, p,
          new System.Action<Actor, Item>((actor, item) => {
            if (currentAction != null) currentAction.status = BehaviorActonStatus.Completed;
          }));
      }
      break;

      case BehaviorActionType.Speak: {
        Say(currentAction.str);
        currentAction.status = BehaviorActonStatus.Completed;
      }
      break;

      case BehaviorActionType.Ask:
        break;
      case BehaviorActionType.Expression:
        break;
      case BehaviorActionType.EnableDisable:
        break;
      case BehaviorActionType.OpenClose:
        break;
      case BehaviorActionType.LockUnlock:
        break;
      case BehaviorActionType.Sound:
        break;
      case BehaviorActionType.AnimActor:
        break;
      case BehaviorActionType.AnimItem:
        break;
      case BehaviorActionType.SetFlag:
        break;
      case BehaviorActionType.BlockActor: {
        if ((Chars)currentAction.val1 == Chars.Player) currentAction.val1 = (int)Chars.Current;
        Actor act = Controller.GetActor((Chars)currentAction.val1);
        if (act == null) {
          Debug.Log("No actor: " + (Chars)currentAction.val1);
          currentAction.status = BehaviorActonStatus.Completed;
          return;
        }
        act.block = (FlagValue)currentAction.val2 == FlagValue.Yes;
        currentAction.status = BehaviorActonStatus.Completed;
      }
      break;
    }
  }

  public void SetScaleAndPosition(Vector3 pos, PathNode p = null) {
    float ty = pos.y;
    if (ty < currentRoom.minY) ty = currentRoom.minY;
    if (ty > currentRoom.maxY) ty = currentRoom.maxY;
    if (p == null) {
      if (destination.node == null || !destination.node.isStair) {
        float scaley = -.05f * (ty - currentRoom.minY - 1.9f) + .39f;
        scaley *= currentRoom.scalePerc;
        transform.localScale = new Vector3(scaley, scaley, 1);
        int zpos = (int)(scaley * 10000);
        Face.sortingOrder = zpos + 1;
        Arms.sortingOrder = zpos + 2;
        Legs.sortingOrder = zpos;
      }
    }
    else if (!p.isStair) {
      float scaley = -.05f * (ty - currentRoom.minY - 1.9f) + .39f;
      scaley *= currentRoom.scalePerc;
      transform.localScale = new Vector3(scaley, scaley, 1);
      int zpos = (int)(scaley * 10000);
      Face.sortingOrder = zpos + 1;
      Arms.sortingOrder = zpos + 2;
      Legs.sortingOrder = zpos;
    }
    else {
      // Find the path going down recursively until we will find a non-stairs node.
      PathNode node = p;
      while (node != null && node.isStair)
        node = node.down;

      if (node == null) { // Check going up
        node = p;
        while (node != null && node.isStair)
          node = node.top;
      }

      if (node == null) { // Check going left
        node = p;
        while (node != null && node.isStair)
          node = node.left;
      }

      if (node == null) { // Check going right
        node = p;
        while (node != null && node.isStair)
          node = node.right;
      }

      if (node == null) {
        Debug.LogError("Cannot find a sub-non-stairs node!");
      }
      else {
        // Then get the top position and do the scaling with this Y coordinate
        float subty = (node.tl.y + node.tr.y) * .5f;
        float scaley = -.05f * (subty - currentRoom.minY - 1.9f) + .39f;
        scaley *= currentRoom.scalePerc;
        transform.localScale = new Vector3(scaley, scaley, 1);
        int zpos = (int)(scaley * 10000);
        Face.sortingOrder = zpos + 1;
        Arms.sortingOrder = zpos + 2;
        Legs.sortingOrder = zpos;
      }
    }
    pos.y = ty;
    transform.position = pos;
  }
}

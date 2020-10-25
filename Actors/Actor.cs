using System.Collections.Generic;
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
  float blockMinX = -float.MaxValue;
  float blockMaxX = float.MaxValue;

  public List<GameScene> behaviors;

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
  
  public string HasSkillOLD(Skill skill) {
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

  public bool HasSkill(Skill skill) {
    foreach (Skill s in skills)
      if (s == skill) return true;
    return false;
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

  internal void RestoreWalking() {
    walking = true;
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
    followed = null;
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

  Transform followed = null;
  Dir followSide = Dir.None;

  internal bool WalkTo(Transform destActor, Dir side, GameAction action) {
    followed = destActor;
    followSide = side;

    // Get the pathnode of where we are (at least the closest one)
    PathNode p = currentRoom.GetPathNode(destActor.position);
    if (p == null) {
      Debug.Log("BUMMER! no path node");
      return true;
    }
    if (p.isStair) return true; // Not following on stairs

    Vector2 pos = followed.position;
    if (side == Dir.L) pos.x -= 1.5f;
    if (side == Dir.R) pos.x += 1.5f;
    if (side == Dir.F) pos.y -= 1.5f;
    if (side == Dir.B) pos.y += 1.5f;
    WalkTo(pos, p,
      action == null ? 
        null 
      :
        new System.Action<Actor, Item>((actor, item) => {
          // Should we do something if we reach the destination?
          Debug.Log(this + " reached destination");
          action.Complete();
          followed = null;
          walking = false;
        }
      ));

    return false;
  }

  internal void WalkTo(Vector2 dest, PathNode p, System.Action<Actor, Item> action = null, Item item = null) {
    if (dest.x < blockMinX) dest.x = blockMinX;
    if (dest.x > blockMaxX) dest.x = blockMaxX;

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


    nextBehaviorCheck -= Time.deltaTime;
    if (IAmNPC && nextBehaviorCheck < 0) { // Behaviors checking
      nextBehaviorCheck = .25f;

      // Check if at least one of the behaviors is valid
      foreach(GameScene b in behaviors) {
        if (b.IsValid(this, null, null, null, When.Always)) {
          if (b.Run(this, null))
            nextBehaviorCheck = 0;
        }
      }
    }


    // Normal movement from here
    if (!walking) {
      if (dir == Dir.None) dir = Dir.F;
      anim.Play(idle + dir);
      if (audios.isPlaying) audios.Stop();
      return;
    }

    if (!audios.isPlaying && gameObject.activeSelf) {
      audios.Play();
    }
    anim.speed = Controller.walkSpeed * .8f;


    if (followed != null) {
      destination.pos = followed.position;
      if (followSide == Dir.L) destination.pos.x -= 1.5f;
      if (followSide == Dir.R) destination.pos.x += 1.5f;
      if (followSide == Dir.F) destination.pos.y -= 1.5f;
      if (followSide == Dir.B) destination.pos.y += 1.5f;
      dir = CalculateDirection(destination.pos);
    }

    Vector2 walkDir = (destination.pos - transform.position);
    Vector3 wdir = walkDir.normalized;
    wdir.y *= .65f;
    Vector3 np = transform.position + wdir * actorSpeed * Controller.walkSpeed * Time.deltaTime;
    np.z = 0;

    float ty = transform.position.y;
    if (ty < currentRoom.minY) ty = currentRoom.minY;
    if (ty > currentRoom.maxY) ty = currentRoom.maxY;
    float scaley = -.05f * (ty - currentRoom.minY - 1.9f) + .39f;
    if (destination.node != null && !destination.node.isStair) {
      scaley *= currentRoom.scalePerc;
      transform.localScale = new Vector3(scaley, scaley, 1);
      int zpos = (int)(scaley * 10000);
      Face.sortingOrder = zpos + 1;
      Arms.sortingOrder = zpos + 2;
      Legs.sortingOrder = zpos;
    }

    if (walkDir.sqrMagnitude < .05f) {
      if (callBack == null && followed != null) { // Persistent following
        audios.Stop();
        prevFloor = FloorType.None;

        if (followSide == Dir.L) anim.Play(idle + Dir.R);
        if (followSide == Dir.R) anim.Play(idle + Dir.L);
        if (followSide == Dir.F) anim.Play(idle + Dir.B);
        if (followSide == Dir.B) anim.Play(idle + Dir.F);
        PathNode p = currentRoom.GetPathNode(followed.position);
        if (p != null) {
          anim.Play(walk + dir);
          WalkTo(followed.position, p, null);
        }
        return;
      }
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
      if (floor != prevFloor && gameObject.activeSelf) {
        if (!isTentacle) audios.clip = Sounds.GetStepSound(floor);
        audios.Play();
      }
      parcour.RemoveAt(0);
      dir = CalculateDirection(destination.pos);
      anim.Play(walk + dir);
    }
    transform.position = np;
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

  internal void SetMinMaxX(float minx, float maxx) {
    blockMinX = minx;
    blockMaxX = maxx;
  }
}

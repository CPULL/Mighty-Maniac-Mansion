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
  public Room currentRoom;
  public float actorSpeed = 4f;
  Animator anim;
  readonly Parcour3 destination = new Parcour3(Vector3.zero, null);
  System.Action<Actor, Item> callBackWalk = null;
  Item callBackItemWalk = null;
  GameAction followAction = null;
  WalkingMode walking = WalkingMode.None;
  [HideInInspector] public Dir dir = Dir.F;
  private AudioSource audios;
  bool isTentacle = false;
  public List<Item> inventory;
  public Chars id;
  public float mainScale = 1f;
  public List<Skill> skills;
  public bool male;
  public bool faceFront = false;
  List<Parcour> parcour;
  [TextArea(3, 12)] public string Description;
  FloorType floor = FloorType.None;
  FloorType prevFloor = FloorType.None;
  string idle;
  string walk;
  [HideInInspector] public bool IAmNPC = true;
  public AudioClip TentacleSteps;
  float blockMinX = -float.MaxValue;
  float blockMaxX = float.MaxValue;
  public LightMode lightIsOn;
  string lastanim = null;

  public List<GameScene> behaviors;

  public Wearable Coat = null;
  public Animator CoatAnim;

  bool isSpeaking = false;
  int faceNum = 0;
  float speakt = 0;
  GameAction fromAction = null;
  float lastChangedDir = 0;
  Transform followed = null;
  Dir followSide = Dir.None;
  float nextBehaviorCheck = .5f;
  [HideInInspector] public bool dead = false;
  [HideInInspector] public bool IsVisible = false;
  public void SetVisible(bool v) {
    IsVisible = v;
    Face.enabled = v;
    Arms.enabled = v;
    Legs.enabled = v;
  }

  public void Player() {
    IAmNPC = false;
    dead = false;
  }

  private void Awake() {
    anim = GetComponent<Animator>();
    audios = GetComponent<AudioSource>();

    isTentacle = id == Chars.GreenTentacle || id == Chars.PurpleTentacle || id == Chars.BlueTentacle;
    if (isTentacle) audios.clip = TentacleSteps;
    lightIsOn = LightMode.On;

    if (currentRoom != null) {
      Vector3 startpos = new Vector3((currentRoom.maxR + currentRoom.minL) / 2, (currentRoom.maxY + currentRoom.minY) / 2, 0);
      if (Vector3.Distance(transform.position, startpos) > 10)
        SetScaleAndPosition(startpos);
      else
        SetScaleAndPosition(transform.position);
      lightIsOn = currentRoom.lights;
    }

    idle = id.ToString() + " Idle";
    walk = id.ToString() + " Walk";
  }

  private void Start() {
    SetLight(LightMode.On);
  }

  internal bool HasItem(ItemEnum item) {
    foreach (Item i in inventory)
      if (i.ID == item) return true;
    return false;
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


  public bool Say(string message, GameAction action = null) {
    if (string.IsNullOrEmpty(message)) return true;
    isSpeaking = true;
    faceNum = 0;
    speakt = 0;
    fromAction = action;
    string txt =
      message.Replace("\\n", "\n")
      .Replace("[a1]", GD.c.actor1.name)
      .Replace("[a2]", GD.c.actor2.name)
      .Replace("[a3]", GD.c.actor3.name)
      .Replace("[kn]", GD.c.kidnappedActor.name)
      .Replace("[ca]", GD.c.currentActor.name)
      .Replace("[hr1]", GD.c.actor1.male ? "him" : "her")
      .Replace("[hr2]", GD.c.actor2.male ? "him" : "her")
      .Replace("[hr3]", GD.c.actor3.male ? "him" : "her")
      .Replace("[hrk]", GD.c.kidnappedActor.male ? "him" : "her")
      .Replace("[hs1]", GD.c.actor1.male ? "he" : "she")
      .Replace("[hs2]", GD.c.actor2.male ? "he" : "she")
      .Replace("[hs3]", GD.c.actor3.male ? "he" : "she")
      .Replace("[hsk]", GD.c.kidnappedActor.male ? "he" : "she")
      .Replace("[Hs1]", GD.c.actor1.male ? "He" : "She")
      .Replace("[Hs2]", GD.c.actor2.male ? "He" : "She")
      .Replace("[Hs3]", GD.c.actor3.male ? "He" : "She")
      .Replace("[Hsk]", GD.c.kidnappedActor.male ? "He" : "She");
    Balloon.Show(txt, transform, CompleteSpeaking);
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
    UsedItemFront = (d == Dir.F || d == Dir.R);
  }

  internal void SetExpression(Expression exp) {
    faceNum = (int)exp;
  }

  internal bool IsWalking() {
    return walking != WalkingMode.None;
  }


  void OnMouseEnter() {
    if (Controller.NotItemUsed() || Options.IsActive() || Controller.OverActor(this)) return;
    Material m;
    if (lightIsOn == LightMode.External || (lightIsOn == LightMode.On && GD.globalLights)) m = GD.Outline();
    else if (GD.flashLight) m = GD.FlashLight();
    else m = GD.LightOffOutline();
    Face.material = m;
    Arms.material = m;
    Legs.material = m;
  }

  void OnMouseExit() {
    if (Options.IsActive()) return;
    Controller.OverActor(null);
    SetLight(lightIsOn);
  }

  public void SetLight(LightMode lights) {
    lightIsOn = lights;
    Material m;
    if (lightIsOn == LightMode.External || (lightIsOn == LightMode.On && GD.globalLights)) m = GD.Normal();
    else if (GD.flashLight) m = GD.FlashLight();
    else m = GD.LightOffOutline();
    Face.material = m;
    Arms.material = m;
    Legs.material = m;
  }

  public void Stop() {
    isSpeaking = false;
    followed = null;
    if (walking != WalkingMode.None) {
      walking = WalkingMode.None;
      if (callBackWalk != null) {
        callBackWalk.Invoke(this, callBackItemWalk);
        callBackWalk = null;
        callBackItemWalk = null;
      }
      if (followAction != null) {
        followAction.Complete();
        followAction = null;
      }
    }
  }

  Dir CalculateDirection(Vector3 point) {
    if (lastChangedDir < .3f) return dir;
    Vector3 ap = transform.position;
    Vector3 mp = point;

    float dx = ap.x - mp.x;
    float dy = ap.y - mp.y;

    if (Mathf.Abs(dx) < .0125f && Mathf.Abs(dy) < .0125f) return dir;

    // Vert or Horiz?
    float mult = .75f;
    if (dir == Dir.F || dir == Dir.B) mult = 1.25f;
    if (Mathf.Abs(dx) * mult < Mathf.Abs(dy)) { // Vert
      if (dy < 0) {
        if (dir != Dir.B) lastChangedDir = 0;
        return Dir.B;
      }
      else {
        if (dir != Dir.F) lastChangedDir = 0;
        return Dir.F;
      }
    }
    else { // Horiz
      if (dx < 0) {
        if (dir != Dir.R) lastChangedDir = 0;
        return Dir.R;
      }
      else {
        if (dir != Dir.L) lastChangedDir = 0;
        return Dir.L;
      }
    }
  }


  /// <summary>
  /// Follows the specified transform staying on the specified side.
  /// </summary>
  /// <param name="destActor">The Transform to follow</param>
  /// <param name="side">The side (Direction)</param>
  /// <param name="action">The action to stop when reached</param>
  internal void WalkToActor(Transform destActor, Dir side, GameAction action) {
    if (walking == WalkingMode.Follower && destActor == followed) return;

    // Get the pathnode of where we are (at least the closest one)
    PathNode p = currentRoom.GetPathNode(destActor.position);
    if (p == null || p.isStair) {  // Not following on stairs
      return;
    }

    Controller.RemovePressAction(this);
    followed = destActor;
    followSide = side;
    followAction = action;

    // Reset the normal walk if any
    if (callBackWalk != null) {
      callBackWalk.Invoke(this, callBackItemWalk);
      callBackWalk = null;
      callBackItemWalk = null;
    }

    Vector3 pos = followed.position; // this should be done at every update
    if (followSide == Dir.L) pos.x -= 1.5f;
    if (followSide == Dir.R) pos.x += 1.5f;
    if (followSide == Dir.F) pos.y -= 1.5f;
    if (followSide == Dir.B) pos.y += 1.5f;
    if (pos.x < blockMinX) pos.x = blockMinX;
    if (pos.x > blockMaxX) pos.x = blockMaxX;
    pos.z = transform.position.z;

    Vector2 walkDir = (pos - transform.position);
    if (walkDir.sqrMagnitude < .05f) {
      if (action != null) action.Complete();
      return;
    }

    CalculateInitialParcour(pos, p);

    dir = CalculateDirection(destination.pos);
    if (lastanim != walk + dir) {
      anim.Play(walk + dir);
      lastanim = walk + dir;
      if (CoatAnim != null) CoatAnim.enabled = false;
    }
    walking = WalkingMode.Follower;
  }

  /// <summary>
  /// Walks to a specific position and then stops
  /// </summary>
  /// <param name=""></param>
  /// <param name="item"></param>
  internal void WalkToPos(Vector2 dest, PathNode p, System.Action<Actor, Item> action = null, Item item = null) { // DESTINATION *******************************
    if (callBackWalk != null && walking != WalkingMode.None) return;

    destination.pos = dest;
    destination.node = p;
    destination.pos.z = transform.position.z;
    Vector2 wdir = destination.pos - transform.position;
    float dist = Vector2.Distance(transform.position, dest);
    if (wdir.sqrMagnitude < .01f || dist < .05f) {
      Debug.Log(name + " too close to dest");
      action?.Invoke(this, item);
      return;
    }

    if (followAction != null) {
      followAction.Complete();
      followAction = null;
    }

    Controller.RemovePressAction(this);
    callBackWalk = action;
    callBackItemWalk = item;

    if (dest.x < blockMinX) dest.x = blockMinX;
    if (dest.x > blockMaxX) dest.x = blockMaxX;
    CalculateInitialParcour(dest, p);

    dir = CalculateDirection(destination.pos);
    if (lastanim != walk + dir) {
      anim.Play(walk + dir);
      lastanim = walk + dir;
      if (CoatAnim != null) CoatAnim.enabled = false;
    }
    walking = WalkingMode.Position;
  }


  private void CalculateInitialParcour(Vector2 dest, PathNode p) {
    destination.pos = dest;
    destination.node = p;
    destination.pos.z = transform.position.z;
    Vector2 wdir = destination.pos - transform.position;
    float dist = Vector2.Distance(transform.position, dest);
    if (wdir.sqrMagnitude < .01f || dist < .05f) return;

    // Calculate the path
    parcour = p.parent.PathFind(transform.position, dest);
    if (parcour == null) {
      destination.pos = dest;
      prevFloor = FloorType.None;
      floor = p.floorType;
      if (!isTentacle) audios.clip = Sounds.GetStepSound(floor);
      if (IsVisible && !audios.isPlaying) audios.Play();
    }
    else {
      destination.pos = parcour[1].pos;
      destination.node = parcour[1].node;
      floor = parcour[1].node.floorType;
      parcour.RemoveRange(0, 2);

      if (floor != prevFloor || !audios.isPlaying) {
        prevFloor = floor;
        if (!isTentacle) audios.clip = Sounds.GetStepSound(floor);
        if (IsVisible && !audios.isPlaying) audios.Play();
      }
    }
    destination.pos.z = (destination.pos.y - currentRoom.CameraGround) / 10f;

    dir = CalculateDirection(destination.pos);
    if (lastanim != walk + dir) {
      anim.Play(walk + dir);
      lastanim = walk + dir;
      if (CoatAnim != null) CoatAnim.enabled = false;
    }
    walking = WalkingMode.Follower;
  }



  public void SetScaleAndPosition(Vector3 pos) {
    float y = pos.y;
    if (y < currentRoom.minY) y = currentRoom.minY;
    if (y > currentRoom.maxY) y = currentRoom.maxY;
    ScaleByPosition(y);
    transform.position = pos;
  }

  internal void SetMinMaxX(float minx, float maxx) {
    blockMinX = minx;
    blockMaxX = maxx;
  }

  private void Update() {
    IsVisible = currentRoom == GD.c.currentRoom && currentRoom != null;

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
    if (IAmNPC && nextBehaviorCheck < 0 && IsVisible) { // Behaviors checking
      nextBehaviorCheck = .25f;

      // If we have a cutscene playing with this actor do not play behaviors
      foreach (GameScene b in behaviors) {
        b.performer = this;
        if (b.Run()) {
          nextBehaviorCheck = 0;
          break;
        }
      }
    }

    lastChangedDir += Time.deltaTime;
    // Normal movement from here
    switch (walking) {
      case WalkingMode.None:
        if (dir == Dir.None) dir = Dir.F;
        if (IsVisible) {
          if (lastanim != idle + dir) {
            anim.Play(idle + dir);
            lastanim = idle + dir;
            if (CoatAnim != null) CoatAnim.enabled = false;
          }
          if (audios.isPlaying) audios.Stop();
        }
        return;
      case WalkingMode.Position: WalkPosition(); break;
      case WalkingMode.Follower: Follow(); break;
    }
  }

  void WalkPosition() {
    // Claculate the step
    Vector2 walkDir = (destination.pos - transform.position);
    Vector3 wdir = walkDir.normalized;
    wdir.y *= .65f;
    Vector3 np = transform.position + wdir * actorSpeed * 1.2f * Controller.walkSpeed * Time.deltaTime;
    np.z = 0;
    transform.position = np;
    ScaleByPosition(transform.position.y);

    if (!audios.isPlaying && gameObject.activeSelf && IsVisible && !audios.isPlaying) {
      audios.Play();
    }
    anim.speed = Controller.walkSpeed * .9f;

    // Check if we reached the destination
    CheckReachingDestination(walkDir);
  }

  void Follow() {
    WalkPosition();

    // Update position
    PathNode p = currentRoom.GetPathNode(followed.position);
    if (p == null || p.isStair) {
      walking = WalkingMode.None;
      if (followAction != null) {
        followAction.Complete();
        followAction = null;
      }
      return;
    }
    if (p != destination.node) { // Recalculate path
      Vector2 pos = followed.position; // this should be done at every update
      if (followSide == Dir.L) pos.x -= 1.5f;
      if (followSide == Dir.R) pos.x += 1.5f;
      if (followSide == Dir.F) pos.y -= 1.5f;
      if (followSide == Dir.B) pos.y += 1.5f;
      if (pos.x < blockMinX) pos.x = blockMinX;
      if (pos.x > blockMaxX) pos.x = blockMaxX;
      CalculateInitialParcour(pos, p);
    }
    else {
      destination.pos = followed.position;
      if (followSide == Dir.L) destination.pos.x -= 1.5f;
      if (followSide == Dir.R) destination.pos.x += 1.5f;
      if (followSide == Dir.F) destination.pos.y -= 1.5f;
      if (followSide == Dir.B) destination.pos.y += 1.5f;
    }
  }

  void ScaleByPosition(float y) {
    float ty = y;
    if (ty < currentRoom.minY) ty = currentRoom.minY;
    if (ty > currentRoom.maxY) ty = currentRoom.maxY;
    float scaley = -.05f * (ty - currentRoom.minY - 1.9f) + .39f;

    int zpos = (int)(scaley * 10000);
    scaley *= currentRoom.scalePerc * mainScale;
    transform.localScale = new Vector3(scaley, scaley, 1);
    if (faceFront || (!male && dir != Dir.F)) {
      Face.sortingOrder = zpos + 2;
      Arms.sortingOrder = zpos + 1;
    }
    else {
      Face.sortingOrder = zpos + 1;
      Arms.sortingOrder = zpos + 2;
    }
    Legs.sortingOrder = zpos;
    if (Coat != null && Coat.gameObject.activeSelf) Coat.SetSortingOrder(zpos);
    if (UsedItemFront)
      UsedItemSR.sortingOrder = zpos + 4;
    else
      UsedItemSR.sortingOrder = zpos - 2;
  }

  void CheckReachingDestination(Vector2 walkDir) {
    if (walking == WalkingMode.None) return;

    if (walkDir != Vector2.zero && IsVisible) {
      // Play walk anim
      if (lastanim != walk + dir) {
        anim.Play(walk + dir);
        lastanim = walk + dir;
        if (CoatAnim != null) CoatAnim.enabled = false;
      }
    }

    // Check if we reached the destination
    if (walkDir.sqrMagnitude < .05f) {
      // Do we still have a path to follow?
      if (parcour == null || parcour.Count == 0) {
        // OK, no more steps

        if (walking == WalkingMode.Position) { // Just stop and call the callback
          if (callBackWalk != null) {
            callBackWalk.Invoke(this, callBackItemWalk);
            callBackWalk = null;
            callBackItemWalk = null;
          }
          if (walking != WalkingMode.None) {
            transform.position = destination.pos;
          }
          walking = WalkingMode.None;
          audios.Stop();
          prevFloor = FloorType.None;

        }
        else if (walking == WalkingMode.Follower) {
          // Do we have a callback?
          if (followAction != null) { //  Yes -> call it and stop
            followAction.Complete();
            followAction = null;

            destination.pos = followed.position;
            if (followSide == Dir.L) { destination.pos.x -= 1.5f; dir = Dir.R; }
            if (followSide == Dir.R) { destination.pos.x += 1.5f; dir = Dir.L; }
            if (followSide == Dir.F) { destination.pos.y -= 1.5f; dir = Dir.B; }
            if (followSide == Dir.B) { destination.pos.y += 1.5f; dir = Dir.F; }
            if (walking != WalkingMode.None) {
              transform.position = destination.pos;
            }
            walking = WalkingMode.None;
            audios.Stop();
            prevFloor = FloorType.None;
            return;
          }

          //  No -> continue by recalculating the path
          Vector2 pos = followed.position; // this should be done at every update
          if (followSide == Dir.L) pos.x -= 1.5f;
          if (followSide == Dir.R) pos.x += 1.5f;
          if (followSide == Dir.F) pos.y -= 1.5f;
          if (followSide == Dir.B) pos.y += 1.5f;
          if (pos.x < blockMinX) pos.x = blockMinX;
          if (pos.x > blockMaxX) pos.x = blockMaxX;

          PathNode p = currentRoom.GetPathNode(followed.position);
          if (p == null || p.isStair) {  // Not following on stairs
            if (followAction != null) {
              followAction.Complete();
              followAction = null;
            }
            walking = WalkingMode.None;
            audios.Stop();
            prevFloor = FloorType.None;
            return;
          }
          CalculateInitialParcour(pos, p);
        }
      }
      else { // Get next part of parcour
        destination.pos = parcour[0].pos;
        destination.node = parcour[0].node;
        floor = parcour[0].node.floorType;
        if (floor != prevFloor && gameObject.activeSelf && IsVisible && !audios.isPlaying) {
          if (!isTentacle) audios.clip = Sounds.GetStepSound(floor);
          audios.Play();
        }
        parcour.RemoveAt(0);
        dir = CalculateDirection(destination.pos);
      }
    }
  }

  internal void Wear(ItemEnum itemId, bool remove = false) {
    if (!HasItem(itemId)) {
      Say("I do not have " + itemId);
      return;
    }

    if (itemId == ItemEnum.Coat && Coat == null) {
      Debug.LogError("Missing coat for " + name);
      return;
    }

    if (itemId == ItemEnum.Coat) {
      if (remove) {
        Coat.gameObject.SetActive(false);
      }
      else {
        Coat.gameObject.SetActive(!Coat.gameObject.activeSelf);
        SetScaleAndPosition(transform.position);
      }
    }
  }


  private string animToPlay = null;
  private System.DateTime animStartTime;
  private float timeForAnim;
  private Dir animDir;
  public bool PlayAnim(string animName, Dir d, float timer) {
    if (gameObject.activeInHierarchy) {
      animToPlay = null;
      anim.enabled = true;
      if (string.IsNullOrEmpty(animName)) { // Stop the anims
        anim.StopPlayback();
        lastanim = null;
        if (Coat.gameObject.activeSelf && CoatAnim != null) {
          CoatAnim.StopPlayback();
          CoatAnim.enabled = false;
        }
        return true;
      }

      SetDirection(d);
      SetScaleAndPosition(transform.position);
      anim.Play(id.ToString() + " " + animName.Trim(), -1, 0);
      if (Coat.gameObject.activeSelf) {
        if (CoatAnim != null) CoatAnim.enabled = true;
        CoatAnim.speed = anim.speed;
        CoatAnim.Play("Coat " + animName.Trim(), -1, 0);
      }
    }
    else {
      if (string.IsNullOrEmpty(animName)) { // Stop the anims
        animToPlay = null;
        animDir = d;
        animStartTime = System.DateTime.Now;
        timeForAnim = 0;
        anim.StopPlayback();
        lastanim = null;
        if (Coat.gameObject.activeSelf && CoatAnim != null) {
          CoatAnim.StopPlayback();
          CoatAnim.enabled = false;
        }
        return true;
      }

      animToPlay = animName.Trim();
      animStartTime = System.DateTime.Now;
      timeForAnim = timer;
    }
    return timer == 0;
  }

  private void OnEnable() {
    if (animToPlay == null) return;
    System.TimeSpan elapsed = System.DateTime.Now - animStartTime;
    if (elapsed.TotalSeconds > timeForAnim) {
      animToPlay = null;
      return;
    }
    SetDirection(animDir);
    SetScaleAndPosition(transform.position);
    anim.Play(id.ToString() + " " + animToPlay, -1, (float)elapsed.TotalSeconds / timeForAnim);
    if (Coat.gameObject.activeSelf) {
      if (CoatAnim != null) CoatAnim.enabled = true;
      CoatAnim.speed = anim.speed;
      CoatAnim.Play("Coat " + animToPlay, -1, (float)elapsed.TotalSeconds / timeForAnim);
    }
  }

  public SpriteRenderer UsedItemSR;
  bool UsedItemFront = true;

  /// <summary>
  /// Uses an item
  /// </summary>
  /// <param name="id"></param>
  /// <param name="wear">
  /// 0 = Remove
  /// 1 = use but only if owned, and put on front
  /// 2 = use but only if owned, and put on back
  /// 3 = use in all cases, and put on front
  /// 4 = use in all cases, and put on back
  /// </param>
  internal void WearItem(ItemEnum id, int wear) {
    if (wear == 0) {
      UsedItemSR.enabled = false;
      return;
    }

    if ((wear == 1 || wear == 2) && !HasItem(id)) return; // The item is not here

    // Get the item
    Item item = AllObjects.GetItem(id);
    UsedItemSR.enabled = true;
    UsedItemSR.sprite = item.GetComponent<SpriteRenderer>().sprite;
    UsedItemFront = wear == 1 || wear == 3;
  }

  internal void SetZPos(float z, Dir d) {
    dir = d;
    int zpos = (int)z;
    if (faceFront || (!male && dir != Dir.F)) {
      Face.sortingOrder = zpos + 2;
      Arms.sortingOrder = zpos + 1;
    }
    else {
      Face.sortingOrder = zpos + 1;
      Arms.sortingOrder = zpos + 2;
    }
    Legs.sortingOrder = zpos;
    if (Coat != null && Coat.gameObject.activeSelf) Coat.SetSortingOrder(zpos);
    if (UsedItemFront)
      UsedItemSR.sortingOrder = zpos + 4;
    else
      UsedItemSR.sortingOrder = zpos - 2;
  }
}

public enum WalkingMode {
  None, Position, Follower
}

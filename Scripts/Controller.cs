using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using SimpleJSON;

public class Controller : MonoBehaviour {
  private static Controller c;
  public Texture2D[] Cursors;
  float cursorTime = 0;
  Vector2 center32 = new Vector2(32, 32);
  Camera cam;

  public PortraitClickHandler ActorPortrait1;
  public PortraitClickHandler ActorPortrait2;
  public PortraitClickHandler ActorPortrait3;
  public PortraitClickHandler InventoryPortrait;
  public GameObject Inventory;
  public GameObject InventoryItemTemplate;
  public AllObjects allObjects;

  public Actor[] actors;
  Actor actor1;

  internal static bool WeHaveActorPlaying(Chars actor) {
    return actor == c.actor1.id || actor == c.actor2.id || actor == c.actor3.id;
  }

  internal static Actor GetActor(Chars actor) {
    switch (actor) {
      case Chars.Current: return c.currentActor;
      case Chars.Actor1: return c.actor1;
      case Chars.Actor2: return c.actor2;
      case Chars.Actor3: return c.actor3;
      case Chars.KidnappedActor: return c.kidnappedActor;
    }
    return null;
  }

  Actor actor2;
  Actor actor3;
  readonly Actor kidnappedActor;

  Actor currentActor = null;
  Room currentRoom;
  Color32 unselectedActor = new Color32(0x6D, 0x7D, 0x7C, 255);
  Color32 selectedActor = new Color32(200, 232, 152, 255);

  GameStatus status = GameStatus.IntroDialogue;
  public AudioClip[] Sounds;
  public Options options;

  public static float walkSpeed;
  public static float textSpeed;

  private void Awake() {
    c = this;
    cam = Camera.main;

    actor1 = actors[(int)Chars.Dave];
    actor2 = actors[(int)Chars.Bernard];
    actor3 = actors[(int)Chars.Wendy];

    LoadSequences();
    PickValidSequence();


    StartCoroutine(StartDelayed());
    Cursor.SetCursor(Cursors[(int)CursorTypes.Wait], center32, CursorMode.Auto);
    status = GameStatus.IntroDialogue;
  }

  private void Start() {
    currentRoom = allObjects.roomsList[0];
    currentActor = actor1;
    ActorPortrait1.GetComponent<UnityEngine.UI.RawImage>().color = c.selectedActor;

    float vol = 40f * PlayerPrefs.GetFloat("MasterVolume", 1) - 40;
    options.mixerMusic.SetFloat("MasterVolume", vol);

    vol = 10 * Mathf.Log(1 + PlayerPrefs.GetFloat("MusicVolume", 1) * .74f) * 14.425f - 80;
    options.mixerMusic.SetFloat("MusicVolume", vol);

    vol = 10 * Mathf.Log(1 + PlayerPrefs.GetFloat("SoundsVolume", 1) * .74f) * 14.425f - 80;
    options.mixerMusic.SetFloat("SoundsVolume", vol);

    vol = 10 * Mathf.Log(1 + PlayerPrefs.GetFloat("BackSoundsVolume", 1) * .74f) * 14.425f - 80;
    options.mixerMusic.SetFloat("BackSoundsVolume", vol);

    walkSpeed = PlayerPrefs.GetFloat("WalkSpeed", 1);
    textSpeed = PlayerPrefs.GetFloat("TalkSpeed", 1);
  }

  IEnumerator StartDelayed() {
    yield return new WaitForSeconds(.5f);
    ShowName(currentRoom.RoomName);
  }

  public LayerMask pathLayer;
  public UnityEngine.UI.Image BlackFade;

  void Update() {
    cursorTime += Time.deltaTime;
    HandleCursor();

    // Handling of text messages
    if (textMsgTime > 0) {
      textMsgTime -= Time.deltaTime;
      switch (txtMsgMode) {
        case TextMsgMode.Appearing:
          TextMsgRT.sizeDelta = new Vector2(textSizeX * 4 * (.25f - textMsgTime), 50);
          TextBackRT.sizeDelta = TextMsgRT.sizeDelta;
          break;
        case TextMsgMode.Disappearing:
          TextMsgRT.sizeDelta = new Vector2(textSizeX * 4 * (textMsgTime), 50);
          TextBackRT.sizeDelta = TextMsgRT.sizeDelta;
          break;
      }
      if (textMsgTime <= 0) {
        switch (txtMsgMode) {
          case TextMsgMode.Appearing:
            txtMsgMode = TextMsgMode.Visible;
            textMsgTime = 3 * textSpeed;
            break;
          case TextMsgMode.Disappearing:
            txtMsgMode = TextMsgMode.None;
            TextMsg.text = "";
            break;
          case TextMsgMode.Visible:
            txtMsgMode = TextMsgMode.Disappearing;
            textMsgTime = .25f;
            break;
          case TextMsgMode.None:
            break;
        }
      }
    }

    // Sequences of actions
    if (currentSequence != null) { // Do we have a sequence?
      if (currentAction == null) { // Do we have the action?
        currentAction = currentSequence.GetNextAction();
        if (currentAction == null) {
          currentSequence = null;
          if (actions.Count > 0) currentAction = actions.GetFirst();
        }
      }
    }
    else if (actions.Count > 0) currentAction = actions.GetFirst();

    if (currentAction != null) {
      PlayCurrentAction();
      return;
    }

    // Handle camera
    Vector2 cpos = cam.WorldToScreenPoint(currentActor.transform.position);
    if (cam.transform.position.x < currentRoom.minL) {
      Vector3 p = cam.transform.position;
      p.x = currentRoom.minL;
      cam.transform.position = p;
    }
    else if (cam.transform.position.x > currentRoom.maxR) {
      Vector3 p = cam.transform.position;
      p.x = currentRoom.maxR;
      cam.transform.position = p;
    }
    else if (cpos.x < .3f * Screen.width) {
      if (cam.transform.position.x > currentRoom.minL) {
        cam.transform.position -= cam.transform.right * Time.deltaTime * (.3f * Screen.width - cpos.x) / 10;
      }
    }
    else if (cpos.x > .7f * Screen.width) {
      if (cam.transform.position.x < currentRoom.maxR) {
        cam.transform.position += cam.transform.right * Time.deltaTime * (cpos.x - .7f * Screen.width) / 10;
      }
    }



    if (c.status != GameStatus.NormalGamePlay) return;

    // LMB -> Walk or secondary action
    // RMB -> Default action

    /* FIXME
     * If we are over an object, do the action defined for the item with the mouse click
     * If we are not over an object, just walk with leftMB
     * 
     */


    // Mouse control
    bool lmb = Input.GetMouseButtonDown(0);
    bool rmb = Input.GetMouseButtonDown(1);

    if (overItem != null) {
      if ((overItem.whatItDoesR == WhatItDoes.Read && rmb) || (overItem.whatItDoesL == WhatItDoes.Read && lmb)) {
        WalkAndAction(currentActor, overItem,
          new System.Action<Actor, Item>((actor, item) => {
            actor.SetDirection(item.dir);
            actor.Say(item.Description);
          }));
      }

      else if ((overItem.whatItDoesR == WhatItDoes.Use && rmb) || (overItem.whatItDoesL == WhatItDoes.Use && lmb)) {
        WalkAndAction(currentActor, overItem,
          new System.Action<Actor, Item>((actor, item) => {
            actor.SetDirection(item.dir);
            string res = item.Use(currentActor);
            if (res != null)
              actor.Say(res);
            else {
              usedItem = item;
            }
          }));
      }

      else if ((overItem.whatItDoesR == WhatItDoes.Pick && rmb) || (overItem.whatItDoesL == WhatItDoes.Pick && lmb)) {
        WalkAndAction(currentActor, overItem,
          new System.Action<Actor, Item>((actor, item) => {
            ShowName(currentActor + " got " + item.Name);
            actor.inventory.Add(item);
            item.transform.parent = null;
            item.gameObject.SetActive(false);
            item.owner = Chars.None;
            if (actor == actor1) item.owner = Chars.Actor1;
            else if (actor == actor2) item.owner = Chars.Actor2;
            else if (actor == actor3) item.owner = Chars.Actor3;
            item.PlayActions(currentActor);
            item = null;
            forcedCursor = CursorTypes.None;
            if (Inventory.activeSelf) ActivateInventory(currentActor);
          }));
      }

      else if ((overItem.whatItDoesR == WhatItDoes.Walk && rmb) || (overItem is Door && overItem.whatItDoesL == WhatItDoes.Walk && lmb)) {
        WalkAndAction(currentActor, overItem,
          new System.Action<Actor, Item>((actor, item) => {
            if (item.Usable == Tstatus.OpenableLocked) {
              actor.Say("Is locked");
              return;
            }
            StartCoroutine(ChangeRoom(actor, (item as Door)));
          }));
      }

      else if (overItem.whatItDoesL == WhatItDoes.Walk && lmb) {
        WalkAndAction(currentActor, overItem, null);

      }

    }
    else if (lmb && !currentActor.IsWalking()) {
      RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), cam.transform.forward, 10000, pathLayer);
      if (hit.collider != null) {
        Path p = hit.collider.GetComponent<Path>();
        // FIXME We need to check if we may need to follow a series of paths (A*-ish?)
        currentActor.WalkTo(hit.point, CalculateDirection(hit.point), p);
      }
    }
    else if (Input.GetMouseButton(0) && currentActor.IsWalking()) {
      RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), cam.transform.forward, 10000, pathLayer);
      if (hit.collider != null) {
        Path p = hit.collider.GetComponent<Path>();
        currentActor.WalkTo(hit.point, CalculateDirection(hit.point), p);
      }
    }
  }



  Dir CalculateDirection(Vector3 point) {
    Vector3 ap = cam.WorldToScreenPoint(currentActor.transform.position);
    Vector3 mp = cam.WorldToScreenPoint(point);

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




  void WalkAndAction(Actor actor, Item item, System.Action<Actor, Item> action) {
    Vector3 one = actor.transform.position;
    one.z = 0;
    Vector3 two = item.HotSpot;
    two.z = 0;
    float dist = Vector3.Distance(one, two);
    if (dist > .2f) { // Need to walk
      RaycastHit2D hit = Physics2D.Raycast(overItem.HotSpot, cam.transform.forward, 10000, pathLayer);
      if (hit.collider != null) {
        Path p = hit.collider.GetComponent<Path>();
        currentActor.WalkTo(overItem.HotSpot, CalculateDirection(overItem.HotSpot), p, action, item);
      }
      return;
    }
    else {
      action?.Invoke(currentActor, item);
    }
  }

  private CursorTypes forcedCursor = CursorTypes.None;
  private Item overItem = null;
  private Item usedItem = null;
  private Texture2D oldCursor = null;
  void HandleCursor() {
    if (c.status != GameStatus.NormalGamePlay) return;

    if (forcedCursor == CursorTypes.None) {
      if (0 <= cursorTime && cursorTime <= .5f) {
        if (oldCursor != Cursors[0]) {
          Cursor.SetCursor(Cursors[0], center32, CursorMode.Auto);
          oldCursor = Cursors[0];
        }
      }
      else if (.5f < cursorTime && cursorTime <= .75f) {
        if (oldCursor != Cursors[1]) {
          Cursor.SetCursor(Cursors[1], center32, CursorMode.Auto);
          oldCursor = Cursors[1];
        }
      }
      else if (.75f < cursorTime && cursorTime <= .9f) {
        if (oldCursor != Cursors[2]) {
          Cursor.SetCursor(Cursors[2], center32, CursorMode.Auto);
          oldCursor = Cursors[2];
        }
      }
      else if (.9f < cursorTime && cursorTime <= 1.05f) {
        if (oldCursor != Cursors[1]) {
          Cursor.SetCursor(Cursors[1], center32, CursorMode.Auto);
          oldCursor = Cursors[1];
        }
      }
      else {
        cursorTime = 0;
        if (oldCursor != Cursors[0]) {
          Cursor.SetCursor(Cursors[0], center32, CursorMode.Auto);
          oldCursor = Cursors[0];
        }
      }
      return;
    }


    if (oldCursor != Cursors[(int)forcedCursor]) {
      Cursor.SetCursor(Cursors[(int)forcedCursor], center32, CursorMode.Auto);
      oldCursor = Cursors[(int)forcedCursor];
    }
  }



  public TextMeshProUGUI TextMsg;
  public RectTransform TextMsgRT;
  public RectTransform TextBackRT;
  TextMsgMode txtMsgMode = TextMsgMode.None;
  float textMsgTime = 0;
  float textSizeX = 0;

  void ShowName(string name) {
    if (TextMsg.text == name) return;
    textSizeX = TextMsg.GetPreferredValues(name).x;
    TextMsgRT.sizeDelta = new Vector2(0, 50);
    TextBackRT.sizeDelta = TextMsgRT.sizeDelta;
    TextMsg.text = name;
    textMsgTime = .25f;
    txtMsgMode = TextMsgMode.Appearing;
  }

  void HideName() {
    if (txtMsgMode == TextMsgMode.Disappearing || txtMsgMode == TextMsgMode.None) return;
    textMsgTime = .25f;
    txtMsgMode = TextMsgMode.Disappearing;
  }


  internal static void SendEventData(IPointerClickHandler handler) {
    if (c.status != GameStatus.NormalGamePlay) return;
    PortraitClickHandler h = (PortraitClickHandler)handler;
    if (h == c.ActorPortrait1) {
      SendActorEventData(c.actor1, true);
    } else if (h == c.ActorPortrait2) {
      SendActorEventData(c.actor2, true);
    } else if (h == c.ActorPortrait3) {
      SendActorEventData(c.actor3, true);
    } else if (h == c.InventoryPortrait) {
      if (c.Inventory.activeSelf) { // Show/Hide inventory of current actor
        c.Inventory.SetActive(false);
        c.InventoryPortrait.GetComponent<UnityEngine.UI.RawImage>().color = new Color32(0x6D, 0x7D, 0x7C, 0xff);
        return;
      }
      else
        c.ActivateInventory(c.currentActor);
    }
  }

  private void ActivateInventory(Actor actor) {
    Inventory.SetActive(true);
    InventoryPortrait.GetComponent<UnityEngine.UI.RawImage>().color = new Color32(0x7D, 0x8D, 0xfC, 0xff);
    foreach (Transform t in Inventory.transform)
      GameObject.Destroy(t.gameObject);

    foreach(Item item in  actor.inventory) {
      GameObject ii = Instantiate(InventoryItemTemplate, Inventory.transform);
      ii.gameObject.SetActive(true);
      InventoryItem it = ii.GetComponent<InventoryItem>();
      it.text.text = item.Name;
      it.front.sprite = item.iconImage;
      it.item = item;
    }
  }

  internal static void SendActorEventData(Actor actor, bool click) {
    if (c.status != GameStatus.NormalGamePlay || !click) return;
    c.currentActor = actor;

    c.ActorPortrait1.GetComponent<UnityEngine.UI.RawImage>().color = c.unselectedActor;
    c.ActorPortrait2.GetComponent<UnityEngine.UI.RawImage>().color = c.unselectedActor;
    c.ActorPortrait3.GetComponent<UnityEngine.UI.RawImage>().color = c.unselectedActor;
    if (actor == c.actor1) {
      c.ActorPortrait1.GetComponent<UnityEngine.UI.RawImage>().color = c.selectedActor;
    }
    else if (actor == c.actor2) {
      c.ActorPortrait2.GetComponent<UnityEngine.UI.RawImage>().color = c.selectedActor;
    }
    if (actor == c.actor3) {
      c.ActorPortrait3.GetComponent<UnityEngine.UI.RawImage>().color = c.selectedActor;
    }
    c.ShowName("Selected: " + c.currentActor.name);
    if (!c.currentActor.gameObject.activeSelf) { // Different room
      c.StartCoroutine(c.FadeToRoomActor());
    }
    if (c.Inventory.activeSelf) c.ActivateInventory(c.currentActor);
  }

  internal static void SetItem(Item item) {
    if (c.status != GameStatus.NormalGamePlay) return;
    if (item == null) {
      c.forcedCursor = CursorTypes.None;
      c.overItem = null;
      if (c.TextMsg.text != "") c.HideName();
      return;
    }

    // Right
    if (item.whatItDoesR == WhatItDoes.Walk) {
      c.forcedCursor = CursorTypes.None;
      c.overItem = item;
    }
    else if (item.whatItDoesR == WhatItDoes.Pick) {
      c.forcedCursor = CursorTypes.PickUp;
      c.overItem = item;
      c.ShowName(item.Name);
    }
    else if (item.whatItDoesR == WhatItDoes.Use) {
      c.forcedCursor = CursorTypes.Use;
      c.overItem = item;
      c.ShowName(item.Name);
    }
    else if (item.whatItDoesR == WhatItDoes.Read) {
      c.forcedCursor = CursorTypes.Examine;
      c.overItem = item;
      c.ShowName(item.Name);
    }
    // Left
    else if (item.whatItDoesL == WhatItDoes.Walk) {
      c.forcedCursor = CursorTypes.None;
      c.overItem = item;
    }
    else if (item.whatItDoesL == WhatItDoes.Pick) {
      c.forcedCursor = CursorTypes.PickUp;
      c.overItem = item;
      c.ShowName(item.Name);
    }
    else if (item.whatItDoesL == WhatItDoes.Use) {
      c.forcedCursor = CursorTypes.Use;
      c.overItem = item;
      c.ShowName(item.Name);
    }
    else if (item.whatItDoesL == WhatItDoes.Read) {
      c.forcedCursor = CursorTypes.Examine;
      c.overItem = item;
      c.ShowName(item.Name);
    }
  }

  GameSequence currentSequence;
  GameAction currentAction;
  public List<GameSequence> sequences;
  readonly SList<GameAction> actions = new SList<GameAction>(16);

  void LoadSequences() {
    string path = Application.dataPath + "/Actions/";

    foreach(string file in System.IO.Directory.GetFiles(path, "*.json")) {
//FIXME Debug.Log(file);
      string json = System.IO.File.ReadAllText(file);

      try {
        JSONNode j = JSON.Parse(json);

        GameSequence seq = new GameSequence(j["id"].Value, j["name"].Value);
        // FIXME conditions
        // Actions
        JSONNode.ValueEnumerator vals = j["actions"].Values;
        foreach (JSONNode val in vals) {
          GameAction a = new GameAction(val["type"].Value);
          if (a.type == ActionType.Teleport) {
            a.SetActor(val["actor"].Value);
            a.SetPos(val["pos"][0].AsFloat, val["pos"][1].AsFloat);
            a.SetDir(val["dir"].Value);
          }
          else if (a.type == ActionType.Speak) {
            a.SetActor(val["actor"].Value);
            a.SetDir(val["dir"].Value);
            a.SetValue(val["msg"].Value);
          }
          else if (a.type == ActionType.Expression) {
            a.SetActor(val["actor"].Value);
            a.SetDir(val["dir"].Value);
            a.SetValue(val["expr"].Value);
          }
          else if (a.type == ActionType.ShowRoom) {
            a.SetPos(val["pos"][0].AsFloat, val["pos"][1].AsFloat);
            a.SetValue(val["id"].Value);
          }
          else if (a.type == ActionType.Sound) {
            a.SetActor(val["actor"].Value);
            a.SetDir(val["dir"].Value);
            a.SetValue(val["snd"].Value);
          }
          a.SetWait(val["wait"].AsFloat);
          seq.actions.Add(a);
        }
        sequences.Add(seq);

      } catch (System.Exception e) {
        Debug.Log("ERROR (" + file + "): " + e.Message);
      }
    }

    currentSequence = null;
    foreach(GameSequence s in sequences) {
      if (s.id == "intro") {
        currentSequence = s;
        break;
      }
    }
  }

  void PickValidSequence() {
    return; // FIXME we should check if there are sequences that can be activated
    // Check all conditions and pick a valid sequence. In case there are more pick one random
  }


  public static void AddAction(GameAction a) {
    c.actions.Add(a);
  }

  void PlayCurrentAction() {
    if (currentAction.NotStarted()) {
      // FIXME        Debug.Log(currentAction.ToString());

      if (currentAction.type == ActionType.ShowRoom) {
        currentRoom = allObjects.GetRoom(currentAction.strValue);
        Vector3 pos = currentAction.pos;
        pos.z = -10;
        cam.transform.position = pos;
        currentAction.Complete();
      }
      else if (currentAction.type == ActionType.Teleport) {
        Actor a = GetActor(currentAction);
        a.transform.position = currentAction.pos;
        a.SetDirection(currentAction.dir);
        RaycastHit2D hit = Physics2D.Raycast(currentAction.pos, cam.transform.forward, 10000, pathLayer);
        if (hit.collider != null) {
          Path p = hit.collider.GetComponent<Path>();
          a.WalkTo(currentAction.pos, currentAction.dir, p);
        }
        currentAction.Complete();
      }
      else if (currentAction.type == ActionType.Speak) {
        Actor a = GetActor(currentAction);
        a.Say(currentAction.strValue, currentAction);
        a.SetDirection(currentAction.dir);
        currentAction.Play();
      }
      else if (currentAction.type == ActionType.Expression) {
        Actor a = GetActor(currentAction);
        a.SetDirection(currentAction.dir);
        a.SetExpression(Enums.GetExp(currentAction.strValue));
        currentAction.Play();
      }
      else if (currentAction.type == ActionType.Sound) {
        Actor a = GetActor(currentAction);
        if (a != null) a.SetDirection(currentAction.dir);
        currentActor.PlaySound(Sounds[(int)currentAction.sound]);
        currentAction.Play();
      }
      else if (currentAction.type == ActionType.Enable) {
        // Find the actual Item from all the known items, pick it by enum
        Item item = allObjects.FindItemByID(currentAction.item);
        if (item == null) {
          Debug.LogError("Cannot find item!");
          return;
        }
        item.gameObject.SetActive(currentAction.yesNo);
        currentAction.Complete();
      }
      else if (currentAction.type == ActionType.Open) {
        // Find the actual Item from all the known items, pick it by enum
        Item item = allObjects.FindItemByID(currentAction.item);
        item.Open(currentAction.yesNo);
        currentAction.Complete();
      }
      else if (currentAction.type == ActionType.Lock) {
        // Find the actual Item from all the known items, pick it by enum
        Item item = allObjects.FindItemByID(currentAction.item);
        item.Lock(currentAction.yesNo);
        currentAction.Complete();
      }
      else {
        // FIXME do the other actions
        Debug.Log("Not implemented action: " + currentAction.ToString());
        currentAction.Complete(); // Just to avoid to block the game for action not yet done
      }

    }
    else if (currentAction.IsPlaying()) {
      currentAction.CheckTime(Time.deltaTime);
    }
    else if (currentAction.IsCompleted()) {
      if (currentAction.Repeatable) currentAction.Reset();
      if (currentSequence != null) {
        currentAction = currentSequence.GetNextAction();
      }
      else if (actions.Count > 0) {
        currentAction = actions.GetFirst();
      }
      else
        currentAction = null;

      if (currentAction == null) {
        status = GameStatus.NormalGamePlay;
      }
    }
  }

  private Actor GetActor(GameAction a) {
    if (a.actor == Chars.Current) return currentActor;
    if (a.actor == Chars.Actor1) return actor1;
    if (a.actor == Chars.Actor2) return actor2;
    if (a.actor == Chars.Actor3) return actor3;
    if (a.actor == Chars.None) return null;
    return actors[(int)a.actor];
  }


  private IEnumerator ChangeRoom(Actor actor, Door door) {
    // Disable gameplay
    status = GameStatus.RoomTransition;
    yield return null;

    // Enable dst
    door.dst.gameObject.SetActive(true);

    // Get dst camera pos + door dst pos
    Vector3 orgp = cam.transform.position;
    Vector3 dstp = door.camerapos;

    // Move camera quickly from current to dst
    float time = 0;
    while (time < .125f) {
      // Fade black
      BlackFade.color = new Color32(0, 0, 0, (byte)(255 * (time * 8)));
      cam.transform.position = (1 - time * 4) * orgp + (time * 4) * dstp;
      time += Time.deltaTime;
      yield return null;
    }
    actor.transform.position = door.correspondingDoor.HotSpot;
    yield return null;
    while (time < .25f) {
      // Fade black
      BlackFade.color = new Color32(0, 0, 0, (byte)(255 * (1 - (8 * (time - .125f)))));
      cam.transform.position = (1 - time * 4) * orgp + (time * 4) * dstp;
      time += Time.deltaTime;
      yield return null;
    }
    cam.transform.position = dstp;

    // Disable src
    door.src.gameObject.SetActive(false);
    actor.Stop();

    // Move actor to dst door pos
    currentRoom = door.dst;
    currentActor = actor;
    actor.transform.position = door.correspondingDoor.HotSpot;
    actor.currentRoom = currentRoom;
    yield return null;

    // Disable actors not in current room
    foreach (Actor a in actors) {
      if (a == null) continue;
      a.gameObject.SetActive(a.currentRoom == currentRoom);
    }

    // Enable gmaeplay
    status = GameStatus.NormalGamePlay;
    forcedCursor = CursorTypes.None;
    overItem = null;
  }

  private IEnumerator FadeToRoomActor() {
    // Disable gameplay
    status = GameStatus.RoomTransition;
    yield return null;

    Room prev = currentRoom;
    currentRoom = currentActor.currentRoom;

    // Get dst camera pos + door dst pos
    Vector3 dstp = currentActor.transform.position;
    dstp.y = currentRoom.CameraGround;
    dstp.z = -10;

    // Move camera quickly from current to dst
    float time = 0;
    while (time < .125f) {
      // Fade black
      BlackFade.color = new Color32(0, 0, 0, (byte)(255 * (time * 8)));
      time += Time.deltaTime;
      yield return null;
    }
    prev.gameObject.SetActive(false);
    currentRoom.gameObject.SetActive(true);
    cam.transform.position = dstp;
    status = GameStatus.NormalGamePlay; // Enable gmaeplay, this will make the camera to adjust
    yield return null;

    // Disable actors not in current room
    foreach (Actor a in actors) {
      if (a == null) continue;
      a.gameObject.SetActive(a.currentRoom == currentRoom);
    }

    while (time < .25f) {
      // Fade black
      BlackFade.color = new Color32(0, 0, 0, (byte)(255 * (1 - (8 * (time - .125f)))));
      time += Time.deltaTime;
      yield return null;
    }
    forcedCursor = CursorTypes.None;
    overItem = null;
  }

  internal static Running ActionStatus(ActionEnum action) {
    // Find the action, it can be in the sequences or in any action we saw in some way
    return Running.NotStarted; // FIXME
  }


}



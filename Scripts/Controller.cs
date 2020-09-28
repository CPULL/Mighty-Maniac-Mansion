using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using SimpleJSON;

public class Controller : MonoBehaviour {
  Camera cam;
  public AllObjects allObjects;
  public AudioClip[] Sounds;
  public LayerMask pathLayer;
  public UnityEngine.UI.Image BlackFade;
  private static Controller c;
  public Transform PickedItems;
  public Material SceneSelectionPoint;
  public AudioSource MusicPlayer;


  public TextMeshProUGUI DbgMsg;
  public static void Dbg(string txt) {
    c.DbgMsg.text = txt;
  }


  #region *********************** Mouse and Interaction *********************** Mouse and Interaction *********************** Mouse and Interaction ***********************
  void Update() {
    if (Options.IsActive()) return;
    if (GameData.status == GameStatus.StartGame) {
      StartGame();
      return;
    }
    if (GameData.status != GameStatus.NormalGamePlay && GameData.status != GameStatus.Cutscene) return;


    cursorTime += Time.deltaTime;
    HandleCursor();



    #region Handling of text messages
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
    #endregion

    #region Sequences and actions
    if (currentCutscene != null) { // Do we have a sequence?
      if (currentAction == null) { // Do we have the action?
        currentAction = currentCutscene.GetNextAction();
        if (currentAction == null) {
          currentCutscene = null;
          forcedCursor = CursorTypes.None;
          if (actionsToPlay.Count > 0) currentAction = actionsToPlay.GetFirst();
        }
      }
    }
    else if (actionsToPlay.Count > 0) currentAction = actionsToPlay.GetFirst();

    if (currentAction != null) {
      PlayCurrentAction();
      return;
    }
    #endregion

    #region Handle camera
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
    else if (cpos.x < .4f * Screen.width) {
      if (cam.transform.position.x > currentRoom.minL) {
        cam.transform.position -= cam.transform.right * Time.deltaTime * (.4f * Screen.width - cpos.x) / 10;
      }
    }
    else if (cpos.x > .6f * Screen.width) {
      if (cam.transform.position.x < currentRoom.maxR) {
        cam.transform.position += cam.transform.right * Time.deltaTime * (cpos.x - .6f * Screen.width) / 10;
      }
    }
    #endregion


    if (GameData.status != GameStatus.NormalGamePlay) return;

    // LMB -> Walk or secondary action
    // RMB -> Default action


    #region Mouse control
    bool lmb = Input.GetMouseButtonDown(0);
    bool rmb = Input.GetMouseButtonDown(1);

    if (overActor != null) {
      if (rmb && usedItem != null) {
        if (currentActor == overActor) return;
        receiverActor = overActor;
        usedItem.Give(currentActor, receiverActor);
        Inventory.SetActive(false);
        usedItem = null;
        oldCursor = null;
        forcedCursor = CursorTypes.None;
        return;
      }
      if (lmb) {
        // FIXME check that is a playable actor
        if (overActor != currentActor) {
          SelectActor(overActor);
          return;
        }
      }
    }
    
    if (overItem != null) {
      if ((overItem.whatItDoesR == WhatItDoes.Read && rmb) || (overItem.whatItDoesL == WhatItDoes.Read && lmb)) {
        WalkAndAction(currentActor, overItem,
          new System.Action<Actor, Item>((actor, item) => {
            actor.SetDirection(item.dir);
            if (item.HasActions(When.Use)) {
              if (item.PlayActions(currentActor, null, When.Use))
                actor.Say(item.Description); // By default read what is written in the description of the object
            }
            else
              actor.Say(item.Description);
          }));
      }

      else if ((overItem.whatItDoesR == WhatItDoes.Use && rmb) || (overItem.whatItDoesL == WhatItDoes.Use && lmb)) {
        if (usedItem == null) {
          WalkAndAction(currentActor, overItem,
            new System.Action<Actor, Item>((actor, item) => {
              actor.SetDirection(item.dir);
              string res = item.Use(currentActor);
              if (res != null)
                actor.Say(res);
            }));
        }
        else { // Can we use the two items together?
          if (usedItem.CheckCombinedActions(currentActor, overItem)) { // Yes
            if (!usedItem.PlayActions(currentActor, null, When.Use, overItem))
              currentActor.Say("It does not work");
            else {
              c.forcedCursor = CursorTypes.None;
              oldCursor = null;
              usedItem = null;
              Inventory.SetActive(false);
              return;
            }
          }
          else if (overItem.CheckCombinedActions(currentActor, usedItem)) { // Yes
            if (!overItem.PlayActions(currentActor, null, When.Use, usedItem))
              currentActor.Say("It does not work");
            else {
              c.forcedCursor = CursorTypes.None;
              oldCursor = null;
              usedItem = null;
              Inventory.SetActive(false);
              return;
            }

          }
          else {
            currentActor.Say("It does not work...");
          }

        }
      }

      else if (overItem.owner == Chars.None && ((overItem.whatItDoesR == WhatItDoes.Pick && rmb) || (overItem.whatItDoesL == WhatItDoes.Pick && lmb))) {
        WalkAndAction(currentActor, overItem,
          new System.Action<Actor, Item>((actor, item) => {
            ShowName(currentActor + " got " + item.Name);
            actor.inventory.Add(item);
            item.transform.parent = PickedItems;
            item.gameObject.SetActive(false);
            item.owner = Chars.None;
            if (actor == actor1) item.owner = Chars.Actor1;
            else if (actor == actor2) item.owner = Chars.Actor2;
            else if (actor == actor3) item.owner = Chars.Actor3;
            item.PlayActions(currentActor, null, When.Pick);
            item = null;
            forcedCursor = CursorTypes.None;
            if (Inventory.activeSelf) ActivateInventory(currentActor);
          }));
        overItem = null;
      }

      else if (overItem.owner != Chars.None && lmb) { // Get item from inventory
        if (usedItem == overItem) {
          c.forcedCursor = CursorTypes.None;
          oldCursor = null;
          usedItem = null;
          return;
        }
        usedItem = overItem;
        overItem = null;
        oldCursor = null;
        c.forcedCursor = CursorTypes.Item;
        Cursor.SetCursor(usedItem.cursorImage, new Vector2(usedItem.cursorImage.width / 2, usedItem.cursorImage.height / 2), CursorMode.Auto);
      }

      else if ((overItem.whatItDoesR == WhatItDoes.Walk && rmb) || (overItem is Door && overItem.whatItDoesL == WhatItDoes.Walk && lmb)) {
        WalkAndAction(currentActor, overItem,
          new System.Action<Actor, Item>((actor, item) => {
            if (item.Usable == Tstatus.OpenableLocked || item.Usable == Tstatus.OpenableLockedAutolock) {
              actor.Say("Is locked");
              return;
            }
            else if (item.Usable == Tstatus.OpenableClosed || item.Usable == Tstatus.OpenableClosedAutolock) {
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
        PathNode p = hit.collider.GetComponent<PathNode>();
        currentActor.WalkTo(hit.point, p);
        walkDelay = 0;
      }
    }
    else if (Input.GetMouseButton(0) && currentActor.IsWalking()) {
      if (walkDelay > .1f) {
        RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), cam.transform.forward, 10000, pathLayer);
        if (hit.collider != null) {
          PathNode p = hit.collider.GetComponent<PathNode>();
          currentActor.WalkTo(hit.point, p);
          walkDelay = 0;
        }
      }
      else {
        walkDelay += Time.deltaTime;
      }
    }
    #endregion
  }

  private CursorTypes forcedCursor = CursorTypes.None;
  private Texture2D oldCursor = null;
  public Texture2D[] Cursors;
  float cursorTime = 0;

  void HandleCursor() {
    if (GameData.status != GameStatus.NormalGamePlay) {
      if (oldCursor != Cursors[(int)CursorTypes.Wait]) {
        Cursor.SetCursor(Cursors[(int)CursorTypes.Wait], new Vector2(Cursors[(int)CursorTypes.Wait].width / 2, Cursors[(int)CursorTypes.Wait].height / 2), CursorMode.Auto);
        oldCursor = Cursors[(int)CursorTypes.Wait];
      }
      return;
    }

    if (forcedCursor == CursorTypes.Item) return;

    if (forcedCursor == CursorTypes.None) {
      if (0 <= cursorTime && cursorTime <= .5f) {
        if (oldCursor != Cursors[0]) {
          Cursor.SetCursor(Cursors[0], new Vector2(Cursors[0].width / 2, Cursors[0].height / 2), CursorMode.Auto);
          oldCursor = Cursors[0];
        }
      }
      else if (.5f < cursorTime && cursorTime <= .75f) {
        if (oldCursor != Cursors[1]) {
          Cursor.SetCursor(Cursors[1], new Vector2(Cursors[1].width / 2, Cursors[1].height / 2), CursorMode.Auto);
          oldCursor = Cursors[1];
        }
      }
      else if (.75f < cursorTime && cursorTime <= .9f) {
        if (oldCursor != Cursors[2]) {
          Cursor.SetCursor(Cursors[2], new Vector2(Cursors[2].width / 2, Cursors[2].height / 2), CursorMode.Auto);
          oldCursor = Cursors[2];
        }
      }
      else if (.9f < cursorTime && cursorTime <= 1.05f) {
        if (oldCursor != Cursors[1]) {
          Cursor.SetCursor(Cursors[1], new Vector2(Cursors[1].width / 2, Cursors[1].height / 2), CursorMode.Auto);
          oldCursor = Cursors[1];
        }
      }
      else {
        cursorTime = 0;
        if (oldCursor != Cursors[0]) {
          Cursor.SetCursor(Cursors[0], new Vector2(Cursors[0].width / 2, Cursors[0].height / 2), CursorMode.Auto);
          oldCursor = Cursors[0];
        }
      }
      return;
    }

    if (oldCursor != Cursors[(int)forcedCursor]) {
      Cursor.SetCursor(Cursors[(int)forcedCursor], new Vector2(Cursors[(int)forcedCursor].width / 2, Cursors[(int)forcedCursor].height / 2), CursorMode.Auto);
      oldCursor = Cursors[(int)forcedCursor];
    }
  }

  internal static CursorTypes GetCursor() {
    Cursor.SetCursor(c.Cursors[0], new Vector2(c.Cursors[0].width / 2, c.Cursors[0].height / 2), CursorMode.Auto);
    return c.forcedCursor;
  }


  internal static void SetCursor(CursorTypes cur) {
    c.forcedCursor = cur;
  }


  internal static void HandleToolbarClicks(IPointerClickHandler handler) {
    if (GameData.status != GameStatus.NormalGamePlay || Options.IsActive()) return;
    PortraitClickHandler h = (PortraitClickHandler)handler;
    if (h == c.ActorPortrait1) {
      SelectActor(c.actor1);
    }
    else if (h == c.ActorPortrait2) {
      SelectActor(c.actor2);
    }
    else if (h == c.ActorPortrait3) {
      SelectActor(c.actor3);
    }
    else if (h == c.InventoryPortrait) {
      if (c.Inventory.activeSelf) { // Show/Hide inventory of current actor
        c.Inventory.SetActive(false);
        c.InventoryPortrait.GetComponent<UnityEngine.UI.RawImage>().color = new Color32(0x6D, 0x7D, 0x7C, 0xff);
        return;
      }
      else
        c.ActivateInventory(c.currentActor);
    }
  }


  #endregion



  #region *********************** Initialization

  private void Awake() {
    c = this;
    cam = Camera.main;

    LoadSequences();

    StartCoroutine(StartDelayed());
    foreach (Room r in allObjects.roomsList) {
      r.gameObject.SetActive(false);
    }
    foreach (Actor a in allEnemies)
      if (a != null) a.gameObject.SetActive(false);
    foreach (Actor a in allActors)
      if (a != null) a.gameObject.SetActive(false);
    ActorsButtons.SetActive(false);

    GameData.status = GameStatus.IntroVideo;
  }

  private void Start() {
    SceneSelectionPoint.color = new Color32(0, 0, 0, 0);
    currentRoom = allObjects.roomsList[0];
    currentActor = actor1;
    ActorPortrait1.GetComponent<UnityEngine.UI.RawImage>().color = c.selectedActor;

    Options.GetOptions();
    GameData.status = GameStatus.IntroVideo;
  }

  void StartGame() {
    foreach (Room r in allObjects.roomsList) {
      r.gameObject.SetActive(r == currentRoom);
    }
    GameData.status = GameStatus.NormalGamePlay;
    foreach (Actor a in allActors) {
      if (a == null) continue;
      a.gameObject.SetActive(a.currentRoom == currentRoom);
    }
    foreach (Actor a in allEnemies) {
      if (a == null) continue;
      a.gameObject.SetActive(a.currentRoom == currentRoom);
    }
    SkyBackground.enabled = true;

    actor1 = GetActor(GameData.actor1);
    actor2 = GetActor(GameData.actor2);
    actor3 = GetActor(GameData.actor3);
    kidnappedActor = GetActor(GameData.kidnapped);

    // We need to update the portraits...


    ActorsButtons.SetActive(true);
    StartIntroCutscene();
  }

  IEnumerator StartDelayed() {
    yield return new WaitForSeconds(.5f);
    ShowName(currentRoom.RoomName);
  }

  void OnApplicationQuit() {
    SceneSelectionPoint.color = new Color32(255, 255, 255, 255);
  }
  #endregion


  #region *********************** Cutscenes and Actions *********************** Cutscenes and Actions *********************** Cutscenes and Actions ***********************
  Cutscene currentCutscene;
  ContextualizedAction currentAction;
  public List<Cutscene> cutscenes;
  readonly SList<ContextualizedAction> actionsToPlay = new SList<ContextualizedAction>(16);
  readonly HashSet<GameAction> allKnownActions = new HashSet<GameAction>();

  void LoadSequences() {
    string path = Application.dataPath + "/Actions/";

    foreach (string file in System.IO.Directory.GetFiles(path, "*.json")) {
      //FIXME Debug.Log(file);
      string json = System.IO.File.ReadAllText(file);

      try {
        JSONNode j = JSON.Parse(json);

        Cutscene seq = new Cutscene(j["id"].Value, j["name"].Value);
        // FIXME conditions
        // Actions
        JSONNode.ValueEnumerator vals = j["actions"].Values;
        foreach (JSONNode val in vals) {
          GameAction a = new GameAction(val["type"].Value);
          if (a.type == ActionType.Teleport) {
            a.SetActor(val["actor"].Value);
            a.SetPos(val["pos"][0].AsFloat, val["pos"][1].AsFloat);
            a.SetDir(val["dir"].Value);
            a.SetValue(val["room"].Value);
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
            a.SetValue(val["room"].Value);
          }
          else if (a.type == ActionType.Sound) {
            a.SetActor(val["actor"].Value);
            a.SetDir(val["dir"].Value);
            a.SetValue(val["snd"].Value);
          }
          a.SetWait(val["wait"].AsFloat);
          seq.actions.Add(a);
        }
        cutscenes.Add(seq);

      } catch (System.Exception e) {
        Debug.Log("ERROR (" + file + "): " + e.Message);
      }
    }

    currentCutscene = null;
  }

  void StartIntroCutscene() {
    foreach (Cutscene s in cutscenes) {
      if (s.id == "intro") {
        currentCutscene = s;
        forcedCursor = CursorTypes.Wait;
        oldCursor = null;
        GameData.status = GameStatus.NormalGamePlay;
        break;
      }
    }
  }

  public static void AddAction(GameAction a, Actor perf, Actor sec, Item item) {
    c.actionsToPlay.Add(new ContextualizedAction { action = a, performer = perf, secondary = sec, item = item });
    c.allKnownActions.Add(a);
  }

  public static void KnowAction(GameAction a) {
    c.allKnownActions.Add(a);
  }

  void PlayCurrentAction() {
    if (currentAction.NotStarted()) {
      // FIXME        Debug.Log(currentAction.ToString());
      RunCurrentAction();
    }
    else if (currentAction.IsPlaying()) {
      currentAction.action.CheckTime(Time.deltaTime);
    }
    else if (currentAction.IsCompleted()) {
      if (currentAction.action.Repeatable) currentAction.action.Reset();
      if (currentCutscene != null) {
        currentAction = currentCutscene.GetNextAction();
      }
      else if (actionsToPlay.Count > 0) {
        currentAction = actionsToPlay.GetFirst();
      }
      else
        currentAction = null;

      if (currentAction == null) {
        GameData.status = GameStatus.NormalGamePlay;
      }
    }
  }

  internal static Running ActionStatus(ActionEnum action) {
    foreach(GameAction a in c.allKnownActions) {
      if (a.action == action) return a.running;
    }
    return Running.NotStarted;
  }

  void RunCurrentAction() {
    switch (currentAction.action.type) {
      case ActionType.Teleport: {
        Actor a = GetActor(currentAction.action.actor);
        Room aroom = allObjects.GetRoom(currentAction.action.strValue);
        if (aroom != null) a.currentRoom = aroom;
        a.transform.position = currentAction.action.pos;
        a.SetDirection(currentAction.action.dir);
        a.SetScaleAndPosition(currentAction.action.pos);
        currentAction.Complete();
      }
      break;

      case ActionType.Speak: {
        Actor a = GetActor(currentAction.action.actor);
        if (a != null) {
          a.Say(currentAction.action.strValue, currentAction.action);
          a.SetDirection(currentAction.action.dir);
          currentAction.Play();
        }
        else
          currentAction.Complete();
      }
      break;

      case ActionType.Move: {
        RaycastHit2D hit = Physics2D.Raycast(currentAction.action.pos, cam.transform.forward, 10000, pathLayer);
        if (hit.collider != null) {
          PathNode p = hit.collider.GetComponent<PathNode>();
          GameAction copy = currentAction.action;
          currentAction.performer.WalkTo(currentAction.action.pos, p,
          new System.Action<Actor, Item>((actor, item) => { copy.Complete(); }));
        }
      }
      break;

      case ActionType.Expression: {
        Actor a = GetActor(currentAction.action.actor);
        a.SetDirection(currentAction.action.dir);
        a.SetExpression(Enums.GetExp(currentAction.action.strValue));
        currentAction.Play();
      }
      break;

      case ActionType.Open: {
        // Find the actual Item from all the known items, pick it by enum
        Item item = allObjects.FindItemByID(currentAction.action.item);
        currentActor.Say(item.Open(currentAction.action.change));
        currentAction.Complete();
      }
      break;

      case ActionType.Enable: {
        // Find the actual Item from all the known items, pick it by enum
        Item item = allObjects.FindItemByID(currentAction.action.item);
        if (item == null) {
          Debug.LogError("Cannot find item!");
          return;
        }
        item.gameObject.SetActive(currentAction.action.change == ChangeWay.EnOpenLock);
        currentAction.Complete();
      }
      break;

      case ActionType.Lock: {
        // Find the actual Item from all the known items, pick it by enum
        Item item = allObjects.FindItemByID(currentAction.action.item);
        currentActor.Say(item.Lock(currentAction.action.change));
        currentAction.Complete();
      }
      break;

      case ActionType.ShowRoom: {
        currentRoom = allObjects.GetRoom(currentAction.action.strValue);
        Vector3 pos = currentAction.action.pos;
        pos.z = -10;
        cam.transform.position = pos;
        foreach (Room r in allObjects.roomsList)
          r.gameObject.SetActive(false);
        currentRoom.gameObject.SetActive(true);
        currentAction.Complete();
      }
      break;

      case ActionType.Cutscene: {

      }
      break;

      case ActionType.Sound: {
        Actor a = GetActor(currentAction.action.actor);
        if (a != null) a.SetDirection(currentAction.action.dir);
        currentActor.PlaySound(Sounds[(int)currentAction.action.sound]);
        currentAction.Play();
      }
      break;

      case ActionType.ReceiveY: {
        currentAction.performer.inventory.Remove(currentAction.item);
        currentAction.secondary.inventory.Add(currentAction.item);
        currentAction.item.owner = GetCharFromActor(currentAction.secondary);
        UpdateInventory();
        if (currentAction.secondary != null) {
          currentAction.secondary.Say(currentAction.action.strValue, currentAction.action);
          currentAction.secondary.SetDirection(currentAction.action.dir);
          currentAction.Play();
        }
        else
          currentAction.Complete();
      }
      break;

      case ActionType.ReceiveN: {
        if (currentAction.secondary != null) {
          currentAction.secondary.Say(currentAction.action.strValue, currentAction.action);
          currentAction.secondary.SetDirection(currentAction.action.dir);
          currentAction.Play();
        }
        else
          currentAction.Complete();
      }
      break;

      default: {
        // FIXME do the other actions
        Debug.Log("Not implemented action: " + currentAction.ToString());
        currentAction.Complete(); // Just to avoid to block the game for action not yet done
      }
      break;
    }

  }

  #endregion

  #region *********************** Inventory and Items *********************** Inventory and Items *********************** Inventory and Items ***********************
  public GameObject Inventory;
  public GameObject InventoryItemTemplate;
  private Item overItem = null; // Items we are over with the mouse
  private Item usedItem = null; // Item that is being used (and visible on the cursor)

  internal static void UpdateInventory() {
    if (c.Inventory.activeSelf)
      c.ActivateInventory(c.currentActor);
  }

  private void ActivateInventory(Actor actor) {
    Inventory.SetActive(true);
    InventoryPortrait.GetComponent<UnityEngine.UI.RawImage>().color = new Color32(0x7D, 0x8D, 0xfC, 0xff);
    foreach (Transform t in Inventory.transform)
      GameObject.Destroy(t.gameObject);

    foreach (Item item in actor.inventory) {
      GameObject ii = Instantiate(InventoryItemTemplate, Inventory.transform);
      ii.gameObject.SetActive(true);
      InventoryItem it = ii.GetComponent<InventoryItem>();
      it.text.text = item.Name;
      it.front.sprite = item.iconImage;
      it.item = item;
    }
  }

  internal static void SetItem(Item item, bool fromInventory = false) {
    if (GameData.status != GameStatus.NormalGamePlay) return;

    if (fromInventory) {
      if (item == null) {
        c.overItem = null;
        if (c.TextMsg.text != "") c.HideName();
        return;
      }
      c.overItem = item;
      if (c.usedItem == null) {
        if (item.whatItDoesR == WhatItDoes.Use) {
          c.forcedCursor = CursorTypes.Use;
          c.overItem = item;
          c.ShowName(item.Name);
        }
        else if (item.whatItDoesR == WhatItDoes.Read) {
          c.forcedCursor = CursorTypes.Examine;
          c.overItem = item;
          c.ShowName(item.Name);
        }
      }
      return;
    }

    if (item == null) {
      if (c.forcedCursor != CursorTypes.Item) c.forcedCursor = CursorTypes.None;
      c.overItem = null;
      if (c.TextMsg.text != "") c.HideName();
      return;
    }
    if (item.owner != Chars.None) {
      c.overItem = item;
      return;
    }

    // Right
    if (item.whatItDoesR == WhatItDoes.Walk) {
      if (c.forcedCursor != CursorTypes.Item) c.forcedCursor = CursorTypes.None;
      c.overItem = item;
    }
    else if (item.whatItDoesR == WhatItDoes.Pick) {
      if (c.forcedCursor != CursorTypes.Item) c.forcedCursor = CursorTypes.PickUp;
      c.overItem = item;
      c.ShowName(item.Name);
    }
    else if (item.whatItDoesR == WhatItDoes.Use) {
      if (c.forcedCursor != CursorTypes.Item) c.forcedCursor = CursorTypes.Use;
      c.overItem = item;
      c.ShowName(item.Name);
    }
    else if (item.whatItDoesR == WhatItDoes.Read) {
      if (c.forcedCursor != CursorTypes.Item) c.forcedCursor = CursorTypes.Examine;
      c.overItem = item;
      c.ShowName(item.Name);
    }
    // Left
    else if (item.whatItDoesL == WhatItDoes.Walk) {
      if (c.forcedCursor != CursorTypes.Item) c.forcedCursor = CursorTypes.None;
      c.overItem = item;
    }
    else if (item.whatItDoesL == WhatItDoes.Pick) {
      if (c.forcedCursor != CursorTypes.Item) c.forcedCursor = CursorTypes.PickUp;
      c.overItem = item;
      c.ShowName(item.Name);
    }
    else if (item.whatItDoesL == WhatItDoes.Use) {
      if (c.forcedCursor != CursorTypes.Item) c.forcedCursor = CursorTypes.Use;
      c.overItem = item;
      c.ShowName(item.Name);
    }
    else if (item.whatItDoesL == WhatItDoes.Read) {
      if (c.forcedCursor != CursorTypes.Item) c.forcedCursor = CursorTypes.Examine;
      c.overItem = item;
      c.ShowName(item.Name);
    }
  }

  internal static bool IsItemCollected(ItemEnum itemID) {
    foreach(Item item in c.actor1.inventory) {
      if (item.Item == itemID) return true;
    }
    foreach(Item item in c.actor2.inventory) {
      if (item.Item == itemID) return true;
    }
    foreach(Item item in c.actor3.inventory) {
      if (item.Item == itemID) return true;
    }
    return false;
  }

  #endregion

  #region *********************** Actors *********************** Actors *********************** Actors *********************** Actors *********************** Actors ***********************
  public PortraitClickHandler ActorPortrait1;
  public PortraitClickHandler ActorPortrait2;
  public PortraitClickHandler ActorPortrait3;
  public PortraitClickHandler InventoryPortrait;
  Actor actor1;
  Actor actor2;
  Actor actor3;
  Actor kidnappedActor;
  Actor receiverActor; // FIXME we should set this in some way, probably from actions
  Actor currentActor = null;
  Color32 unselectedActor = new Color32(0x6D, 0x7D, 0x7C, 255);
  Color32 selectedActor = new Color32(200, 232, 152, 255);
  public Actor[] allEnemies;
  public Actor[] allActors;

  /// <summary>
  /// Gets the actual Actor from the Chars enum
  /// </summary>
  public static Actor GetActor(Chars actor) {
    switch (actor) {
      case Chars.None: return null;
      case Chars.Current: return c.currentActor;
      case Chars.Actor1: return c.actor1;
      case Chars.Actor2: return c.actor2;
      case Chars.Actor3: return c.actor3;
      case Chars.KidnappedActor: return c.kidnappedActor;
      case Chars.Receiver: return c.receiverActor;
      case Chars.Fred: return c.allEnemies[0];
      case Chars.Edna: return c.allEnemies[1];
      case Chars.Ted: return c.allEnemies[2];
      case Chars.Ed: return c.allEnemies[3];
      case Chars.Edwige: return c.allEnemies[4];
      case Chars.GreenTentacle: return c.allEnemies[5];
      case Chars.PurpleTentacle: return c.allEnemies[6];
      case Chars.Dave: return c.allActors[0];
      case Chars.Bernard: return c.allActors[1];
      case Chars.Wendy: return c.allActors[2];
      case Chars.Syd: return c.allActors[3];
      case Chars.Hoagie: return c.allActors[4];
      case Chars.Razor: return c.allActors[5];
      case Chars.Michael: return c.allActors[6];
      case Chars.Jeff: return c.allActors[7];
      case Chars.Javid: return c.allActors[8];
      case Chars.Laverne: return c.allActors[9];
      case Chars.Ollie: return c.allActors[10];
      case Chars.Sandy: return c.allActors[11];
    }
    Debug.LogError("Invalid actor requested! " + actor);
    return null;
  }


  public static Chars GetCharFromActor(Actor actor) {
    if (actor == null) return Chars.None;
    else if (actor == c.actor1) return Chars.Actor1;
    else if (actor == c.actor2) return Chars.Actor2;
    else if (actor == c.actor3) return Chars.Actor3;
    else if (actor == c.kidnappedActor) return Chars.KidnappedActor;
    else if (actor == c.receiverActor) return Chars.Receiver;
    else if (actor == c.allEnemies[0]) return Chars.Fred;
    else if (actor == c.allEnemies[1]) return Chars.Edna;
    else if (actor == c.allEnemies[2]) return Chars.Ted;
    else if (actor == c.allEnemies[3]) return Chars.Ed;
    else if (actor == c.allEnemies[4]) return Chars.Edwige;
    else if (actor == c.allEnemies[5]) return Chars.GreenTentacle;
    else if (actor == c.allEnemies[6]) return Chars.PurpleTentacle;
    else if (actor == c.allActors[0]) return Chars.Dave;
    else if (actor == c.allActors[1]) return Chars.Bernard;
    else if (actor == c.allActors[2]) return Chars.Wendy;
    else if (actor == c.allActors[3]) return Chars.Syd;
    else if (actor == c.allActors[4]) return Chars.Hoagie;
    else if (actor == c.allActors[5]) return Chars.Razor;
    else if (actor == c.allActors[6]) return Chars.Michael;
    else if (actor == c.allActors[7]) return Chars.Jeff;
    else if (actor == c.allActors[8]) return Chars.Javid;
    else if (actor == c.allActors[9]) return Chars.Laverne;
    else if (actor == c.allActors[10]) return Chars.Ollie;
    else if (actor == c.allActors[11]) return Chars.Sandy;
    else if (actor == c.currentActor) return Chars.Current;
    Debug.LogError("Invalid actor requested! " + actor);
    return Chars.None;
  }

  /// <summary>
  /// Checks if the passed actor is an enemy (Fred, Edna, Ed, etc.)
  /// </summary>
  public static bool IsEnemy(Actor actor) {
    foreach (Actor a in c.allEnemies)
      if (a == actor) return true;
    return false;
  }

  /// <summary>
  /// Checks if the actor is one of the actors of the playing trio
  /// </summary>
  internal static bool WeHaveActorPlaying(Chars actor) {
    return actor == c.actor1.id || actor == c.actor2.id || actor == c.actor3.id;
  }
  float walkDelay = 0;

  /// <summary>
  /// Moves the actor to the destination and execute the action callback when the destination is reached
  /// </summary>
  void WalkAndAction(Actor actor, Item item, System.Action<Actor, Item> action) {
    Vector3 one = actor.transform.position;
    one.z = 0;
    Vector3 two = item.HotSpot;
    two.z = 0;
    float dist = Vector3.Distance(one, two);
    if (dist > .2f) { // Need to walk
      RaycastHit2D hit = Physics2D.Raycast(overItem.HotSpot, cam.transform.forward, 10000, pathLayer);
      if (hit.collider != null) {
        PathNode p = hit.collider.GetComponent<PathNode>();
        currentActor.WalkTo(overItem.HotSpot, p, action, item);
      }
      return;
    }
    else {
      action?.Invoke(currentActor, item);
    }
  }

  internal static void SelectActor(Actor actor) {
    if (GameData.status != GameStatus.NormalGamePlay) return;

    c.forcedCursor = CursorTypes.None;
    c.oldCursor = null;
    c.usedItem = null;

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

  Actor overActor = null;
  internal static void OverActor(Actor actor) {
    c.overActor = actor;
  }


  #endregion

  #region *********************** Rooms and Transitions *********************** Rooms and Transitions *********************** Rooms and Transitions ***********************
  Room currentRoom;

  private IEnumerator ChangeRoom(Actor actor, Door door) {
    // Disable gameplay
    GameData.status = GameStatus.Cutscene;
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
    actor.SetDirection(door.correspondingDoor.arrivalDirection);
    yield return null;

    // Disable actors not in current room
    foreach (Actor a in allActors) {
      if (a == null) continue;
      a.gameObject.SetActive(a.currentRoom == currentRoom);
    }
    foreach (Actor a in allEnemies) {
      if (a == null) continue;
      a.gameObject.SetActive(a.currentRoom == currentRoom);
    }

    // Enable gmaeplay
    GameData.status = GameStatus.NormalGamePlay;
    forcedCursor = CursorTypes.None;
    overItem = null;
  }

  private IEnumerator FadeToRoomActor() {
    // Disable gameplay
    GameData.status = GameStatus.Cutscene;
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
    GameData.status = GameStatus.NormalGamePlay; // Enable gmaeplay, this will make the camera to adjust
    yield return null;

    // Disable actors not in current room
    foreach (Actor a in allActors) {
      if (a == null) continue;
      a.gameObject.SetActive(a.currentRoom == currentRoom);
    }
    foreach (Actor a in allEnemies) {
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

  #endregion

  #region *********************** UI and Options *********************** UI and Options *********************** UI and Options *********************** UI and Options ***********************
  public static float walkSpeed;
  public static float textSpeed;
  public Options options;

  public TextMeshProUGUI TextMsg;
  public RectTransform TextMsgRT;
  public RectTransform TextBackRT;
  TextMsgMode txtMsgMode = TextMsgMode.None;
  float textMsgTime = 0;
  float textSizeX = 0;

  public GameObject ActorsButtons;
  public Canvas SkyBackground;

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

  #endregion




  public static void PlayMusic(AudioClip clip) {
    c.MusicPlayer.Stop();
    c.MusicPlayer.clip = clip;
    c.MusicPlayer.Play();
  }

  public static void StopMusic() {
    c.MusicPlayer.Stop();
  }

  public static void PauseMusic() {
    if (c.MusicPlayer.isPlaying)
      c.MusicPlayer.Pause();
    else
      c.MusicPlayer.UnPause();
  }



}



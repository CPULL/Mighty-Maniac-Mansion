using System.Collections.Generic;
using UnityEngine;

public class AllObjects : MonoBehaviour {
  public List<Item> itemsList;
  public List<Room> roomsList;
  public List<FlagStatus> flagsList;
  public List<GameScene> cutscenes;

  public static IEnumerable<Room> RoomList { get { return GD.a.roomsList; } }

  private void Awake() {
    GD.a = this;

    flagsList = new List<FlagStatus>();
    foreach (GameFlag gf in System.Enum.GetValues(typeof(GameFlag)))
      flagsList.Add(new FlagStatus(gf, 0));
  }

  internal static Room GetRoom(string id) {
    foreach (Room r in GD.a.roomsList)
      if (r.ID == id) {
        return r;
      }
    Debug.LogError("Cannot find room with id: \"" + id + "\"");
    return null;
  }

  internal static Item GetItem(ItemEnum id) {
    if (id == ItemEnum.Undefined) return null;
    foreach (Item i in GD.a.itemsList) {
      if (i.ID == id) {
        return i;
      }
    }
    
    Debug.LogWarning("Cannot find Item with id: \"" + id + "\"");
    return null;
  }

  internal static bool CheckFlag(GameFlag flag, int value) {
    foreach (FlagStatus fs in GD.a.flagsList)
      if (fs.flag == flag) return fs.value == value;
    return false;
  }

  internal static void SetFlag(GameFlag flag, int val) {
    if (flag == GameFlag.GameOver) {
      if (val == 1)
        GameOver.RunGameOver(true);
      else if (val == 2)
        GameOver.PlayerDeath(GD.c.actor1);
      else if (val == 3)
        GameOver.PlayerDeath(GD.c.actor2);
      else if (val == 4)
        GameOver.PlayerDeath(GD.c.actor3);
    }

    foreach (FlagStatus fs in GD.a.flagsList)
      if (fs.flag == flag) {
        fs.value = val;
        return;
      }
  }

  internal static int GetFlag(GameFlag flag) {
    foreach (FlagStatus fs in GD.a.flagsList)
      if (fs.flag == flag) {
        return fs.value;
      }
    return 0;
  }

  public static void ResetFlags() {
    foreach (FlagStatus fs in GD.a.flagsList)
        fs.value = 0;
  }

  public static GameScene GetCutscene(CutsceneID id) {
    foreach (GameScene s in GD.a.cutscenes) {
      if (s.Id == id) {
        return s;
      }
    }
    Debug.LogError("Cutscene not found: \"" + id + "\"");
    return null;
  }


}


/// <summary>
/// List of all items that are in game and can be referenced by actions by the ID (the ID is this enum value)
/// </summary>
public enum ItemEnum {
  Undefined,
  Sign,
  GenericInvisibleDoor,
  Mailbox,
  MailboxFlag,
  FrontDoorKey,
  Grass,
  DoorBell,
  Doormat,
  Grate,
  BasementPassage,
  TedGrave,
  FrontDoor,
  Flashlight,
  Cheese,
  Batteries,
  Hamster,
  PiggyBank,
  Coin,
  DeveloperBottle,
  Fridge,
  BasementDoor,
  DungeonDoor,
  EdDoor,
  Microwave,
  RoastedHamster,
  BrokenBottleLiquid,
  BrokenDeveloperBottle,
  OldBatteries,
  PlutoniumBatteries,
  SilverKey,
  Coat,
  FruitJuice,
  WoodsMap,
  BackDoorIn,
  BackDoorExt,
  PoolValve,
  PoolWater,
  Radio,
}


public enum GameFlag {
  GreenTentacleIsFed,
  LightIsOut,
  PoolHasWater,
  PowerIsDown,
  PackageIsDeliveredStamps,
  EdnaBrowsingFridge,
  EdCheckingPackage,
  SamIsFriend, // 0=Never seen (do camera pan), 1=not friend, 2=friend
  GameOver, // 0=no, 1=nuclear, 2=noplayers
}

[System.Serializable]
public class FlagStatus {
  public GameFlag flag;
  public int value;

  public FlagStatus(GameFlag f, int v) {
    flag = f;
    value = v;
  }

  public override string ToString() {
    return flag + " = " + value;
  }
}

public enum CutsceneID {
  NONE,
  Intro,
  Doorbell,
  EdHungryCheese,
  EdnaBrowsingFridge,
  EdnaWatch,
  EdnaCatch,
  GreenTentaclePatrolling,
  EdWatch,
  EdCatch,
  Microwave,
  FredTalkingToKidnapped,
  PickDeveloperBottle,
  GoAwayFromDogR,
  GoAwayFromDogL,
  PoolEmpty,
  WaterDownAlarm,
  RedButton,

  unused1,
  unused2,
  
  Javidx9 // FIXME


}

[System.Serializable]
public class PressAction {
  public Actor actor;
  public GameFlag flag;
  public Item item;
  public int previous;

  internal void Set(Actor a, GameFlag f) {
    actor = a;
    flag = f;
    item = null;
    previous = AllObjects.GetFlag(f);
  }

  internal void Set(Actor a, Item i) {
    actor = a;
    flag = default;
    item = i;
    previous = item.GetOpeningStatus();
  }

  internal void Reset(Actor a) {
    if (actor != a) return;
    if (item == null)
      AllObjects.SetFlag(flag, previous);
    else
      item.SetOpeningStatus(previous);

    item = null;
    flag = default;
    previous = 0;
    actor = null;
  }

  internal void Reset(GameFlag f) {
    if (flag != f) return;

    AllObjects.SetFlag(flag, previous);
    item = null;
    flag = default;
    previous = 0;
    actor = null;
  }

  internal void Reset(Item i) {
    if (item != i) return;

    item.SetOpeningStatus(previous);
    item = null;
    flag = default;
    previous = 0;
    actor = null;
  }
}



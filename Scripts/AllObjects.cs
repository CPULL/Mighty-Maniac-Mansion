using System.Collections.Generic;
using UnityEngine;

public class AllObjects : MonoBehaviour {
  public List<Item> itemsList;
  public List<Room> roomsList;
  public List<FlagStatus> flagsList;
  public List<GameScene> cutscenes;

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

  internal static Item FindItemByID(ItemEnum id) {
    foreach (Item i in GD.a.itemsList) {
      if (i.Item == id) {
        return i;
      }
    }
    Debug.LogWarning("Cannot find Item with id: \"" + id + "\"");
    return null;
  }

  internal static Item FindItemByID(string sid) {
    string val = sid.Trim().ToLowerInvariant();
    ItemEnum id = ItemEnum.Undefined;
    if (val == "sign") id = ItemEnum.Sign;
    if (val == "genericinvisibledoor") id = ItemEnum.GenericInvisibleDoor;
    if (val == "mailbox") id = ItemEnum.Mailbox;
    if (val == "mailboxflag") id = ItemEnum.MailboxFlag;
    if (val == "frontdoorkey") id = ItemEnum.FrontDoorKey;
    if (val == "grass") id = ItemEnum.Grass;
    if (val == "doorbell") id = ItemEnum.DoorBell;
    if (val == "doormat") id = ItemEnum.Doormat;
    if (val == "grate") id = ItemEnum.Grate;
    if (val == "basementpassage") id = ItemEnum.BasementPassage;
    if (val == "tedgrave") id = ItemEnum.TedGrave;
    if (val == "frontdoor") id = ItemEnum.FrontDoor;

    if (id == ItemEnum.Undefined) return null;
    return FindItemByID(id);
  }

  internal static bool CheckFlag(GameFlag flag, int value) {
    foreach (FlagStatus fs in GD.a.flagsList)
      if (fs.flag == flag) return fs.value == value;
    return false;
  }

  internal static void SetFlag(GameFlag flag, int val) {
    foreach(FlagStatus fs in GD.a.flagsList)
      if (fs.flag==flag) {
        fs.value = val;
        return;
      }
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

}


public enum GameFlag {
  GreenTentacleIsFed,
  LightIsOut,
  PoolHasWater,
  PowerIsDown,
  PackageIsDeliveredStamps,
  EdnaBrowsingFridge,
}
public enum FlagValueOLD {
  Yes, No, NA
}

public class FlagStatus {
  public GameFlag flag;
  public int value;

  public FlagStatus(GameFlag f, int v) {
    flag = f;
    value = v;
  }
}

public enum CutsceneID {
  NONE,
  Intro,
  Doorbell,
  EdHungryCheese,
  EdnaBrowsingFridge,
  EdnaCatch,
  GreenTentaclePatrolling,
  GreenTentaclePatrolling2,
  GreenTentaclePatrolling3,
  GreenTentaclePatrolling4,
  FredTalkingToKidnapped,
  Javidx9 // FIXME
      

}

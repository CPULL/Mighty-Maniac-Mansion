using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharSelection : MonoBehaviour {
  public Canvas charSelectionCanvas;
  public TextMeshProUGUI Text;
  public Image[] Selections;
  public Image ActorPortraitH;
  public Image ActorPortraitA;
  public Image ActorPortraitL;
  public TextMeshProUGUI ActorDescription;
  public Sprite[] Heads;
  public Sprite[] Arms;
  public Sprite[] Legs;

  Color Transparent = new Color32(0, 0, 0, 0);
  public Image[] SPs;
  public Sprite[] Portraits;
  public Sprite QuestionMark;

  public Button ButtonStart;
  public GameObject[] Hidden;
  int mmm = 0; // FIXME not sure it will go in the final version
  Chars a1, a2, a3, ak;

  private void Awake() {
    GD.charSel = this;
  }

  private void Update() {
    if (GD.theStatus != GameStatus.CharSelection) return;
    if (!charSelectionCanvas.enabled) SelectCharacters();
    if (Options.IsActive()) return;

    if (Input.GetKeyUp(KeyCode.M)) {
      mmm++;
      if (mmm == 3) {
        foreach (GameObject h in Hidden)
          h.SetActive(true);
      }
    } else if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.M))
      mmm = 0;
  }


  public void SelectCharacters() {
    charSelectionCanvas.enabled = true;
    ButtonStart.interactable = false;
    ActorDescription.text = "";
    foreach (Image img in Selections) {
      if (img != null)
        img.color = new Color32(0, 0, 0, 0);
    }
    a1 = Chars.None;
    a2 = Chars.None;
    a3 = Chars.None;
    ak = Chars.None;

    if (GD.gs.AutoSkipDebug) {
      a1 = Chars.Razor; // FIXME
      a2 = Chars.Bernard;
      a3 = Chars.Laverne;
      ak = Chars.Hoagie;
    } // FIXME
    UpdateTitle();
  }

  void UpdateTitle() {
    int num = 0;
    if (a1 != Chars.None) num++;
    if (a2 != Chars.None) num++;
    if (a3 != Chars.None) num++;
    if (ak != Chars.None) num++;

    int pos = GetPositionFromCharacter(a1);
    SPs[0].sprite = (pos == -1) ? QuestionMark : Portraits[pos];
    pos = GetPositionFromCharacter(a2);
    SPs[1].sprite = (pos == -1) ? QuestionMark : Portraits[pos];
    pos = GetPositionFromCharacter(a3);
    SPs[2].sprite = (pos == -1) ? QuestionMark : Portraits[pos];
    pos = GetPositionFromCharacter(ak);
    SPs[3].sprite = (pos == -1) ? QuestionMark : Portraits[pos];

    if (num == 4) {
      ButtonStart.interactable = true;
      Text.text = "Select your team (            ) and\nthe person     that was kidnapped\nStart!";
    } else {
      ButtonStart.interactable = false;
      Text.text = "Select your team (            ) and\nthe person     that was kidnapped";
    }
  }


  public void Select(Actor actor) {
    Chars id = Chars.None;
    if (actor != null) id = actor.id;
    int pos = GetPositionFromCharacter(id);

    if (a1 == id) { // If selected, remove it
      Selections[pos].color = new Color32(0, 0, 0, 0);
      SPs[0].sprite = QuestionMark;
      a1 = Chars.None;
    } else if (a2 == id) { // If selected, remove it
      Selections[pos].color = new Color32(0, 0, 0, 0);
      SPs[1].sprite = QuestionMark;
      a2 = Chars.None;
    } else if (a3 == id) { // If selected, remove it
      Selections[pos].color = new Color32(0, 0, 0, 0);
      SPs[2].sprite = QuestionMark;
      a3 = Chars.None;
    } else if (ak == id) { // If selected, remove it
      Selections[pos].color = new Color32(0, 0, 0, 0);
      SPs[3].sprite = QuestionMark;
      ak = Chars.None;
    } else if (a1 == Chars.None) {
      Selections[pos].color = new Color32(0, 220, 50, 255);
      SPs[0].sprite = Portraits[pos];
      a1 = id;
    } else if (a2 == Chars.None) {
      Selections[pos].color = new Color32(0, 220, 50, 255);
      SPs[1].sprite = Portraits[pos];
      a2 = id;
    } else if (a3 == Chars.None) {
      Selections[pos].color = new Color32(0, 220, 50, 255);
      SPs[2].sprite = Portraits[pos];
      a3 = id;
    } else if (ak == Chars.None) {
      Selections[pos].color = new Color32(0, 220, 50, 255);
      SPs[3].sprite = Portraits[pos];
      ak = id;
    }

    UpdateTitle();
  }

  public void Over(Chars id) {
    int num = GetPositionFromCharacter(id);

    if (num == -1) {
      ActorPortraitH.sprite = null;
      ActorPortraitA.sprite = null;
      ActorPortraitL.sprite = null;
      ActorPortraitH.color = Transparent;
      ActorPortraitA.color = Transparent;
      ActorPortraitL.color = Transparent;
      ActorDescription.text = "";
      return;
    }
    Actor a = Controller.GetActor(id);
    ActorPortraitH.color = Color.white;
    ActorPortraitA.color = Color.white;
    ActorPortraitL.color = Color.white;
    ActorPortraitH.sprite = Heads[num];
    ActorPortraitA.sprite = Arms[num];
    ActorPortraitL.sprite = Legs[num];
    if (a == null)
      ActorDescription.text = "Not yet available:\n" + id;
    else {
      string descr = a.Description + "\n\n<i>Skills</u>:\n";
      foreach (Skill s in a.skills)
        descr += " - " + s.ToString() + "\n";
      ActorDescription.text = descr;
    }
  }

  public void StartGame() {
    GD.actor1 = a1;
    GD.actor2 = a2;
    GD.actor3 = a3;
    GD.kidnapped = ak;
    charSelectionCanvas.enabled = false;
    GD.theStatus = GameStatus.StartGame;
  }

  public void OptionsButton() {
    Options.Activate(true);
  }

  public void MainTitle() {
    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
  }

  public void QuitGame() {
    Confirm.Show("Are you sure you want to quit?", 0);
  }

  public void RemoveActor(int pos) {
    if (pos == 0 && a1 != Chars.None) {
      int num = GetPositionFromCharacter(a1);
      Selections[num].color = new Color32(0, 0, 0, 0);
      a1 = Chars.None;
    }
    if (pos == 1 && a1 != Chars.None) {
      int num = GetPositionFromCharacter(a2);
      Selections[num].color = new Color32(0, 0, 0, 0);
      a1 = Chars.None;
    }
    if (pos == 2 && a1 != Chars.None) {
      int num = GetPositionFromCharacter(a3);
      Selections[num].color = new Color32(0, 0, 0, 0);
      a1 = Chars.None;
    }
    if (pos == 4 && a1 != Chars.None) {
      int num = GetPositionFromCharacter(ak);
      Selections[num].color = new Color32(0, 0, 0, 0);
      a1 = Chars.None;
    }
    UpdateTitle();
  }

  int GetPositionFromCharacter(Chars id) {
    switch (id) {
      case Chars.None: return -1;
      case Chars.Fred: return 0;
      case Chars.Edna: return 1;
      case Chars.Ted: return 2;
      case Chars.Ed: return 3;
      case Chars.Edwige: return 4;
      case Chars.GreenTentacle: return 5;
      case Chars.PurpleTentacle: return 6;
      case Chars.BlueTentacle: return 7;
      case Chars.PurpleMeteor: return 8;
      case Chars.Dave: return 9;
      case Chars.Bernard: return 10;
      case Chars.Wendy: return 11;
      case Chars.Syd: return 12;
      case Chars.Hoagie: return 13;
      case Chars.Razor: return 14;
      case Chars.Michael: return 15;
      case Chars.Jeff: return 16;
      case Chars.Javid: return 17;
      case Chars.Laverne: return 18;
      case Chars.Ollie: return 19;
      case Chars.Sandy: return 20;
      case Chars.Unused32: return 21;
      case Chars.Unused33: return 22;
      case Chars.Unused34: return 23;
      case Chars.Unused35: return 24;
      case Chars.Unused36: return 25;
      case Chars.Unused37: return 26;
      case Chars.Unused38: return 27;
      case Chars.Unused39: return 28;
      case Chars.MarkEteer: return 29;
    }
    return -1;
  }

}


/*
refactor..........................


If we go over one of the buttons, we show the description (add +selected as XYZ+ if selected already), we should highlight the face
If we click on the selected icons we remove the actor


*/

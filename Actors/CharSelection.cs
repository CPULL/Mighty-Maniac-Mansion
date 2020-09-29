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
  int a1 = -1;
  int a2 = -1;
  int a3 = -1;
  int ak = -1;
  Color Transparent = new Color32(0, 0, 0, 0);

  public Button ButtonStart;

  private void Awake() {
    GD.charSel = this;
  }

  private void Update() {
    if (GD.status != GameStatus.CharSelection) return;
    if (!charSelectionCanvas.enabled) SelectCharacters();
    if (Options.IsActive()) return;
  }


  public void SelectCharacters() {
    charSelectionCanvas.enabled = true;
    ButtonStart.interactable = false;
    ActorDescription.text = "";
    foreach (Image img in Selections) {
      img.color = new Color32(0, 0, 0, 0);
    }
    a1 = -1;
    a2 = -1;
    a3 = -1;
    ak = -1;
    UpdateTitle();
  }

  void UpdateTitle() {
    int num = 0;
    if (a1 != -1) num++;
    if (a2 != -1) num++;
    if (a3 != -1) num++;
    Text.text = "Select your team (" + num + "/3) and the person that was kidnapped.";
    if (ak != -1) num++;

    ButtonStart.interactable = num == 4;
  }


  public void Select(int pos) {
    if (a1 == pos) {
      Selections[pos].color = new Color32(0, 0, 0, 0);
      a1 = -1;
      UpdateTitle();
      return;
    }
    if (a2 == pos) {
      Selections[pos].color = new Color32(0, 0, 0, 0);
      a2 = -1;
      UpdateTitle();
      return;
    }
    if (a3 == pos) {
      Selections[pos].color = new Color32(0, 0, 0, 0);
      a3 = -1;
      UpdateTitle();
      return;
    }
    if (ak == pos) {
      Selections[pos].color = new Color32(0, 0, 0, 0);
      ak = -1;
      UpdateTitle();
      return;
    }

    int num = 0;
    if (a1 != -1) num++;
    if (a2 != -1) num++;
    if (a3 != -1) num++;
    if (ak != -1) num++;


    if (num == 0) {
      a1 = pos;
      Selections[pos].color = new Color32(0, 220, 50, 255);
    }

    else if (num == 1) {
      if (a1 == -1) {
        a1 = pos;
        Selections[pos].color = new Color32(0, 220, 50, 255);
      }
      else if (a2 == -1) {
        a2 = pos;
        Selections[pos].color = new Color32(0, 220, 50, 255);
      }
    }

    else if (num == 2) {
      if (a1 == -1) {
        a1 = pos;
        Selections[pos].color = new Color32(0, 220, 50, 255);
      }
      else if (a2 == -1) {
        a2 = pos;
        Selections[pos].color = new Color32(0, 220, 50, 255);
      }
      else if (a3 == -1) {
        a3 = pos;
        Selections[pos].color = new Color32(0, 220, 50, 255);
      }
    }

    else if (num == 3) {
      if (a1 == -1) {
        a1 = pos;
        Selections[pos].color = new Color32(0, 220, 50, 255);
      }
      else if (a2 == -1) {
        a2 = pos;
        Selections[pos].color = new Color32(0, 220, 50, 255);
      }
      else if (a3 == -1) {
        a3 = pos;
        Selections[pos].color = new Color32(0, 220, 50, 255);
      }
      else if (ak == -1) {
        ak = pos;
        Selections[pos].color = new Color32(220, 50, 0, 255);
      }
    }

    else if (num == 4) {
      Selections[ak].color = new Color32(0, 0, 0, 0);
      ak = pos;
      Selections[pos].color = new Color32(220, 50, 0, 255);
    }

    UpdateTitle();
  }

  public void Over(int num) {
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

    Actor a = Controller.GetActor((Chars)(20 + num));
    ActorPortraitH.color = Color.white;
    ActorPortraitA.color = Color.white;
    ActorPortraitL.color = Color.white;
    ActorPortraitH.sprite = Heads[num];
    ActorPortraitA.sprite = Arms[num];
    ActorPortraitL.sprite = Legs[num];
    if (a == null)
      ActorDescription.text = "Not yet available:\n" + ((Chars)(20 + num)).ToString();
    else {
      string descr = a.Description + "\n\n<i>Skills</u>:\n";
      foreach (Skill s in a.skills)
        descr += " - " + s.ToString() + "\n";
      ActorDescription.text = descr;

    }
  }

  public void StartGame() {
    GD.actor1 = (Chars)(a1 + 20);
    GD.actor2 = (Chars)(a2 + 20);
    GD.actor3 = (Chars)(a3 + 20);
    GD.kidnapped = (Chars)(ak + 20);
    charSelectionCanvas.enabled = false;
    GD.status = GameStatus.StartGame;
  }

  public void OptionsButton() {
    Options.Activate(true);
  }

  public void MainTitle() {
    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
  }

  public void QuitGame() {
    Confirm.Show();
  }
}

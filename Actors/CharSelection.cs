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
  int akOver = -1;
  Color Transparent = new Color32(0, 0, 0, 0);
  public Image[] SPs;
  public Sprite[] Portraits;
  public Sprite QuestionMark;

  public Button ButtonStart;
  public GameObject[] Hidden;
  int mmm = 0; // FIXME not sure it will go in the final version
  int over = -1;

  private void Awake() {
    GD.charSel = this;
  }

  private void Update() {
    if (GD.status != GameStatus.CharSelection) return;
    if (!charSelectionCanvas.enabled) SelectCharacters();
    if (Options.IsActive()) return;

    if (Input.GetKeyUp(KeyCode.M)) {
      mmm++;
      if (mmm==3) {
        foreach (GameObject h in Hidden)
          h.SetActive(true);
      }
    }
    else if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.M)) 
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
    a1 = -1;
    a2 = -1;
    a3 = -1;
    ak = -1;
    a1 = 23; // FIXME
    a2 = 22;
    a3 = 21;
    ak = 20;
    UpdateTitle();
  }

  void UpdateTitle() {
    int num = 0;
    if (a1 != -1) num++;
    if (a2 != -1) num++;
    if (a3 != -1) num++;
    if (ak != -1) num++;

    SPs[0].sprite = (a1 != -1) ? Portraits[a1 - 10] : QuestionMark;
    SPs[1].sprite = (a2 != -1) ? Portraits[a2 - 10] : QuestionMark;
    SPs[2].sprite = (a3 != -1) ? Portraits[a3 - 10] : QuestionMark;
    SPs[3].sprite = (ak != -1) ? Portraits[ak - 10] : QuestionMark;

    if (num == 4) {
      ButtonStart.interactable = true;
      Text.text = "Select your team (            ) and\nthe person     that was kidnapped\nStart!";
    } else {
      ButtonStart.interactable = false;
      Text.text = "Select your team (            ) and\nthe person     that was kidnapped";
    }
  }


  public void Select(int pos) {
    if (a1 == pos) {
      Selections[over].color = new Color32(0, 0, 0, 0);
      a1 = -1;
      UpdateTitle();
      return;
    }
    if (a2 == pos) {
      Selections[over].color = new Color32(0, 0, 0, 0);
      a2 = -1;
      UpdateTitle();
      return;
    }
    if (a3 == pos) {
      Selections[over].color = new Color32(0, 0, 0, 0);
      a3 = -1;
      UpdateTitle();
      return;
    }
    if (ak == pos) {
      Selections[over].color = new Color32(0, 0, 0, 0);
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
      Selections[over].color = new Color32(0, 220, 50, 255);
    }

    else if (num == 1) {
      if (a1 == -1) {
        a1 = pos;
        Selections[over].color = new Color32(0, 220, 50, 255);
      }
      else if (a2 == -1) {
        a2 = pos;
        Selections[over].color = new Color32(0, 220, 50, 255);
      }
    }

    else if (num == 2) {
      if (a1 == -1) {
        a1 = pos;
        Selections[over].color = new Color32(0, 220, 50, 255);
      }
      else if (a2 == -1) {
        a2 = pos;
        Selections[over].color = new Color32(0, 220, 50, 255);
      }
      else if (a3 == -1) {
        a3 = pos;
        Selections[over].color = new Color32(0, 220, 50, 255);
      }
    }

    else if (num == 3) {
      if (a1 == -1) {
        a1 = pos;
        Selections[over].color = new Color32(0, 220, 50, 255);
      }
      else if (a2 == -1) {
        a2 = pos;
        Selections[over].color = new Color32(0, 220, 50, 255);
      }
      else if (a3 == -1) {
        a3 = pos;
        Selections[over].color = new Color32(0, 220, 50, 255);
      }
      else if (ak == -1) {
        ak = pos;
        akOver = over;
        Selections[over].color = new Color32(220, 50, 0, 255);
      }
    }

    else if (num == 4) {
      if (akOver != -1)
        Selections[akOver].color = new Color32(0, 0, 0, 0);
      ak = pos;
      Selections[over].color = new Color32(220, 50, 0, 255);
      akOver = over;
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
      over = -1;
      return;
    }
    over = num;
    Actor a = Controller.GetActorForSelection(num);
    ActorPortraitH.color = Color.white;
    ActorPortraitA.color = Color.white;
    ActorPortraitL.color = Color.white;
    ActorPortraitH.sprite = Heads[num];
    ActorPortraitA.sprite = Arms[num];
    ActorPortraitL.sprite = Legs[num];
    if (a == null)
      ActorDescription.text = "Not yet available:\n" + (Chars)num;
    else {
      string descr = a.Description + "\n\n<i>Skills</u>:\n";
      foreach (Skill s in a.skills)
        descr += " - " + s.ToString() + "\n";
      ActorDescription.text = descr;

    }
  }

  public void StartGame() {
    GD.actor1 = (Chars)a1;
    GD.actor2 = (Chars)a2;
    GD.actor3 = (Chars)a3;
    GD.kidnapped = (Chars)ak;
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
    Confirm.Show("Are you sure you want to quit?", 0);
  }
}

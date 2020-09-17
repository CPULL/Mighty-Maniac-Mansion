using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ActionAndCondition {
  [SerializeField] public GameCondition Condition;
  [SerializeField] public GameAction Action;
}

[System.Serializable]
public class GameAction {
  private Running running = Running.NotStarted;
  private float time;

  public ActionType type;
  public bool Repeatable;
  public string ID;
  public Chars actor;
  public Vector2 pos;
  public string Value;
  public Dir dir;
  public float delay;
  public GameItem Item;
  



  public GameAction(string stype) {
    string t = stype.ToLowerInvariant();
    if (t == "synchro") type = ActionType.Synchro;
    if (t == "teleport") type = ActionType.Teleport;
    if (t == "speak") type = ActionType.Speak;
    if (t == "move") type = ActionType.Move;
    if (t == "expression") type = ActionType.Expression;
    if (t == "open") type = ActionType.Open;
    if (t == "enable") type = ActionType.Enable;
    if (t == "showroom") type = ActionType.ShowRoom;
    if (t == "setsequence") type = ActionType.SetSequence;
    if (t == "sound") type = ActionType.Sound;
  }

  internal void SetActor(string a) {
    if (a == null) {
      actor = Chars.None;
      return;
    }
    string n = a.ToLowerInvariant();
    if (n == "none") actor = Chars.None;
    if (n == "fred") actor = Chars.Fred;
    if (n == "edna") actor = Chars.Edna;
    if (n == "ted") actor = Chars.Ted;
    if (n == "ed") actor = Chars.Ed;
    if (n == "edwige") actor = Chars.Edwige;
    if (n == "greententacle") actor = Chars.GreenTentacle;
    if (n == "purpletentacle") actor = Chars.PurpleTentacle;
    if (n == "actor1") actor = Chars.Actor1;
    if (n == "actor2") actor = Chars.Actor2;
    if (n == "actor3") actor = Chars.Actor3;
    if (n == "kidnappedactor") actor = Chars.KidnappedActor;
    if (n == "dave") actor = Chars.Dave;
    if (n == "bernard") actor = Chars.Bernard;
    if (n == "hoagie") actor = Chars.Hoagie;
    if (n == "michael") actor = Chars.Michael;
    if (n == "razor") actor = Chars.Razor;
    if (n == "sandy") actor = Chars.Sandy;
    if (n == "syd") actor = Chars.Syd;
    if (n == "wendy") actor = Chars.Wendy;
    if (n == "jeff") actor = Chars.Jeff;
    if (n == "javid") actor = Chars.Javid;
  }

  //internal void SetOther(string a) {
  //  if (a == null) {
  //    other = Chars.None;
  //    return;
  //  }
  //  string n = a.ToLowerInvariant();
  //  if (n == "none") other = Chars.None;
  //  if (n == "fred") other = Chars.Fred;
  //  if (n == "edna") other = Chars.Edna;
  //  if (n == "ted") other = Chars.Ted;
  //  if (n == "ed") other = Chars.Ed;
  //  if (n == "edwige") other = Chars.Edwige;
  //  if (n == "greententacle") other = Chars.GreenTentacle;
  //  if (n == "purpletentacle") other = Chars.PurpleTentacle;
  //  if (n == "actor1") other = Chars.Actor1;
  //  if (n == "actor2") other = Chars.Actor2;
  //  if (n == "actor3") other = Chars.Actor3;
  //  if (n == "kidnappedactor") other = Chars.KidnappedActor;
  //  if (n == "dave") other = Chars.Dave;
  //  if (n == "bernard") other = Chars.Bernard;
  //  if (n == "hoagie") other = Chars.Hoagie;
  //  if (n == "michael") other = Chars.Michael;
  //  if (n == "razor") other = Chars.Razor;
  //  if (n == "sandy") other = Chars.Sandy;
  //  if (n == "syd") other = Chars.Syd;
  //  if (n == "wendy") other = Chars.Wendy;
  //  if (n == "jeff") other = Chars.Jeff;
  //  if (n == "javid") other = Chars.Javid;
  //}

  internal void SetDir(string value) {
    string d = value.ToLowerInvariant();
    if (d == "b") dir = Dir.B;
    if (d == "f") dir = Dir.F;
    if (d == "l") dir = Dir.L;
    if (d == "r") dir = Dir.R;
  }

  internal void SetPos(float x, float y) {
    pos = new Vector2(x, y);
  }

  internal void SetValue(string txt) {
    Value = txt;
  }
  internal void SetID(string txt) {
    ID = txt;
  }

  internal void SetWait(float w) {
    delay = w;
  }

  internal void Play() {
    running = Running.Running;
    time = delay;
  }

  internal void Complete() {
    running = Running.Completed;
  }

  internal bool IsCompleted() {
    return running == Running.Completed;
  }

  internal bool IsPlaying() {
    return running == Running.Running;
  }

  internal bool NotStarted() {
    return running == Running.NotStarted;
  }

  public override string ToString() {
    string res = type.ToString();
    if (type == ActionType.Teleport) res += " " + actor.ToString();
    if (type == ActionType.Speak) res += " " + actor.ToString() + " : " + Value;
    if (type == ActionType.Expression) res += " " + actor.ToString() + " : " + Value;
    // FIXME do the others

    return res;
  }

  internal void CheckTime(float deltaTime) {
    if (time > 0) {
      time -= deltaTime;
      if (time <= 0) {
        running = Running.Completed;
        time = delay;
      }
    }
  }

  internal void Reset() {
    running = Running.NotStarted;
    time = delay;
  }
}


[CustomPropertyDrawer(typeof(GameAction))]
public class MyActionPropertyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    EditorGUI.BeginProperty(position, label, property);

    int indent = EditorGUI.indentLevel;
    float labw = EditorGUIUtility.labelWidth;
    EditorGUI.indentLevel = 1;
    EditorGUIUtility.labelWidth /= 4;

    Rect rect  = new Rect(position.x, position.y, position.width * .8f - 8, EditorGUIUtility.singleLineHeight);
    Rect rectr  = new Rect(position.x + position.width * .8f, position.y, position.width * .1f - 4, EditorGUIUtility.singleLineHeight);
    Rect rectErr  = new Rect(position.x + position.width * .8f + 32, position.y, position.width * .1f - 4, EditorGUIUtility.singleLineHeight);
    Rect rect1 = new Rect(position.x, position.y + 1 * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
    Rect rect2 = new Rect(position.x, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
    Rect rect3 = new Rect(position.x, position.y + 3 * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
    Rect rect4 = new Rect(position.x, position.y + 4 * EditorGUIUtility.singleLineHeight + 5, position.width, EditorGUIUtility.singleLineHeight);

    SerializedProperty type = property.FindPropertyRelative("type");

    type.isExpanded = EditorGUI.Foldout(rect, type.isExpanded, "");
    type.intValue = EditorGUI.Popup(rect, "Type", type.intValue, type.enumNames);
    if (type.isExpanded) {
      SerializedProperty repeat = property.FindPropertyRelative("Repeatable");
      repeat.boolValue = EditorGUI.Toggle(rectr, "", repeat.boolValue);
      EditorGUIUtility.labelWidth *= 2;

      SerializedProperty id = property.FindPropertyRelative("ID");
      SerializedProperty pos = property.FindPropertyRelative("pos");
      SerializedProperty actor = property.FindPropertyRelative("actor");
      SerializedProperty val = property.FindPropertyRelative("Value");
      SerializedProperty dir = property.FindPropertyRelative("dir");
      SerializedProperty delay = property.FindPropertyRelative("delay");
      SerializedProperty item = property.FindPropertyRelative("Item");

      switch ((ActionType)type.intValue) {
        case ActionType.ShowRoom:
          id.stringValue = EditorGUI.TextField(rect1, "ID", id.stringValue);
          pos.vector2Value = EditorGUI.Vector2Field(rect2, "Pos", pos.vector2Value);
          val.stringValue = EditorGUI.TextField(rect3, "Status", val.stringValue);
          if (string.IsNullOrEmpty(id.stringValue) || string.IsNullOrEmpty(val.stringValue) || pos.vector2Value == Vector2.zero) {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;
            EditorGUI.LabelField(rectErr, "INVALID!", style);
          }
          break;

        case ActionType.Teleport:
          actor.intValue = EditorGUI.Popup(rect1, "Actor", actor.intValue, actor.enumNames);
          dir.intValue = EditorGUI.Popup(rect2, "Dir", dir.intValue, dir.enumNames);
          pos.vector2Value = EditorGUI.Vector2Field(rect3, "Pos", pos.vector2Value);
          if (actor.intValue < 1 || string.IsNullOrEmpty(val.stringValue) || pos.vector2Value == Vector2.zero) {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;
            EditorGUI.LabelField(rectErr, "INVALID!", style);
          }
          break;

        case ActionType.Speak:
          actor.intValue = EditorGUI.Popup(rect1, "Actor", actor.intValue, actor.enumNames);
          dir.intValue = EditorGUI.Popup(rect2, "Dir", dir.intValue, dir.enumNames);
          val.stringValue = EditorGUI.TextField(rect3, "Text", val.stringValue);
          if (actor.intValue < 1 || string.IsNullOrEmpty(val.stringValue)) {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;
            EditorGUI.LabelField(rectErr, "INVALID!", style);
          }
          break;

        case ActionType.Expression:
          actor.intValue = EditorGUI.Popup(rect1, "Actor", actor.intValue, actor.enumNames);
          dir.intValue = EditorGUI.Popup(rect2, "Dir", dir.intValue, dir.enumNames);
          val.stringValue = EditorGUI.TextField(rect3, "Expression", val.stringValue);
          delay.floatValue = EditorGUI.FloatField(rect4, "Delay", delay.floatValue);
          if (actor.intValue < 1 || string.IsNullOrEmpty(val.stringValue) || delay.floatValue <= 0) {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;
            EditorGUI.LabelField(rectErr, "INVALID!", style);
          }
          break;

        case ActionType.Sound:
          actor.intValue = EditorGUI.Popup(rect1, "Actor", actor.intValue, actor.enumNames);
          dir.intValue = EditorGUI.Popup(rect2, "Dir", dir.intValue, dir.enumNames);
          val.stringValue = EditorGUI.TextField(rect3, "Value", val.stringValue);
          delay.floatValue = EditorGUI.FloatField(rect4, "Delay", delay.floatValue);
          if (string.IsNullOrEmpty(val.stringValue) || delay.floatValue <= 0) {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;
            EditorGUI.LabelField(rectErr, "INVALID!", style);
          }
          break;

        case ActionType.Enable:
          id.stringValue = EditorGUI.TextField(rect1, "ID", id.stringValue);
          item.objectReferenceValue = EditorGUI.ObjectField(rect2, "Item", item.objectReferenceValue, typeof(Item), true);
          val.stringValue = EditorGUI.TextField(rect3, "Action", val.stringValue);
          delay.floatValue = EditorGUI.FloatField(rect4, "Delay", delay.floatValue);

          int.TryParse(val.stringValue, out int tmp);
          if (!System.Enum.IsDefined(typeof(EnableMode), tmp) || delay.floatValue <= 0 || item.objectReferenceValue == null || (item.objectReferenceValue is Item && tmp != 301 && tmp != 302 && tmp != 303)) {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;
            EditorGUI.LabelField(rectErr, "INVALID!", style);
          }
          break;
      }

    }

    EditorGUI.indentLevel = indent;
    EditorGUIUtility.labelWidth = labw;
    EditorGUI.EndProperty();
  }

  //This will need to be adjusted based on what you are displaying
  public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
    SerializedProperty type = property.FindPropertyRelative("type");
    if (!type.isExpanded) return EditorGUIUtility.singleLineHeight * 1;
    switch ((ActionType)type.intValue) {
      case ActionType.ShowRoom: return EditorGUIUtility.singleLineHeight * 4;
      case ActionType.Teleport: return EditorGUIUtility.singleLineHeight * 4;
      case ActionType.Speak: return EditorGUIUtility.singleLineHeight * 4;
      case ActionType.Expression: return EditorGUIUtility.singleLineHeight * 5 + 5;
      case ActionType.Sound: return EditorGUIUtility.singleLineHeight * 5 + 5;
      case ActionType.Enable: return EditorGUIUtility.singleLineHeight * 5 + 5;
    }
    return EditorGUIUtility.singleLineHeight * 1;
  }
}
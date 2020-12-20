using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Condition))]
public class ConditionPropertyDrawer : PropertyDrawer {
  private readonly string[] isIsNot = { "Is Not", "Is" };
  private readonly string[] hasHasNot = { "Has Not", "Has" };
  private readonly string[] lessMore = { "is More", "is Less" };
  private readonly string[] openLocked = { "Open", "Locked" };
  private readonly string[] withWithNot = { "with not", "with" };
  private readonly string[] equalDifferent = { "!=", "==" };
  private readonly string[] comparison = { "", "!=", "==", "<", ">" };
  private readonly string[] internalExternal = { "is external", "is intrnal" };
  private readonly string[] participantsArray = { "Nobody", "Actor1", "Actor2", "Actor3", "Kidnapped", "A Player" };


  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    EditorGUI.BeginProperty(position, label, property);
    int indent = EditorGUI.indentLevel;
    float labw = EditorGUIUtility.labelWidth;
    EditorGUI.indentLevel = 1;

    SerializedProperty type = property.FindPropertyRelative("type");
    SerializedProperty id1 = property.FindPropertyRelative("id");
    SerializedProperty iv1 = property.FindPropertyRelative("iv");
    SerializedProperty fv1 = property.FindPropertyRelative("fv");
    SerializedProperty sv = property.FindPropertyRelative("sv");
    SerializedProperty bv = property.FindPropertyRelative("bv");
    SerializedProperty msg = property.FindPropertyRelative("msg");
    SerializedProperty when = property.FindPropertyRelative("when");

    string name = Condition.StringName((ConditionType)type.intValue, id1.intValue, iv1.intValue, fv1.floatValue, sv.stringValue, bv.boolValue);

    float w4 = position.width * .25f;
    float lh = EditorGUIUtility.singleLineHeight;

    Rect titleR = new Rect(position.x, position.y, position.width * .5f, lh);
    Rect whenR = new Rect(position.x + position.width * .5f, position.y, position.width * .2f, lh);
    Rect msgR = new Rect(position.x + position.width * .7f, position.y, position.width * .3f, lh);
    EditorGUI.LabelField(titleR, "Condition: " + name, EditorStyles.boldLabel);
    when.intValue = EditorGUI.Popup(whenR, when.intValue, when.enumDisplayNames);
    msg.stringValue = EditorGUI.TextField(msgR, msg.stringValue);
    property.isExpanded = EditorGUI.Foldout(titleR, property.isExpanded, new GUIContent(""));

    if (property.isExpanded) {
      Rect tRect = new Rect(position.x, position.y + lh, w4, lh);
      type.intValue = EditorGUI.Popup(tRect, type.intValue, type.enumDisplayNames);


      ConditionType t = (ConditionType)type.intValue;
      switch (t) {
        case ConditionType.None: break;
        case ConditionType.CurrentActorIs:
        case ConditionType.ActorIs: {
          Rect aRect = new Rect(position.x + w4 * 1, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + w4 * 2, position.y + lh, w4, lh);
          bv.intValue = EditorGUI.Popup(aRect, bv.intValue, isIsNot);
          id1.intValue = EditorGUI.Popup(bRect, id1.intValue, System.Enum.GetNames(typeof(Chars)));
        }
        break;

        case ConditionType.ActorSet: {
          Rect aRect = new Rect(position.x + w4 * 1, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + w4 * 2, position.y + lh, w4, lh);
          Rect cRect = new Rect(position.x + w4 * 3, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, participantsArray);
          bv.intValue = EditorGUI.Popup(bRect, bv.intValue, isIsNot);
          iv1.intValue = EditorGUI.Popup(cRect, iv1.intValue, System.Enum.GetNames(typeof(Chars)));
        }
        break;

        case ConditionType.ActorHasSkill: {
          Rect aRect = new Rect(position.x + w4 * 1, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + w4 * 2, position.y + lh, w4, lh);
          Rect cRect = new Rect(position.x + w4 * 3, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(Chars)));
          bv.intValue = EditorGUI.Popup(bRect, bv.intValue, hasHasNot);
          iv1.intValue = EditorGUI.Popup(cRect, iv1.intValue, System.Enum.GetNames(typeof(Skill)));
        }
        break;

        case ConditionType.CurrentRoomIs: {
          Rect aRect = new Rect(position.x + w4 * 1, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + w4 * 2, position.y + lh, w4, lh);
          bv.intValue = EditorGUI.Popup(aRect, bv.intValue, isIsNot);
          sv.stringValue = EditorGUI.TextField(bRect, sv.stringValue);
        }
        break;

        case ConditionType.RoomIsInExt: {
          Rect aRect = new Rect(position.x + w4 * 1, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + w4 * 2, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(Chars)));
          bv.intValue = EditorGUI.Popup(bRect, bv.intValue, internalExternal);
        }
        break;

        case ConditionType.SameRoom: {
          Rect aRect = new Rect(position.x + w4 * 1, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + w4 * 2, position.y + lh, w4, lh);
          Rect cRect = new Rect(position.x + w4 * 3, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(Chars)));
          bv.intValue = EditorGUI.Popup(bRect, bv.intValue, isIsNot);
          iv1.intValue = EditorGUI.Popup(cRect, id1.intValue, System.Enum.GetNames(typeof(Chars)));
        }
        break;

        case ConditionType.FlagValueIs: {
          float wh = w4 * .5f;
          Rect aRect = new Rect(position.x + w4 * 1, position.y + lh, w4, lh);
          Rect b1Rect = new Rect(position.x + w4 * 2, position.y + lh, wh, lh);
          Rect b2Rect = new Rect(position.x + w4 * 2 + wh, position.y + lh, wh, lh);
          Rect cRect = new Rect(position.x + w4 * 3, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(GameFlag)));
          int pos = 0;
          switch(sv.stringValue) {
            case "!=": pos = 1; break;
            case "==": pos = 2; break;
            case "<": pos = 3; break;
            case ">": pos = 4; break;
          }
          if (pos == 0) bv.intValue = EditorGUI.Popup(b1Rect, bv.intValue, equalDifferent);
          sv.stringValue = comparison[EditorGUI.Popup(b2Rect, pos, comparison)];
          iv1.intValue = EditorGUI.IntField(cRect, iv1.intValue);
        }
        break;

        case ConditionType.ItemCollected: {
          Rect aRect = new Rect(position.x + w4 * 1, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + w4 * 2, position.y + lh, w4, lh);
          Rect cRect = new Rect(position.x + w4 * 3, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(ItemEnum)));
          bv.intValue = EditorGUI.Popup(bRect, bv.intValue, hasHasNot);
          iv1.intValue = EditorGUI.Popup(cRect, iv1.intValue, System.Enum.GetNames(typeof(Chars)));
        }
        break;

        case ConditionType.ActorInRoom: {
          Rect aRect = new Rect(position.x + w4 * 1, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + w4 * 2, position.y + lh, w4, lh);
          Rect cRect = new Rect(position.x + w4 * 3, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(Chars)));
          bv.intValue = EditorGUI.Popup(bRect, bv.intValue, isIsNot);
          sv.stringValue = EditorGUI.TextField(cRect, sv.stringValue);
        }
        break;

        case ConditionType.ActorDistanceLess: {
          Rect aRect = new Rect(position.x + w4 * 1, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + w4 * 2, position.y + lh, w4, lh);
          Rect cRect = new Rect(position.x + w4 * 3, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(Chars)));
          bv.intValue = EditorGUI.Popup(bRect, bv.intValue, lessMore);
          fv1.floatValue = EditorGUI.FloatField(cRect, fv1.floatValue);
        }
        break;

        case ConditionType.ActorXLess: {
          Rect aRect = new Rect(position.x + w4 * 1, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + w4 * 2, position.y + lh, w4, lh);
          Rect cRect = new Rect(position.x + w4 * 3, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(Chars)));
          bv.intValue = EditorGUI.Popup(bRect, bv.intValue, lessMore);
          fv1.floatValue = EditorGUI.FloatField(cRect, fv1.floatValue);
        }
        break;

        case ConditionType.ItemOpen: {
          Rect aRect = new Rect(position.x + w4 * 1, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + w4 * 2, position.y + lh, w4, lh);
          Rect cRect = new Rect(position.x + w4 * 3, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(ItemEnum)));
          bv.intValue = EditorGUI.Popup(bRect, bv.intValue, isIsNot);
          iv1.intValue = EditorGUI.Popup(cRect, iv1.intValue, openLocked);
        }
        break;

        case ConditionType.RecipientIs: {
          Rect aRect = new Rect(position.x + w4 * 1, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + w4 * 2, position.y + lh, w4, lh);
          bv.intValue = EditorGUI.Popup(aRect, bv.intValue, isIsNot);
          id1.intValue = EditorGUI.Popup(bRect, id1.intValue, System.Enum.GetNames(typeof(Chars)));
        }
        break;

        case ConditionType.WhenIs: {
          Rect aRect = new Rect(position.x + w4 * 1, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + w4 * 2, position.y + lh, w4, lh);
          bv.intValue = EditorGUI.Popup(aRect, bv.intValue, isIsNot);
          id1.intValue = EditorGUI.Popup(bRect, id1.intValue, System.Enum.GetNames(typeof(When)));
        }
        break;

        case ConditionType.UsedWith: {
          Rect aRect = new Rect(position.x + w4 * 1, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + w4 * 2, position.y + lh, w4, lh);
          bv.intValue = EditorGUI.Popup(aRect, bv.intValue, withWithNot);
          id1.intValue = EditorGUI.Popup(bRect, id1.intValue, System.Enum.GetNames(typeof(ItemEnum)));
        }
        break;

        case ConditionType.ItemContains: {
          Rect aRect = new Rect(position.x + w4 * 1, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + w4 * 2, position.y + lh, w4, lh);
          Rect cRect = new Rect(position.x + w4 * 3, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(ItemEnum)));
          bv.intValue = EditorGUI.Popup(bRect, bv.intValue, lessMore);
          iv1.intValue = EditorGUI.Popup(aRect, iv1.intValue, System.Enum.GetNames(typeof(ItemEnum)));
        }
        break;
      }

    }


    EditorGUI.indentLevel = indent;
    EditorGUIUtility.labelWidth = labw;
    EditorGUI.EndProperty();
  }

  public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
    float h = base.GetPropertyHeight(property, label);
    if (property.isExpanded) return h + EditorGUIUtility.singleLineHeight;
    return h;
  }
}
using UnityEditor;
using UnityEngine;

// IngredientDrawer
[CustomPropertyDrawer(typeof(AffectHolder))]
public class AffectHolderDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
                                                GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing*2;
    }

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(rect, label, property);

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        EditorGUI.DrawRect(rect, new Color(0.318897f, 0.322834f, 0.078740f));

        //Draw Affect Type Enum

        rect.height = (rect.height - EditorGUIUtility.standardVerticalSpacing) / 2f;
        AffectType affectType;
        SerializedProperty affectTypeProp = property.FindPropertyRelative("affectType");
        EditorGUI.PropertyField(rect, affectTypeProp, GUIContent.none);

        affectType = (AffectType)affectTypeProp.enumValueIndex;

        //Draw Affect parmetres
        rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
        DrawAffectParametres(affectType, rect, property);

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    private void DrawAffectParametres(AffectType affectType, Rect rect, SerializedProperty property)
    {
        SerializedProperty firstValueProperty = property.FindPropertyRelative("firstValue");
        float firstValue = firstValueProperty.floatValue;
        SerializedProperty secondValueProperty = property.FindPropertyRelative("secondValue");
        float secondValue = secondValueProperty.floatValue;

        //SerializedProperty affect = property.FindPropertyRelative("affect");

        switch (affectType)
        {
            case AffectType.AddActionPoints:
                {
                    int ap = Mathf.FloorToInt(firstValue);
                    firstValueProperty.floatValue = ap;
                    EditorGUI.LabelField(GetFirstValueNameRect(rect), "Action Points");
                    EditorGUI.PropertyField(GetFirstValueFieldRect(rect), firstValueProperty, GUIContent.none);

                    int maxAp = Mathf.FloorToInt(secondValue);
                    secondValueProperty.floatValue = maxAp;
                    EditorGUI.LabelField(GetSecondValueNameRect(rect), "Maximum AP");
                    EditorGUI.PropertyField(GetSecondValueFieldRect(rect),secondValueProperty, GUIContent.none);
                    //DrawProperty(rect, "Maximum Action Points",secondValue);

                    //affect.SetValue(Affects.AddActionPoints(firstValue, secondValue));
                    //Debug.Log(affect.GetValue<Affect>());
                }
                break;
            case AffectType.AddBlock:
                {
                    EditorGUI.LabelField(GetFirstValueNameRect(rect), "Block Amount");
                    EditorGUI.PropertyField(GetFirstValueFieldRect(rect), firstValueProperty, GUIContent.none);

                    //affect.SetValue(Affects.AddBlock(firstValue));
                }
                break;
            case AffectType.AddHealth:
                {
                    EditorGUI.LabelField(GetFirstValueNameRect(rect), "Health Amount");
                    EditorGUI.PropertyField(GetFirstValueFieldRect(rect), firstValueProperty, GUIContent.none);

                    //affect.SetValue(Affects.AddHealth(firstValue));
                }
                break;
            case AffectType.AddPoison:
                {
                    EditorGUI.LabelField(GetFirstValueNameRect(rect), "Poison Amount");
                    EditorGUI.PropertyField(GetFirstValueFieldRect(rect), firstValueProperty, GUIContent.none);

                    //affect.SetValue(Affects.AddPoison(firstValue));
                }
                break;
            case AffectType.AddPower:
                {
                    EditorGUI.LabelField(GetFirstValueNameRect(rect), "Power Amount");
                    EditorGUI.PropertyField(GetFirstValueFieldRect(rect), firstValueProperty, GUIContent.none);

                    //affect.SetValue(Affects.AddPower(firstValue));
                }
                break;
            case AffectType.AddSpikes:
                {
                    EditorGUI.LabelField(GetFirstValueNameRect(rect), "Spikes Amount");
                    EditorGUI.PropertyField(GetFirstValueFieldRect(rect), firstValueProperty, GUIContent.none);

                    //affect.SetValue(Affects.AddSpikes(firstValue));
                }
                break;
            case AffectType.AddWeaknessOnDefense:
                {
                    int ws = Mathf.FloorToInt(firstValue);
                    firstValueProperty.floatValue = ws;
                    EditorGUI.LabelField(GetFirstValueNameRect(rect), "Weakness Stack");
                    EditorGUI.PropertyField(GetFirstValueFieldRect(rect), firstValueProperty, GUIContent.none);

                }
                break;
            case AffectType.Armor:
                {
                    EditorGUI.LabelField(GetFirstValueNameRect(rect), "Armor Amount");
                    EditorGUI.PropertyField(GetFirstValueFieldRect(rect), firstValueProperty, GUIContent.none);

                    //affect.SetValue(Affects.Armor(firstValue));
                }
                break;
            case AffectType.Attack:
                {
                    int ac = Mathf.FloorToInt(firstValue);
                    firstValueProperty.floatValue = ac;

                    EditorGUI.LabelField(GetFirstValueNameRect(rect), "Attack force");
                    EditorGUI.PropertyField(GetFirstValueFieldRect(rect), firstValueProperty, GUIContent.none);

                    EditorGUI.LabelField(GetSecondValueNameRect(rect), "Attack count");
                    EditorGUI.PropertyField(GetSecondValueFieldRect(rect), secondValueProperty, GUIContent.none);
                    //DrawProperty(rect, "Maximum Action Points",secondValue);

                    //affect.SetValue(Affects.Attack(firstValue, ac));

                }
                break;
            case AffectType.AttackOnDefense:
                {
                    EditorGUI.LabelField(GetFirstValueNameRect(rect), "Attack force");
                    EditorGUI.PropertyField(GetFirstValueFieldRect(rect), firstValueProperty, GUIContent.none);

                    //affect.SetValue(Affects.AttackOnDefense(firstValue));
                }
                break;
            case AffectType.BlockTheDamage:
                //affect.SetValue(Affects.BlockTheDamage());
                break;
            case AffectType.Discard:
                //affect.SetValue(Affects.Discard());
                break;
            case AffectType.DiscardAndAddBlockForEach:
                {
                    int bc = Mathf.FloorToInt(firstValue);
                    firstValueProperty.floatValue = bc;

                    EditorGUI.LabelField(GetFirstValueNameRect(rect), "Block Amount");
                    EditorGUI.PropertyField(GetFirstValueFieldRect(rect), firstValueProperty, GUIContent.none);

                    //affect.SetValue(Affects.DiscardAndAddBlockForEach(bc));
                }
                break;
            case AffectType.DoubleNextAffect:
                //affect.SetValue(Affects.DoubleNextAffect());
                break;
            case AffectType.DoubleBlock:
                //affect.SetValue(Affects.DoubleTheBlock());
                break;
            case AffectType.DropKickWithoutAttack:
                //affect.SetValue(Affects.DropKickWithouAttack());
                break;
            case AffectType.Exhaust:
                //affect.SetValue(Affects.Exhaust());
                break;
            case AffectType.GiveEnemyWeaknessOnHit:
                //affect.SetValue(Affects.GiveEnemyWeaknessOnHit());
                break;
            case AffectType.MultiplyBlock:
                {
                    EditorGUI.LabelField(GetFirstValueNameRect(rect), "Block multiplier");
                    EditorGUI.PropertyField(GetFirstValueFieldRect(rect), firstValueProperty, GUIContent.none);

                    //affect.SetValue(Affects.MultiplyBlock(firstValue));
                }
                break;
            case AffectType.Power:
                {
                    EditorGUI.LabelField(GetFirstValueNameRect(rect), "Damage");
                    EditorGUI.PropertyField(GetFirstValueFieldRect(rect), firstValueProperty, GUIContent.none);

                    //affect.SetValue(Affects.MultiplyBlock(firstValue));
                }
                break;
            case AffectType.PullCard:
                {
                    int cc = Mathf.FloorToInt(firstValue);
                    firstValueProperty.floatValue = cc;
                    EditorGUI.LabelField(GetFirstValueNameRect(rect), "Count");
                    EditorGUI.PropertyField(GetFirstValueFieldRect(rect), firstValueProperty, GUIContent.none);

                    //affect.SetValue(Affects.PullCard(cc));
                }
                break;
            case AffectType.SaveBlock:
                //affect.SetValue(Affects.SaveBlock());
                break;
            case AffectType.SteelBlock:
                {
                    EditorGUI.LabelField(GetFirstValueNameRect(rect), "ERORRRRRRRR");
                    EditorGUI.PropertyField(GetFirstValueFieldRect(rect), firstValueProperty, GUIContent.none);

                    //affect.SetValue(Affects.SteelBlock(firstValue));
                }
                break;
            case AffectType.TurnWeaknessIntoPoison:
                //affect.SetValue(Affects.TurnWeaknessIntoPoison());
                break;
            case AffectType.Vulnerability:
                {
                    int vc = Mathf.FloorToInt(firstValue);
                    firstValueProperty.floatValue = vc;
                    EditorGUI.LabelField(GetFirstValueNameRect(rect), "Count");
                    EditorGUI.PropertyField(GetFirstValueFieldRect(rect), firstValueProperty, GUIContent.none);

                    //affect.SetValue(Affects.Vulnerablity(vc));
                }
                break;
            case AffectType.Weakness:
                {
                    int wc = Mathf.FloorToInt(firstValue);
                    firstValueProperty.floatValue = wc;
                    EditorGUI.LabelField(GetFirstValueNameRect(rect), "Count");
                    EditorGUI.PropertyField(GetFirstValueFieldRect(rect), firstValueProperty, GUIContent.none);

                    //affect.SetValue(Affects.Weakness(wc));
                }
                break;
            default:
                break;
        }
       // Debug.Log(affect.GetValue());
    }

    public Rect GetFirstValueNameRect(Rect position)
    {
        return new Rect(position.x,
                                    position.y,
                                    position.width * 0.3f - 5,
                                    position.height);
    }
    public Rect GetFirstValueFieldRect(Rect position)
    {
        return new Rect(position.x + position.width * 0.3f - 5,
                                     position.y,
                                     position.width * 0.1f,
                                     position.height);
    }
    public Rect GetSecondValueNameRect(Rect position)
    {
        return new Rect(position.x + position.width * 0.5f + 5,
                                position.y,
                                position.width * 0.3f - 5,
                                position.height);
    }
    public Rect GetSecondValueFieldRect(Rect position)
    {
        return new Rect(position.x + position.width * 0.8f + 5,
                                position.y,
                                position.width * 0.1f - 5,
                                position.height);
    }
}

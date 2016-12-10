using System;
using PachowStudios.Framework.Attributes;
using UnityEditor;
using UnityEngine;

namespace PachowStudios.Framework.Editor
{
  [CustomPropertyDrawer(typeof(BitMaskAttribute))]
  public class BitMaskPropertyDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
      label.text = label.text;
      prop.intValue = DrawBitMaskField(position, prop.intValue, fieldInfo.FieldType, label);
    }

    private static int DrawBitMaskField(Rect rect, int mask, Type type, GUIContent label)
    {
      var itemNames = Enum.GetNames(type);
      var itemValues = (int[])Enum.GetValues(type);
      var maskValue = 0;
      var maskResult = mask;

      for (var i = 0; i < itemValues.Length; i++)
      {
        if (itemValues[i] != 0)
        {
          if ((mask & itemValues[i]) == itemValues[i])
            maskValue |= 1 << i;
        }
        else if (mask == 0)
          maskValue |= 1 << i;
      }

      var newMaskVal = EditorGUI.MaskField(rect, label, maskValue, itemNames);
      var changes = maskValue ^ newMaskVal;

      for (var i = 0; i < itemValues.Length; i++)
      {
        if ((changes & (1 << i)) == 0)
          continue;

        if ((newMaskVal & (1 << i)) == 0)
        {
          maskResult &= ~itemValues[i];
          continue;
        }

        if (itemValues[i] == 0)
          return 0;

        maskResult |= itemValues[i];
      }

      return maskResult;
    }
  }
}
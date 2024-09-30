
#if UNITY_EDITOR
using Scripts.Framework.Utility;
using Spine;
using UnityEditor;
using UnityEngine;
using static Define;



[CustomEditor(typeof(Skin_Btn_SubUI))]

public class SkinBtnSubUI_Editor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Custom Skin Settings", EditorStyles.boldLabel);

        var skinBtnSubUI = (Skin_Btn_SubUI)target;
        var skinSettings = skinBtnSubUI.Skin;

        if (skinSettings == null)
        {
            EditorGUILayout.HelpBox("Skin is not set.", MessageType.Warning);
            return;
        }

        skinSettings.skinType = (SkinTypeEnum)EditorGUILayout.EnumPopup("Skin Type", skinSettings.skinType);

        if (skinSettings.skinType != SkinTypeEnum.None) { 
        switch (skinSettings.skinType)
        {
            case SkinTypeEnum.Cat:
                skinSettings.isRandom = EditorGUILayout.Toggle("Is Random", skinSettings.isRandom);
                if(!skinSettings.isRandom) skinSettings.catSkinName = (CatSkinName)EditorGUILayout.EnumPopup("Cat Skin Name", skinSettings.catSkinName);
                break;

            case SkinTypeEnum.Pudding:
                skinSettings.isRandom = EditorGUILayout.Toggle("Is Random", skinSettings.isRandom);
                if (!skinSettings.isRandom) skinSettings.puddingSkinName = (PuddingSkinName)EditorGUILayout.EnumPopup("Pudding Skin Name", skinSettings.puddingSkinName);
                break;

            case SkinTypeEnum.BG:
                skinSettings.isRandom = EditorGUILayout.Toggle("Is Random", skinSettings.isRandom);
                if (!skinSettings.isRandom) skinSettings.bgSkinName = (BGSkinName)EditorGUILayout.EnumPopup("BG Skin Name", skinSettings.bgSkinName);
                break;
        }
            }


        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(skinBtnSubUI);
        }

        serializedObject.ApplyModifiedProperties();
    }
}

#endif

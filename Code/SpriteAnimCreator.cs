//Autor: Daniel Barreno.
//Date: 24/11/2020.

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpriteAnimCreator : Editor
{
    const float REFERENCE_FPS = 30;
    const string ANIMATOR_PROPERTY_NAME = "m_Sprite";

    [MenuItem("Tools/Create animation from sprites")]
    public static void CreateSpriteFromAnimation()
    {
        List<Sprite> selectedSprites = new List<Sprite>();

        for (int i = 0; i < Selection.objects.Length; i++)
        {
            if (Selection.objects[i].GetType() == typeof(Texture2D))
            {
                selectedSprites.Add(AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(Selection.objects[i])));
            }
        }   

        if(selectedSprites.Count != 0)
        {
            AnimationClip animClip = new AnimationClip();
            animClip.wrapMode = WrapMode.Loop;
            animClip.frameRate = REFERENCE_FPS;

            AnimationClipSettings tSettings = AnimationUtility.GetAnimationClipSettings(animClip);
            tSettings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(animClip, tSettings);

            EditorCurveBinding spriteBinding = new EditorCurveBinding();
            spriteBinding.type = typeof(SpriteRenderer);
            spriteBinding.path = string.Empty;
            spriteBinding.propertyName = ANIMATOR_PROPERTY_NAME;

            ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[selectedSprites.Count + 1];
            float animDuration = spriteKeyFrames.Length / REFERENCE_FPS;

            for (int i = 0; i < spriteKeyFrames.Length; i++)
            {
                spriteKeyFrames[i] = new ObjectReferenceKeyframe();
                spriteKeyFrames[i].time = ((float)i / (float)spriteKeyFrames.Length) * animDuration;
                spriteKeyFrames[i].value = i != spriteKeyFrames.Length - 1 ? selectedSprites[i] : selectedSprites[i - 1];
            }

            string fileName = EditorUtility.SaveFilePanel("Export Animation", "Animations", "New Animation", "anim");

            if (!string.IsNullOrEmpty(fileName) && fileName.StartsWith(Application.dataPath))
            {
                fileName = "Assets" + fileName.Substring(Application.dataPath.Length);

                AnimationUtility.SetObjectReferenceCurve(animClip, spriteBinding, spriteKeyFrames);
                AssetDatabase.CreateAsset(animClip, fileName);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorUtility.FocusProjectWindow();
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<AnimationClip>(fileName);
            }
        }
    }
}

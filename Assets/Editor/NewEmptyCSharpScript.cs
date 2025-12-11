using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class AssetTest
{
    [MenuItem("Tools/Make animations from sprites")]
    public static void TestAssets()
    {
        string[] subFolders = Directory.GetDirectories("Assets/Sprites/Characters/TODO", "*", SearchOption.AllDirectories);
        foreach (string sub in subFolders)
        {
            int index = sub.IndexOf("TODO", System.StringComparison.OrdinalIgnoreCase);
            string relativePath = sub.Substring(index + "TODO".Length + 1);

            string parent = "Assets/Animations/TODO";
            foreach (string folder in relativePath.Split('/', '\\')) // handle both slashes
            {
                string path = parent + "/" + folder;
                if (!AssetDatabase.IsValidFolder(path))
                {
                    string guid = AssetDatabase.CreateFolder(parent, folder);
                    path = AssetDatabase.GUIDToAssetPath(guid);
                }
                parent = path;
            }
        }
        
        string[] guids = AssetDatabase.FindAssets("t:texture2D", new[] { "Assets/Sprites/Characters/TODO" });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(path);

            List<Sprite> sprites = new List<Sprite>();
            foreach (Object s in allAssets)
            {
                if (s.GetType() == typeof(Sprite))
                {
                    sprites.Add((Sprite)s);
                }
            }

            sprites.Sort((a, b) =>
            {
                int GetNumber(string name)
                {
                    Match match = Regex.Match(name, @"_(\d+)$");
                    return match.Success ? int.Parse(match.Groups[1].Value) : 0;
                }

                int numA = GetNumber(a.name);
                int numB = GetNumber(b.name);

                return numA.CompareTo(numB);
            });

            AnimationClip clip = new AnimationClip();

            // The property we are animating: SpriteRenderer.sprite
            EditorCurveBinding binding = new EditorCurveBinding
            {
                type = typeof(SpriteRenderer),
                path = "", // "" = same GameObject this Animation will be attached to
                propertyName = "m_Sprite"
            };

            // Create keyframes for each sprite
            ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[sprites.Count + 1];

            for (int i = 0; i < sprites.Count; i++)
            {
                keyFrames[i] = new ObjectReferenceKeyframe
                {
                    time = i * 0.1667f, // each frame is 10ms = 0.01f
                    value = sprites[i]
                };
                Debug.Log(sprites[i].name);
            }

            keyFrames[sprites.Count] = new ObjectReferenceKeyframe
            {
                time = sprites.Count * 0.16666667f,
                value = sprites[sprites.Count - 1]
            };

            // Assign keyframes to clip
            AnimationUtility.SetObjectReferenceCurve(clip, binding, keyFrames);

            int index = path.IndexOf("TODO", System.StringComparison.OrdinalIgnoreCase) + "TODO".Length + 1;
            int index2 = path.IndexOf(".png", System.StringComparison.OrdinalIgnoreCase);
            string relativePath = path.Substring(index, index2 - index);

            AssetDatabase.CreateAsset(clip, "Assets/Animations/TODO/" + relativePath + ".anim");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }


    [MenuItem("Tools/Make animation controllers")]
    public static void makeAnimation()
    {
        string[] subFolders = Directory.GetDirectories("Assets/Animations/TODO", "*", SearchOption.TopDirectoryOnly);
        foreach (string sub in subFolders)
        {
            int index = sub.IndexOf("TODO", System.StringComparison.OrdinalIgnoreCase);
            string relativePath = sub.Substring(index + "TODO".Length + 1).Replace(" ", "");
            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(sub + "\\" + relativePath + ".controller");

            string[] guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { sub });

            Dictionary<string, AnimatorState> dict = new Dictionary<string, AnimatorState>();
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);

                AnimatorState state;
                if (clip.name == "Idle")
                {
                    state = controller.AddMotion(clip);
                    controller.layers[0].stateMachine.defaultState = state;
                }

                else
                {
                    state = controller.AddMotion(clip);
                }

                dict[clip.name] = state;
            }

            controller.AddParameter("Damage", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Death", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("XVelocity", AnimatorControllerParameterType.Int);
            controller.AddParameter("YVelocity", AnimatorControllerParameterType.Int);

            AnimatorStateTransition transition;
            if (dict.ContainsKey("Idle") && dict.ContainsKey("Run"))
            {
                transition = dict["Idle"].AddTransition(dict["Run"]);
                transition.duration = 0;
                transition.hasExitTime = false;
                transition.hasFixedDuration = false;
                transition.AddCondition(AnimatorConditionMode.NotEqual, 0, "XVelocity");
            }

            if (dict.ContainsKey("Idle") && dict.ContainsKey("Jump"))
            {
                transition = dict["Idle"].AddTransition(dict["Jump"]);
                transition.duration = 0;
                transition.hasExitTime = false;
                transition.hasFixedDuration = false;
                transition.AddCondition(AnimatorConditionMode.Greater, 0, "YVelocity");
            }

            if (dict.ContainsKey("Idle") && dict.ContainsKey("Fall"))
            {
                transition = dict["Idle"].AddTransition(dict["Fall"]);
                transition.duration = 0;
                transition.hasExitTime = false;
                transition.hasFixedDuration = false;
                transition.AddCondition(AnimatorConditionMode.Less, 0, "YVelocity");
            }

            if (dict.ContainsKey("Jump") && dict.ContainsKey("Idle"))
            {
                transition = dict["Jump"].AddTransition(dict["Idle"]);
                transition.duration = 0;
                transition.hasExitTime = false;
                transition.hasFixedDuration = false;
                transition.AddCondition(AnimatorConditionMode.Equals, 0, "YVelocity");
            }

            if (dict.ContainsKey("Jump") && dict.ContainsKey("Fall"))
            {
                transition = dict["Jump"].AddTransition(dict["Fall"]);
                transition.duration = 0;
                transition.hasExitTime = false;
                transition.hasFixedDuration = false;
                transition.AddCondition(AnimatorConditionMode.Less, 0, "YVelocity");
            }

            if (dict.ContainsKey("Fall") && dict.ContainsKey("Idle"))
            {
                transition = dict["Fall"].AddTransition(dict["Idle"]);
                transition.duration = 0;
                transition.hasExitTime = false;
                transition.hasFixedDuration = false;
                transition.AddCondition(AnimatorConditionMode.Equals, 0, "YVelocity");
            }

            if (dict.ContainsKey("Fall") && dict.ContainsKey("Jump"))
            {
                transition = dict["Fall"].AddTransition(dict["Jump"]);
                transition.duration = 0;
                transition.hasExitTime = false;
                transition.hasFixedDuration = false;
                transition.AddCondition(AnimatorConditionMode.Greater, 0, "YVelocity");
            }

            if (dict.ContainsKey("Run") && dict.ContainsKey("Idle"))
            {
                transition = dict["Run"].AddTransition(dict["Idle"]);
                transition.duration = 0;
                transition.hasExitTime = false;
                transition.hasFixedDuration = false;
                transition.AddCondition(AnimatorConditionMode.Equals, 0, "XVelocity");
            }

            if (dict.ContainsKey("Run") && dict.ContainsKey("Jump"))
            {
                transition = dict["Run"].AddTransition(dict["Jump"]);
                transition.duration = 0;
                transition.hasExitTime = false;
                transition.hasFixedDuration = false;
                transition.AddCondition(AnimatorConditionMode.Greater, 0, "YVelocity");
            }

            if (dict.ContainsKey("Run") && dict.ContainsKey("Fall"))
            {
                transition = dict["Run"].AddTransition(dict["Fall"]);
                transition.duration = 0;
                transition.hasExitTime = false;
                transition.hasFixedDuration = false;
                transition.AddCondition(AnimatorConditionMode.Less, 0, "YVelocity");
            }

            if (dict.ContainsKey("Damage"))
            {
                transition = controller.layers[0].stateMachine.AddAnyStateTransition(dict["Damage"]);
                transition.duration = 0;
                transition.hasExitTime = false;
                transition.hasFixedDuration = false;
                transition.AddCondition(AnimatorConditionMode.If, 0, "Damage");
            }

            if (dict.ContainsKey("Death"))
            {
                transition = controller.layers[0].stateMachine.AddAnyStateTransition(dict["Death"]);
                transition.duration = 0;
                transition.hasExitTime = false;
                transition.hasFixedDuration = false;
                transition.AddCondition(AnimatorConditionMode.If, 0, "Death");
            }

            if (dict.ContainsKey("Damage") && dict.ContainsKey("Idle"))
            {
                transition = dict["Damage"].AddTransition(dict["Idle"]);
                transition.duration = 0;
                transition.hasExitTime = true;
                transition.hasFixedDuration = true;
                transition.duration = 0f;
                transition.offset = 0f;
                transition.exitTime = 1f;
            }

            var states = controller.layers[0].stateMachine.states;
            for (int i = 0; i < states.Length; i++)
            {
                var state = states[i];

                if (state.state.name == "Idle")
                {
                    state.position = new Vector3(30, 190, 0);
                }

                if (state.state.name == "Damage")
                {
                    state.position = new Vector3(-220, 20, 0);
                }

                if (state.state.name == "Fall")
                {
                    state.position = new Vector3(240, 280, 0);
                }

                if (state.state.name == "Run")
                {
                    state.position = new Vector3(30, 360, 0);
                }

                if (state.state.name == "Jump")
                {
                    state.position = new Vector3(-180, 280, 0);
                }

                if (state.state.name == "Death")
                {
                    state.position = new Vector3(270, 20, 0);
                }

                if (state.state.name.Contains("Attack"))
                {
                    state.position = new Vector3(30, -60, 0);
                    controller.AddParameter(state.state.name, AnimatorControllerParameterType.Trigger);

                    transition = controller.layers[0].stateMachine.AddAnyStateTransition(dict[state.state.name]);
                    transition.duration = 0;
                    transition.hasExitTime = false;
                    transition.hasFixedDuration = false;
                    transition.AddCondition(AnimatorConditionMode.If, 0, state.state.name);

                    transition = dict[state.state.name].AddTransition(dict["Idle"]);
                    transition.hasExitTime = true;
                    transition.hasFixedDuration = true;
                    transition.duration = 0f;
                    transition.offset = 0f;
                    transition.exitTime = 1f;
                }

                states[i] = state;
            }
            controller.layers[0].stateMachine.states = states;
        }
    }
}

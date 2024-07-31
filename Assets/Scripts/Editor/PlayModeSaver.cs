using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Moonawar.Editor
{
    public class PlayModeSaver : EditorWindow
    {
        private static Dictionary<string, string> savedStates = new Dictionary<string, string>();
        private Vector2 scrollPosition;
        private Dictionary<string, bool> stateToggles = new Dictionary<string, bool>();

        [MenuItem("Window/Play Mode Saver")]
        public static void ShowWindow()
        {
            var window = GetWindow<PlayModeSaver>("Play Mode Saver");
            // Set the icon for the window
            var icon = EditorGUIUtility.IconContent("SaveAs").image as Texture2D;
            window.titleContent = new GUIContent("Saver", icon);
        }

        private void OnGUI()
        {
            if (Application.isPlaying)
            {
                DrawPlayModeGUI();
            }
            else
            {
                DrawEditModeGUI();
            }
        }

        private void DrawPlayModeGUI()
        {
            EditorGUILayout.LabelField("Save Component States", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            var selection = Selection.gameObjects;
            foreach (var go in selection)
            {
                EditorGUILayout.LabelField(go.name, EditorStyles.boldLabel);
                var components = go.GetComponents<Component>();
                foreach (var component in components)
                {
                    if (CanComponentBeSaved(component))
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.ObjectField(component, component.GetType(), true);
                        if (GUILayout.Button("Save", GUILayout.Width(50)))
                        {
                            SaveComponentState(component);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();
        }

        private bool CanComponentBeSaved(Component component)
        {
            // Check for custom handled components
            if (component is Transform ||
                component is RectTransform ||
                component is Rigidbody ||
                component is Light ||
                component is Camera)
            {
                return true;
            }

            // General check if the component has any serializable fields
            var fields = component.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            return fields.Any(field => field.IsPublic || Attribute.IsDefined(field, typeof(SerializeField)));
        }


        private void DrawEditModeGUI()
        {
            EditorGUILayout.LabelField("Saved Component States", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (savedStates.Count > 0)
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Select All"))
                {
                    foreach (var key in savedStates.Keys)
                    {
                        stateToggles[key] = true;
                    }
                }
                if (GUILayout.Button("Deselect All"))
                {
                    foreach (var key in savedStates.Keys)
                    {
                        stateToggles[key] = false;
                    }
                }
                EditorGUILayout.EndHorizontal();

                foreach (var kvp in savedStates)
                {
                    if (!stateToggles.ContainsKey(kvp.Key))
                    {
                        stateToggles[kvp.Key] = true;
                    }

                    EditorGUILayout.BeginHorizontal();
                    stateToggles[kvp.Key] = EditorGUILayout.Toggle(stateToggles[kvp.Key], GUILayout.Width(20));
                    EditorGUILayout.LabelField(kvp.Key);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();

                if (GUILayout.Button("Apply Selected States"))
                {
                    ApplySelectedSavedStates();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No saved states available. Enter Play Mode to save component states.", MessageType.Info);
            }
        }

        private void SaveComponentState(Component component)
        {
            string key = $"{component.gameObject.name}_{component.GetType().FullName}";

            if (component is Transform transform)
            {
                TransformData data = new TransformData(transform);
                savedStates[key] = JsonUtility.ToJson(data);
            }
            else if (component is RectTransform rectTransform)
            {
                RectTransformData data = new RectTransformData(rectTransform);
                savedStates[key] = JsonUtility.ToJson(data);
            }
            else if (component is Rigidbody rb)
            {
                RigidbodyData data = new RigidbodyData(rb);
                savedStates[key] = JsonUtility.ToJson(data);
            }
            else if (component is Light light)
            {
                LightData data = new LightData(light);
                savedStates[key] = JsonUtility.ToJson(data);
            }
            else if (component is Camera camera)
            {
                CameraData data = new CameraData(camera);
                savedStates[key] = JsonUtility.ToJson(data);
            }
            else
            {
                savedStates[key] = JsonUtility.ToJson(component);
            }

            stateToggles[key] = true;
            Debug.Log($"Saved state for {component.GetType().Name} on {component.gameObject.name}");
        }

        private void ApplySelectedSavedStates()
        {
            foreach (var kvp in savedStates)
            {
                if (!stateToggles[kvp.Key])
                {
                    Debug.Log($"Skipping {kvp.Key} as it's not selected.");
                    continue;
                }

                string[] parts = kvp.Key.Split(new[] { '_' }, 2);
                if (parts.Length != 2)
                {
                    Debug.LogError($"Invalid key format: {kvp.Key}");
                    continue;
                }

                string objectName = parts[0];
                string componentTypeName = parts[1];

                GameObject go = GameObject.Find(objectName);
                if (go == null)
                {
                    Debug.LogError($"GameObject not found: {objectName}");
                    continue;
                }

                Type componentType = FindType(componentTypeName);
                if (componentType == null)
                {
                    Debug.LogError($"Component type not found: {componentTypeName}");
                    continue;
                }

                Component component = go.GetComponent(componentType);
                if (component == null)
                {
                    Debug.LogError($"Component {componentTypeName} not found on {objectName}");
                    continue;
                }

                try
                {
                    if (component is Transform transform)
                    {
                        TransformData data = JsonUtility.FromJson<TransformData>(kvp.Value);
                        data.ApplyTo(transform);
                    }
                    else if (component is Rigidbody rb)
                    {
                        RigidbodyData data = JsonUtility.FromJson<RigidbodyData>(kvp.Value);
                        data.ApplyTo(rb);
                    }
                    else if (component is Light light)
                    {
                        LightData data = JsonUtility.FromJson<LightData>(kvp.Value);
                        data.ApplyTo(light);
                    }
                    else if (component is Camera camera)
                    {
                        CameraData data = JsonUtility.FromJson<CameraData>(kvp.Value);
                        data.ApplyTo(camera);
                    } else if (component is RectTransform rectTransform)
                    {
                        RectTransformData data = JsonUtility.FromJson<RectTransformData>(kvp.Value);
                        data.ApplyTo(rectTransform);
                    }
                    else
                    {
                        JsonUtility.FromJsonOverwrite(kvp.Value, component);
                    }
                    EditorUtility.SetDirty(component);
                    Debug.Log($"Successfully applied state for {componentType.Name} on {objectName}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error applying state for {componentType.Name} on {objectName}: {e.Message}");
                }
            }

            // Clear only the applied states
            var keysToRemove = savedStates.Keys.Where(key => stateToggles[key]).ToList();
            foreach (var key in keysToRemove)
            {
                savedStates.Remove(key);
                stateToggles.Remove(key);
            }

            // Force Unity to update the inspector
            EditorUtility.SetDirty(Selection.activeGameObject);
            SceneView.RepaintAll();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        private Type FindType(string typeName)
        {
            Type type = Type.GetType(typeName);
            if (type != null) return type;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(typeName);
                if (type != null) return type;
            }

            // If still not found, try to find by name without namespace
            string typeNameWithoutNamespace = typeName.Split('.').Last();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetTypes().FirstOrDefault(t => t.Name == typeNameWithoutNamespace);
                if (type != null) return type;
            }

            return null;
        }

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                EditorApplication.delayCall += OpenWindowIfStatesExist;
            }
        }

        private static void OpenWindowIfStatesExist()
        {
            if (savedStates.Count > 0)
            {
                ShowWindow();
            }
        }
    }
}
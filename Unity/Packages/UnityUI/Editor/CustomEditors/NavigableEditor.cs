using System;
using System.Collections.Generic;
using System.Linq;
using SecretPlan.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SecretPlan.UI.Editor
{
    /*
     *  This is currently turned off because it clashes with NaughtyAttributes
     */
    
    
    /// <summary>
    ///     Custom Editor for the SecretNavigable Component.
    ///     Extend this class to write a custom editor for a component derived from Selectable.
    /// </summary>
    
    // [CustomEditor(typeof(Navigable), true)]
    public class SelectableEditor : UnityEditor.Editor
    {
        private const float ArrowThickness = 2.5f;
        private const float ArrowHeadSize = 1.2f;

        private static readonly List<SelectableEditor> Editors = new();
        private static bool showNavigation;
        private static readonly string ShowNavigationKey = "SelectableEditor.ShowNavigation";

        private readonly GUIContent _visualizeNavigation =
            EditorGUIUtility.TrTextContent("Visualize", "Show navigation flows between selectable UI elements.");

        private SerializedProperty? _interactableProperty;
        private SerializedProperty? _navigationProperty;

        // Whenever adding new SerializedProperties to the Selectable and SelectableEditor
        // Also update this guy in OnEnable. This makes the inherited classes from Selectable not require a CustomEditor.
        private string[] _propertyPathToExcludeForChildClasses = Array.Empty<string>();
        private SerializedProperty? _script;

        protected void OnEnable()
        {
            _script = serializedObject.FindProperty("m_Script");
            _interactableProperty = serializedObject.FindProperty("_isInteractive");
            _navigationProperty = serializedObject.FindProperty("_navigation");

            _propertyPathToExcludeForChildClasses = new[]
            {
                _script.propertyPath,
                _navigationProperty.propertyPath,
                _interactableProperty.propertyPath
            };

            Editors.Add(this);
            RegisterStaticOnSceneGUI();

            showNavigation = EditorPrefs.GetBool(ShowNavigationKey);
        }

        protected void OnDisable()
        {
            Editors.Remove(this);
            RegisterStaticOnSceneGUI();
        }

        private void RegisterStaticOnSceneGUI()
        {
            SceneView.duringSceneGui -= StaticOnSceneGUI;
            if (Editors.Count > 0)
            {
                SceneView.duringSceneGui += StaticOnSceneGUI;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_interactableProperty);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_navigationProperty);

            EditorGUI.BeginChangeCheck();
            var toggleRect = EditorGUILayout.GetControlRect();
            toggleRect.xMin += EditorGUIUtility.labelWidth;
            showNavigation = GUI.Toggle(toggleRect, showNavigation, _visualizeNavigation, EditorStyles.miniButton);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(ShowNavigationKey, showNavigation);
                SceneView.RepaintAll();
            }

            // We do this here to avoid requiring the user to also write a Editor for their Selectable-derived classes.
            // This way if we are on a derived class we dont draw anything else, otherwise draw the remaining properties.
            ChildClassPropertiesGUI();

            serializedObject.ApplyModifiedProperties();
        }

        // Draw the extra SerializedProperties of the child class.
        // We need to make sure that m_PropertyPathToExcludeForChildClasses has all the Selectable properties and in the correct order.
        // TODO: find a nicer way of doing this. (creating a InheritedEditor class that automagically does this)
        private void ChildClassPropertiesGUI()
        {
            if (IsDerivedSelectableEditor())
            {
                return;
            }

            DrawPropertiesExcluding(serializedObject, _propertyPathToExcludeForChildClasses);
        }

        private bool IsDerivedSelectableEditor()
        {
            return GetType() != typeof(SelectableEditor);
        }

        private static void StaticOnSceneGUI(SceneView view)
        {
            if (!showNavigation)
            {
                return;
            }

            var navigables = FindObjectsOfType<Navigable>();
            foreach(var navigable in navigables)
            {
                if (StageUtility.IsGameObjectRenderedByCamera(navigable.gameObject, Camera.current))
                {
                    DrawNavigationForSelectable(navigable);
                }
            }
        }

        private static void DrawNavigationForSelectable(Navigable sel)
        {
            if (sel == null)
            {
                return;
            }

            var transform = sel.transform;
            var active = Selection.transforms.Any(e => e == transform);

            Handles.color = new Color(1.0f, 0.6f, 0.2f, active ? 1.0f : 0.4f);
            DrawNavigationArrow(-Vector2.right, sel, sel.FindNavigableOnLeft());
            DrawNavigationArrow(Vector2.up, sel, sel.FindNavigableOnUp());

            Handles.color = new Color(1.0f, 0.9f, 0.1f, active ? 1.0f : 0.4f);
            DrawNavigationArrow(Vector2.right, sel, sel.FindNavigableOnRight());
            DrawNavigationArrow(-Vector2.up, sel, sel.FindNavigableOnDown());
        }

        private static void DrawNavigationArrow(Vector2 direction, Navigable? fromObj, Navigable? toObj)
        {
            if (fromObj == null || toObj == null)
            {
                return;
            }

            var fromTransform = fromObj.transform;
            var toTransform = toObj.transform;

            var sideDir = new Vector2(direction.y, -direction.x);
            var fromPoint =
                fromTransform.TransformPoint((fromTransform as RectTransform).GetPointOnRectEdge(direction));
            var toPoint = toTransform.TransformPoint((toTransform as RectTransform).GetPointOnRectEdge(-direction));
            var fromSize = HandleUtility.GetHandleSize(fromPoint) * 0.05f;
            var toSize = HandleUtility.GetHandleSize(toPoint) * 0.05f;
            fromPoint += fromTransform.TransformDirection(sideDir) * fromSize;
            toPoint += toTransform.TransformDirection(sideDir) * toSize;
            var length = Vector3.Distance(fromPoint, toPoint);
            var fromTangent = fromTransform.rotation * direction * length * 0.3f;
            var toTangent = toTransform.rotation * -direction * length * 0.3f;

            Handles.DrawBezier(fromPoint, toPoint, fromPoint + fromTangent, toPoint + toTangent, Handles.color, null,
                ArrowThickness);
            Handles.DrawAAPolyLine(ArrowThickness, toPoint,
                toPoint + toTransform.rotation * (-direction - sideDir) * toSize * ArrowHeadSize);
            Handles.DrawAAPolyLine(ArrowThickness, toPoint,
                toPoint + toTransform.rotation * (-direction + sideDir) * toSize * ArrowHeadSize);
        }
    }
}

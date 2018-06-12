﻿using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RTLTMPro
{
    public class ContextMenu : Editor
    {
        private const string kUILayerName = "UI";

        private const string kStandardSpritePath = "UI/Skin/UISprite.psd";
        private const string kBackgroundSpritePath = "UI/Skin/Background.psd";
        private const string kInputFieldBackgroundPath = "UI/Skin/InputFieldBackground.psd";
        private const string kKnobPath = "UI/Skin/Knob.psd";
        private const string kCheckmarkPath = "UI/Skin/Checkmark.psd";
        private const string kDropdownArrowPath = "UI/Skin/DropdownArrow.psd";
        private const string kMaskPath = "UI/Skin/UIMask.psd";

        private static RTLDefaultControls.Resources s_StandardResources;

        /// <summary>
        ///     Create a TextMeshPro object that works with the CanvasRenderer
        /// </summary>
        /// <param name="command"></param>
        [MenuItem("GameObject/UI/Text - RTLTMP", false, 2001)]
        private static void CreateTextMeshProGuiObjectPerform(MenuCommand command)
        {
            // Check if there is a Canvas in the scene
            var canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                // Create new Canvas since none exists in the scene.
                var canvasObject = new GameObject("Canvas");
                canvas = canvasObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                // Add a Graphic Raycaster Component as well
                canvas.gameObject.AddComponent<GraphicRaycaster>();

                Undo.RegisterCreatedObjectUndo(canvasObject, "Create " + canvasObject.name);
            }


            // Create the RTLTextMeshPro Object
            var go = new GameObject("Text - RTLTMP");
            var goRectTransform = go.AddComponent<RectTransform>();

            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);

            // Check if object is being create with left or right click
            var contextObject = command.context as GameObject;
            if (contextObject == null)
            {
                //goRectTransform.sizeDelta = new Vector2(200f, 50f);
                GameObjectUtility.SetParentAndAlign(go, canvas.gameObject);

                var textMeshPro = go.AddComponent<RTLTextMeshPro>();
                textMeshPro.text = "";
                textMeshPro.alignment = TextAlignmentOptions.TopRight;
            }
            else
            {
                if (contextObject.GetComponent<Button>() != null)
                {
                    goRectTransform.sizeDelta = Vector2.zero;
                    goRectTransform.anchorMin = Vector2.zero;
                    goRectTransform.anchorMax = Vector2.one;

                    GameObjectUtility.SetParentAndAlign(go, contextObject);

                    var textMeshPro = go.AddComponent<RTLTextMeshPro>();
                    textMeshPro.text = "Button";
                    textMeshPro.fontSize = 24;
                    textMeshPro.alignment = TextAlignmentOptions.Center;
                }
                else
                {
                    //goRectTransform.sizeDelta = new Vector2(200f, 50f);

                    GameObjectUtility.SetParentAndAlign(go, contextObject);

                    var textMeshPro = go.AddComponent<RTLTextMeshPro>();
                    textMeshPro.text = "New Text";
                    textMeshPro.alignment = TextAlignmentOptions.TopRight;
                }
            }


            // Check if an event system already exists in the scene
            if (!FindObjectOfType<EventSystem>())
            {
                var eventObject = new GameObject("EventSystem", typeof(EventSystem));
                eventObject.AddComponent<StandaloneInputModule>();
                Undo.RegisterCreatedObjectUndo(eventObject, "Create " + eventObject.name);
            }

            Selection.activeGameObject = go;
        }


        [MenuItem("GameObject/UI/Input Field - RTLTMP", false, 2037)]
        private static void AddTextMeshProInputField(MenuCommand menuCommand)
        {
            var go = RTLDefaultControls.CreateInputField(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }
       
        private static RTLDefaultControls.Resources GetStandardResources()
        {
            if (s_StandardResources.standard == null)
            {
                s_StandardResources.standard = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
                s_StandardResources.background = AssetDatabase.GetBuiltinExtraResource<Sprite>(kBackgroundSpritePath);
                s_StandardResources.inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>(kInputFieldBackgroundPath);
                s_StandardResources.knob = AssetDatabase.GetBuiltinExtraResource<Sprite>(kKnobPath);
                s_StandardResources.checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>(kCheckmarkPath);
                s_StandardResources.dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>(kDropdownArrowPath);
                s_StandardResources.mask = AssetDatabase.GetBuiltinExtraResource<Sprite>(kMaskPath);
            }

            return s_StandardResources;
        }


        private static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
        {
            // Find the best scene view
            var sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null && SceneView.sceneViews.Count > 0)
                sceneView = SceneView.sceneViews[0] as SceneView;

            // Couldn't find a SceneView. Don't set position.
            if (sceneView == null || sceneView.camera == null)
                return;

            // Create world space Plane from canvas position.
            Vector2 localPlanePosition;
            var camera = sceneView.camera;
            var position = Vector3.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2), camera, out localPlanePosition))
            {
                // Adjust for canvas pivot
                localPlanePosition.x = localPlanePosition.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
                localPlanePosition.y = localPlanePosition.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;

                localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
                localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);

                // Adjust for anchoring
                position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
                position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;

                Vector3 minLocalPosition;
                minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) + itemTransform.sizeDelta.x * itemTransform.pivot.x;
                minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) + itemTransform.sizeDelta.y * itemTransform.pivot.y;

                Vector3 maxLocalPosition;
                maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) - itemTransform.sizeDelta.x * itemTransform.pivot.x;
                maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) - itemTransform.sizeDelta.y * itemTransform.pivot.y;

                position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
                position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
            }

            itemTransform.anchoredPosition = position;
            itemTransform.localRotation = Quaternion.identity;
            itemTransform.localScale = Vector3.one;
        }


        private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
        {
            var parent = menuCommand.context as GameObject;
            if (parent == null || parent.GetComponentInParent<Canvas>() == null) parent = GetOrCreateCanvasGameObject();

            var uniqueName = GameObjectUtility.GetUniqueNameForSibling(parent.transform, element.name);
            element.name = uniqueName;
            Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);
            Undo.SetTransformParent(element.transform, parent.transform, "Parent " + element.name);
            GameObjectUtility.SetParentAndAlign(element, parent);
            if (parent != menuCommand.context) // not a context click, so center in sceneview
                SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(), element.GetComponent<RectTransform>());

            Selection.activeGameObject = element;
        }


        public static GameObject CreateNewUI()
        {
            // Root for the UI
            var root = new GameObject("Canvas");
            root.layer = LayerMask.NameToLayer(kUILayerName);
            var canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            root.AddComponent<CanvasScaler>();
            root.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

            // if there is no event system add one...
            CreateEventSystem(false);
            return root;
        }


        private static void CreateEventSystem(bool select)
        {
            CreateEventSystem(select, null);
        }


        private static void CreateEventSystem(bool select, GameObject parent)
        {
            var esys = FindObjectOfType<EventSystem>();
            if (esys == null)
            {
                var eventSystem = new GameObject("EventSystem");
                GameObjectUtility.SetParentAndAlign(eventSystem, parent);
                esys = eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();

                Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
            }

            if (select && esys != null) Selection.activeGameObject = esys.gameObject;
        }


        public static GameObject GetOrCreateCanvasGameObject()
        {
            var selectedGo = Selection.activeGameObject;

            // Try to find a gameobject that is the selected GO or one if its parents.
            var canvas = selectedGo != null ? selectedGo.GetComponentInParent<Canvas>() : null;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
                return canvas.gameObject;

            // No canvas in selection or its parents? Then use just any canvas..
            canvas = FindObjectOfType(typeof(Canvas)) as Canvas;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
                return canvas.gameObject;

            // No canvas in the scene at all? Then create a new one.
            return CreateNewUI();
        }
    }
}
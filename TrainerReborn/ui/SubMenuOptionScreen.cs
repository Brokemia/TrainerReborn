using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mod.Courier;
using Mod.Courier.UI;
using MonoMod.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TrainerReborn.ui
{
    public class SubMenuOptionScreen : View
    {
        public RectTransform backgroundFrame;

        public Transform optionMenuButtons;

        public Transform backButton;

        protected Vector3 topButtonPos = new Vector3(28.8f, 4.5f, 5.1f);

        public GameObject defaultSelection;

        public GameObject initialSelection;

        public static string onLocID = "OPTIONS_SCREEN_ON";

        public static string offLocID = "OPTIONS_SCREEN_OFF";

        public float heightPerButton = 18f;

        public static string sfxLocID = "OPTIONS_SCREEN_SOUND_FX";

        public static string musicLocID = "OPTIONS_SCREEN_MUSIC";

        public float initialHeight;

        public float startYMax = -270f;

        public Vector3 defaultPos = new Vector3(28.2f, 5.5f, 5.1f);

        public static SubMenuOptionScreen BuildSubMenuOptionScreen(OptionScreen optionScreen)
        {
            GameObject gameObject = new GameObject();
            SubMenuOptionScreen modOptionScreen = gameObject.AddComponent<SubMenuOptionScreen>();
            OptionScreen optionScreen2 = UnityEngine.Object.Instantiate(optionScreen);
            modOptionScreen.name = "SubMenuOptionScreen";

            for (int num = optionScreen2.transform.childCount - 1; num >= 0; num--)
            {
                optionScreen2.transform.GetChild(num).SetParent(modOptionScreen.transform, worldPositionStays: false);
            }

            modOptionScreen.optionMenuButtons = modOptionScreen.transform.Find("Container").Find("BackgroundFrame").Find("OptionsFrame")
                .Find("OptionMenuButtons");
            modOptionScreen.backButton = modOptionScreen.optionMenuButtons.Find("Back");
            Transform[] children = modOptionScreen.optionMenuButtons.GetChildren();
            foreach (Transform transform in children)
            {
                if (!transform.Equals(modOptionScreen.backButton))
                {
                    UnityEngine.Object.Destroy(transform.gameObject);
                }
            }

            modOptionScreen.backButton.SetParent(modOptionScreen.optionMenuButtons);
            Button componentInChildren = modOptionScreen.backButton.GetComponentInChildren<Button>();
            componentInChildren.onClick = new Button.ButtonClickedEvent();
            componentInChildren.onClick.AddListener(modOptionScreen.BackToOptionMenu);
            modOptionScreen.InitStuffUnityWouldDo();
            modOptionScreen.gameObject.SetActive(value: false);
            Courier.UI.ModOptionScreenLoaded = true;
            return modOptionScreen;
        }
        private void Awake()
        {
        }

        private void InitStuffUnityWouldDo()
        {
            backgroundFrame = (RectTransform)base.transform.Find("Container").Find("BackgroundFrame");
            initialHeight = backgroundFrame.sizeDelta.y;
            base.gameObject.AddComponent<Canvas>();
        }
        private void Start()
        {
            InitOptions();
        }
        public override void Init(IViewParams screenParams)
        {
            base.Init(screenParams);
            Courier.UI.InitOptionsViewWithModButtons(this, Courier.UI.ModOptionButtons);
            Sprite sprite2 = (backgroundFrame.GetComponent<Image>().sprite = Courier.EmbeddedSprites["Mod.Courier.UI.mod_options_frame"]);
            Sprite sprite3 = sprite2;
            sprite3.bounds.extents.Set(1.7f, 1.7f, 0.1f);
            sprite3.texture.filterMode = FilterMode.Point;
            sprite2 = (backgroundFrame.Find("OptionsFrame").GetComponent<Image>().sprite = Courier.EmbeddedSprites["Mod.Courier.UI.mod_options_frame"]);
            sprite3 = sprite2;
            sprite3.bounds.extents.Set(1.7f, 1.7f, 0.1f);
            sprite3.texture.filterMode = FilterMode.Point;
            HideUnavailableOptions();
            InitOptions();
            SetInitialSelection();
            foreach (Image item in from c in base.transform.GetComponentsInChildren<Image>()
                                   where c.name.Equals("SelectionFrame")
                                   select c)
            {
                try
                {
                    if (!(item.overrideSprite != null) || !(item.overrideSprite.name != "Empty"))
                    {
                        continue;
                    }

                    RenderTexture renderTexture2 = (RenderTexture.active = new RenderTexture(item.overrideSprite.texture.width, item.overrideSprite.texture.height, 0));
                    Graphics.Blit(item.overrideSprite.texture, renderTexture2);
                    Texture2D texture2D = new Texture2D(renderTexture2.width, renderTexture2.height, TextureFormat.RGBA32, mipmap: true);
                    texture2D.ReadPixels(new Rect(0f, 0f, renderTexture2.width, renderTexture2.height), 0, 0, recalculateMipMaps: false);
                    Color[] pixels = texture2D.GetPixels();
                    for (int i = 0; i < pixels.Length; i++)
                    {
                        if (Math.Abs((double)pixels[i].r - 0.973) < 0.01 && Math.Abs((double)pixels[i].g - 0.722) < 0.01)
                        {
                            pixels[i].r = 0f;
                            pixels[i].g = 0.633f;
                            pixels[i].b = 1f;
                        }
                    }

                    texture2D.SetPixels(pixels);
                    texture2D.Apply();
                    sprite2 = (item.overrideSprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(16f, 16f), 20f, 1u, SpriteMeshType.FullRect, new Vector4(5f, 5f, 5f, 5f)));
                    Sprite sprite6 = sprite2;
                    sprite6.bounds.extents.Set(0.8f, 0.8f, 0.1f);
                    sprite6.texture.filterMode = FilterMode.Point;
                }
                catch
                {
                  
                  
                }
            }
        }

        private IEnumerator WaitAndSelectInitialButton()
        {
            yield return null;
            SetInitialSelection();
        }

        private void OnEnable()
        {
            if (base.transform.parent != null)
            {
                Manager<UIManager>.Instance.SetParentAndAlign(base.gameObject, base.transform.parent.gameObject);
            }

            EventSystem.current.SetSelectedGameObject(null);
        }

        private void OnDisable()
        {
            base.transform.position = defaultPos;
        }

        private void HideUnavailableOptions()
        {
            foreach (OptionsButtonInfo modOptionButton in Courier.UI.ModOptionButtons)
            {
                modOptionButton.gameObject.SetActive(modOptionButton.IsEnabled?.Invoke() ?? true);
            }

            StartCoroutine(WaitAndSelectInitialButton());
            Vector2 sizeDelta = backgroundFrame.sizeDelta;
            backgroundFrame.sizeDelta = new Vector2(sizeDelta.x, 110f + heightPerButton * (float)Courier.UI.EnabledModOptionsCount());
        }

        private void SetInitialSelection()
        {
            GameObject gameObject = (initialSelection ?? defaultSelection).transform.Find("Button").gameObject;
            gameObject.transform.GetComponent<UIObjectAudioHandler>().playAudio = false;
            EventSystem.current.SetSelectedGameObject(gameObject);
            gameObject.GetComponent<Button>().OnSelect(null);
            gameObject.GetComponent<UIObjectAudioHandler>().playAudio = true;
            initialSelection = null;
        }

        public void GoOffscreenInstant()
        {
            base.gameObject.SetActive(value: false);
            // Courier.UI.ModOptionScreenLoaded = false;
        }

        public int GetSelectedButtonIndex()
        {
            if (backButton.Find("Button").gameObject.Equals(EventSystem.current.currentSelectedGameObject))
            {
                //return Courier.UI.EnabledModOptionsCount();
            }

            //foreach (OptionsButtonInfo modOptionButton in Courier.UI.ModOptionButtons)
            //{
            //    if (modOptionButton.gameObject.transform.Find("Button").gameObject.Equals(EventSystem.current.currentSelectedGameObject))
            //    {
            //        return Courier.UI.EnabledModOptionsBeforeButton(modOptionButton);
            //    }
            //}

            return -1;
        }

        private void LateUpdate()
        {
            if (Manager<InputManager>.Instance.GetBackDown())
            {
                BackToOptionMenu();
            }

            Vector3 vector = new Vector3(0f, (float)Math.Min(GetSelectedButtonIndex(), Math.Max(0, Courier.UI.EnabledModOptionsCount() - 10)) * 0.9f) - new Vector3(0f, (float)Math.Max(0, Courier.UI.EnabledModOptionsCount() - 11) * 0.45f);
            base.transform.position = defaultPos + vector;
            //foreach (OptionsButtonInfo modOptionButton in Courier.UI.ModOptionButtons)
            //{
            //    modOptionButton.UpdateNameText();
            //}

            foreach (Image item in from c in base.transform.GetComponentsInChildren<Image>()
                                   where c.name.Equals("SelectionFrame")
                                   select c)
            {
                try
                {
                    if (!(item.overrideSprite != null) || !item.overrideSprite.name.Equals("ShopItemSelectionFrame"))
                    {
                        continue;
                    }

                    RenderTexture renderTexture2 = (RenderTexture.active = new RenderTexture(item.overrideSprite.texture.width, item.overrideSprite.texture.height, 0));
                    Graphics.Blit(item.overrideSprite.texture, renderTexture2);
                    Texture2D texture2D = new Texture2D(renderTexture2.width, renderTexture2.height, TextureFormat.RGBA32, mipmap: true);
                    texture2D.ReadPixels(new Rect(0f, 0f, renderTexture2.width, renderTexture2.height), 0, 0, recalculateMipMaps: false);
                    Color[] pixels = texture2D.GetPixels();
                    for (int i = 0; i < pixels.Length; i++)
                    {
                        if (Math.Abs((double)pixels[i].r - 0.973) < 0.01 && Math.Abs((double)pixels[i].g - 0.722) < 0.01)
                        {
                            pixels[i].r = 0f;
                            pixels[i].g = 0.633f;
                            pixels[i].b = 1f;
                        }
                    }

                    texture2D.SetPixels(pixels);
                    texture2D.Apply();
                    Sprite sprite2 = (item.overrideSprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(16f, 16f), 20f, 1u, SpriteMeshType.FullRect, new Vector4(5f, 5f, 5f, 5f)));
                    Sprite sprite3 = sprite2;
                    sprite3.bounds.extents.Set(0.8f, 0.8f, 0.1f);
                    sprite3.texture.filterMode = FilterMode.Point;
                }
                catch
                {
                  
                }
            }
        }

        private void InitOptions()
        {
            defaultSelection = backButton.gameObject;
            //foreach (OptionsButtonInfo modOptionButton in Courier.UI.ModOptionButtons)
            //{
            //    if (modOptionButton.IsEnabled?.Invoke() ?? true)
            //    {
            //        defaultSelection = modOptionButton.gameObject;
            //        break;
            //    }
            //}

            backgroundFrame.Find("Title").GetComponent<TextMeshProUGUI>().SetText("Test");
            //foreach (OptionsButtonInfo modOptionButton2 in Courier.UI.ModOptionButtons)
            //{
            //    modOptionButton2.UpdateStateText();
            //}
        }

        public void BackToOptionMenu()
        {
            Close(transitionOut: false);
            Manager<UIManager>.Instance.GetView<OptionScreen>().gameObject.SetActive(value: true);
            Courier.UI.ModOptionButton.gameObject.transform.Find("Button").GetComponent<UIObjectAudioHandler>().playAudio = false;
            EventSystem.current.SetSelectedGameObject(Courier.UI.ModOptionButton.gameObject.transform.Find("Button").gameObject);
            Courier.UI.ModOptionButton.gameObject.transform.Find("Button").GetComponent<UIObjectAudioHandler>().playAudio = true;
        }

        public override void Close(bool transitionOut)
        {
            base.Close(transitionOut);
            Courier.UI.ModOptionScreenLoaded = false;
        }
    }
}

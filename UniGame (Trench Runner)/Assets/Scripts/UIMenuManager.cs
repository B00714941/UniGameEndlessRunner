using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

// This was a free asset I acquired from the unity store and modified to my own liking

namespace SlimUI.ModernMenu{
	public class UIMenuManager : MonoBehaviour {

        [SerializeField]
        private Animator animator;
        private Animator CameraObject;
		public GameObject TITLE;
        [SerializeField]
        public AudioClip IntroFx;
        public AudioSource MainMenuTheme;

        // campaign button sub menu
        [Header("MENUS")]
        [Tooltip("The Menu for when the MAIN menu buttons")]
        public GameObject mainMenu;
        [Tooltip("THe first list of buttons")]
        public GameObject firstMenu;
        [Tooltip("The Menu for when the PLAY button is clicked")]
        public GameObject playMenu;
        [Tooltip("The Menu for when the EXIT button is clicked")]
        public GameObject exitMenu;

        public enum Theme {custom1, custom2, custom3};
        [Header("THEME SETTINGS")]
        public Theme theme;
        private int themeIndex;
        public ThemedUIData themeController;


		void Start()
		{

            CameraObject = transform.GetComponent<Animator>();

			playMenu.SetActive(false);
			exitMenu.SetActive(false);
			firstMenu.SetActive(true);
			mainMenu.SetActive(true);

			SetThemeColors();
            MainMenuTheme = GetComponent<AudioSource>();
            MainMenuTheme.clip = IntroFx;
            MainMenuTheme.Play();
        }

		//sets the custom green theme
		void SetThemeColors()
		{
			switch (theme)
			{
				case Theme.custom1:
					themeController.currentColor = themeController.custom1.graphic1;
					themeController.textColor = themeController.custom1.text1;
					themeIndex = 0;
					break;
				case Theme.custom2:
					themeController.currentColor = themeController.custom2.graphic2;
					themeController.textColor = themeController.custom2.text2;
					themeIndex = 1;
					break;
				case Theme.custom3:
					themeController.currentColor = themeController.custom3.graphic3;
					themeController.textColor = themeController.custom3.text3;
					themeIndex = 2;
					break;
				default:
					Debug.Log("Invalid theme selected.");
					break;
			}
		}

		//Hit play button and load new scene while disabling other scenes
		public void PlayCampaign(){
			exitMenu.SetActive(false);
			TITLE.SetActive(false);
			mainMenu.SetActive(false);
            MainMenuTheme.Stop();
            SceneManager.LoadScene("IntroScreen");
        }

		//If Pop Up panel added would allow you to return to a previous menu
		public void ReturnMenu(){
			playMenu.SetActive(false);
			exitMenu.SetActive(false);
			mainMenu.SetActive(true);
		}

		public void  DisablePlayCampaign(){
			playMenu.SetActive(false);
		}

		// Are You Sure - Quit Panel Pop Up could be readded later
		public void AreYouSure(){
			exitMenu.SetActive(true);
			DisablePlayCampaign();
		}

		//Quit the game as soon as you are in the main menu
		public void QuitGame(){
			#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
			#else
				Application.Quit();
			#endif
		}

		}
	}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using UnityPlayer;
// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace Template
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	sealed partial class App : Application
	{
		private WinRTBridge.WinRTBridge _bridge;
		private AppCallbacks appCallbacks;
		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			this.InitializeComponent();
			appCallbacks = new AppCallbacks();
			appCallbacks.RenderingStarted += RemoveSplashScreen;
		}

		/// <summary>
		/// Invoked when application is launched through protocol.
		/// Read more - http://msdn.microsoft.com/library/windows/apps/br224742
		/// </summary>
		/// <param name="args"></param>
		protected override void OnActivated(IActivatedEventArgs args)
		{
			string appArgs = "";
			Windows.ApplicationModel.Activation.SplashScreen splashScreen = null;
			switch (args.Kind)
			{
				case ActivationKind.Protocol:
					ProtocolActivatedEventArgs eventArgs = args as ProtocolActivatedEventArgs;
					splashScreen = eventArgs.SplashScreen;
					appArgs += string.Format("Uri={0}", eventArgs.Uri.AbsoluteUri);
					break;
			}
			InitializeUnity(appArgs, splashScreen);
		}

		/// <summary>
		/// Invoked when application is launched via file
		/// Read more - http://msdn.microsoft.com/library/windows/apps/br224742
		/// </summary>
		/// <param name="args"></param>
		protected override void OnFileActivated(FileActivatedEventArgs args)
		{
			string appArgs = "";
			Windows.ApplicationModel.Activation.SplashScreen splashScreen = null;

			splashScreen = args.SplashScreen;
			appArgs += "File=";
			bool firstFileAdded = false;
			foreach (var file in args.Files)
			{
				if (firstFileAdded) appArgs += ";";
				appArgs += file.Path;
				firstFileAdded = true;
			}

			InitializeUnity(appArgs, splashScreen);
		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used when the application is launched to open a specific file, to display
		/// search results, and so forth.
		/// </summary>
		/// <param name="args">Details about the launch request and process.</param>
		protected override void OnLaunched(LaunchActivatedEventArgs args)
		{
			InitializeUnity(args.Arguments, args.SplashScreen);
		}

		private void InitializeUnity(string args, Windows.ApplicationModel.Activation.SplashScreen splashScreen)
		{
#if UNITY_WP_8_1
			ApplicationView.GetForCurrentView().SuppressSystemOverlays = true;
#pragma warning disable 4014
			StatusBar.GetForCurrentView().HideAsync();
#pragma warning restore 4014
#endif

			appCallbacks.SetAppArguments(args);
			Frame rootFrame = Window.Current.Content as Frame;

			// Do not repeat app initialization when the Window already has content,
			// just ensure that the window is active
			if (rootFrame == null && !appCallbacks.IsInitialized())
			{
				var mainPage = new MainPage(splashScreen);
				Window.Current.Content = mainPage;
				Window.Current.Activate();

				// Setup scripting bridge
				_bridge = new WinRTBridge.WinRTBridge();
				appCallbacks.SetBridge(_bridge);

#if !UNITY_WP_8_1
				appCallbacks.SetKeyboardTriggerControl(mainPage);
#endif

				appCallbacks.SetSwapChainPanel(mainPage.GetSwapChainPanel());
				appCallbacks.SetCoreWindowEvents(Window.Current.CoreWindow);
				appCallbacks.InitializeD3DXAML();
			}

			Window.Current.Activate();
			
#if UNITY_WP_8_1
			SetupLocationService();
#endif
		}

		private void RemoveSplashScreen()
		{
			// This will fail if you change main window class
			// Make sure to adjust accordingly if you do something like this
			MainPage page = (MainPage)Window.Current.Content;
			page.RemoveSplashScreen();
		}
		
#if UNITY_WP_8_1
		// This is the default setup to show location consent message box to the user
		// You can customize it to your needs, but do not remove it completely if your application
		// uses location services, as it is a requirement in Windows Store certification process
		private async void SetupLocationService()
		{
			if (!appCallbacks.IsLocationCapabilitySet())
			{
				return;
			}

			const string settingName = "LocationContent";
			bool userGaveConsent = false;

			object consent;
			var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
			var userWasAskedBefore = settings.Values.TryGetValue(settingName, out consent);

			if (!userWasAskedBefore)
			{
				var messageDialog = new Windows.UI.Popups.MessageDialog("Can this application use your location?", "Location services");

				var acceptCommand = new Windows.UI.Popups.UICommand("Yes");
				var declineCommand = new Windows.UI.Popups.UICommand("No");

				messageDialog.Commands.Add(acceptCommand);
				messageDialog.Commands.Add(declineCommand);

				userGaveConsent = (await messageDialog.ShowAsync()) == acceptCommand;
				settings.Values.Add(settingName, userGaveConsent);
			}
			else
			{
				userGaveConsent = (bool)consent;
			}

			if (userGaveConsent)
			{	// Must be called from UI thread
				appCallbacks.SetupGeolocator();
			}
		}
#endif
	}
}

using System;
using Audiomob.Internal;
using UnityEngine;
using Audiomob.Internal.DevicePlugins;
using Audiomob.Internal.Utility;

namespace Audiomob.Unmanaged
{
	/// <summary>
	/// Initializes the Audiomob Unity Plugin.
	/// </summary>
	public class AudiomobPluginInitialization
	{
		/// <summary>
		/// The mediation adaptor SDK to use in this project.
		/// </summary>
		public static IMediationAdapterSdk MediationAdapterSdk { private get; set; }

		private static bool ManualInitializationEnabled = false;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void RegisterInitializationHook()
		{
			AudiomobPlugin.InitializationHook += InitializePlugin;
		}
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitializePluginAutomatically()
		{
			try
			{
				if (AudiomobSettings.Instance.AutoInitializePlugin)
				{
					DoInitialization();
				}
			}
			catch (Exception exception)
			{
				AMDebug.LogError($"Failed to auto-initialize Audiomob: {exception.Message}");
			}

			ManualInitializationEnabled = true;
		}

		/// <summary>
		/// Initialize the Audiomob Plugin.
		/// This should be done early in the game's runtime lifecycle, but not before the first scene has loaded.
		/// This initialization can be set to happen automatically by checking "Automatically Initialize Plugin" in the Audiomob Settings Window.
		/// Optional callback invoked with 'true' on initialization success and 'false' if the initialization is un-successful.
		/// </summary>
		public static void InitializePlugin(Action<bool> callback = null)
		{
			if (!ManualInitializationEnabled)
			{
				AMDebug.LogError("Initialization of Audiomob Failed: Plugin initialization must not be called before the first scene is loaded, to initialize the plugin on app launch, enable 'Automatically Initialize Plugin' in the Audiomob Settings Window.");
				return;
			}
			
			DoInitialization(callback);
		}
		
		private static void DoInitialization(Action<bool> callback = null)
		{
			IExceptionHandler exceptionHandler = null;
			try
			{
				if (AudiomobPluginController.Initialized)
				{
					InvokeInitCallback(callback, true);
					return;
				}

				if (!AudiomobPluginController.IsInitializationInProgress)
				{
					exceptionHandler = new ExceptionHandler(new WebRequestFactory(null), new ApplicationUtility());

#if UNITY_EDITOR
					IOsPlugin osPlugin = new EditorOsPlugin();
					IOpenMeasurementSdk openMeasurementSdk = null;
#elif UNITY_IOS
					IOsPlugin osPlugin = new IosOsPlugin();
					IOpenMeasurementSdk openMeasurementSdk = new IosOpenMeasurementSdk();
#elif UNITY_ANDROID
					IOsPlugin osPlugin = new AndroidOsPlugin();
					IOpenMeasurementSdk openMeasurementSdk = new AndroidOpenMeasurementSdk();
#else
					IOsPlugin osPlugin = new StandaloneOsPlugin();
					IOpenMeasurementSdk openMeasurementSdk = null;
#endif
					
					ILocationService locationService = new AudiomobLocationService();
					locationService.Start();

					osPlugin.Start();
					osPlugin.SetCallbacks();
				
					AudiomobPluginController.Init(osPlugin, AudiomobSettings.Instance, locationService, openMeasurementSdk, MediationAdapterSdk, exceptionHandler, callback);
				}
				else
				{
					AudiomobPluginController.AppendOnInitCallback(callback);
				}
			}
			catch (Exception exception)
			{
				exceptionHandler?.RecordException(new NativePluginInitialisationFailedException($"Failed to initialize Audiomob: {exception.Message}"));
				InvokeInitCallback(callback, false);
			}
		}

		private static void InvokeInitCallback(Action<bool> callback, bool initSuccess)
		{
			try
			{
				callback?.Invoke(initSuccess);
			}
			catch (Exception exception)
			{
				AMDebug.LogError($"Error caught in initialization callback invoke: {exception.Message}");
			}
		}
	}
}

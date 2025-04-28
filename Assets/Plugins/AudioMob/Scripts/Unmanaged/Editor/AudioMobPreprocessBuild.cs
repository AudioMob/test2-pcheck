using System;
using Audiomob.Internal;
using Audiomob.Internal.Editor;
using Audiomob.Internal.Utility;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Audiomob.Unmanaged.Editor
{
	/// <summary>
	/// Audiomob Preprocess scripts for builds. Called before every build.
	/// </summary>
	public class AudiomobPreprocessBuild : IPreprocessBuildWithReport
	{
		// Required for IPreprocessBuildWithReport interface implementation, left as default.
		public int callbackOrder => 0;

		// We need to get PlayerSettings.muteOtherAudioSources at runtime, however there is no way to do so unless we save the value ourselves.
		// Before every build, ensure that the Audiomob settings stores the PlayerSetting's value for this field.
		public void OnPreprocessBuild(BuildReport report)
		{
			SavePlayerSettings();
			CompileTimeChecks compileTimeChecks = new CompileTimeChecks();
			compileTimeChecks.CheckIfAudioIsDisabled();
			
#if UNITY_ANDROID
			compileTimeChecks.CheckIfMuteOtherAudioSourcesIsEnabled();
			compileTimeChecks.CheckIfNativeLibrariesArePresentAndProperlyConfigured();
#endif
			
			compileTimeChecks.CheckIfTestServerIsSelectedForProductionBuild();
		}
		
		private static void SavePlayerSettings()
		{
			try
			{
				IAudiomobSettings settings = AudiomobSettings.Instance;
				
				bool muteOtherAudioSources = PlayerSettings.muteOtherAudioSources;
				if (settings.MuteOtherAudioSources != muteOtherAudioSources)
				{
					try
					{
						EditorUtility.SetDirty(settings as AudiomobSettings);
					}
					catch (Exception)
					{
						AMDebug.Log($"{AMDebug.Prefix} Failed to set Audiomob Settings asset as dirty, type is not AudiomobSettings");
					}
					
					settings.MuteOtherAudioSources = PlayerSettings.muteOtherAudioSources;
				}

				AssetDatabase.SaveAssets();
			}
			catch (Exception e)
			{
				AMDebug.Log($"Unable to update muteOtherAudioSources Audiomob Settings: {e.Message}");
			}
		}
	}
}

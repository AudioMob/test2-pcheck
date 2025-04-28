using Audiomob;
using UnityEngine;
using UnityEngine.UI;

namespace AudiomobExamples
{
	/// <summary>
	/// This class demonstrates Audiomob's key features via a basic implementation.
	/// Read the full API here: https://developer.audiomob.io/dashboard/plugin-api/latest
	/// </summary>
	[AddComponentMenu("Audiomob/Examples/Ad Manager")]
	public class AudiomobExampleAdManager : MonoBehaviour
	{
		[SerializeField]
    	[Tooltip("Used to display information about the state of Audiomob.")]
    	public Text infoText;

	    [SerializeField]
	    [Tooltip("Used to display information about the version of Audiomob Plugin.")]
	    private Text pluginVersionText; 
        
        private void Awake()
        {
	        Debug.Assert(infoText, $"[{nameof(AudiomobExampleAdManager)}] {nameof(infoText)} is null.");
	        Debug.Assert(pluginVersionText, $"[{nameof(AudiomobExampleAdManager)}] {nameof(pluginVersionText)} is null.");
        }

        private void Start()
		{
			// Assign callbacks to the Audiomob events.
			if (AudiomobPlugin.Instance != null)
			{
				AudiomobPlugin.Instance.OnAdPlaybackStatusChanged += OnAdPlaybackStatusChanged;
				AudiomobPlugin.Instance.OnAdFailed += OnAdFailed;
			}

			pluginVersionText.text = AudiomobPlugin.PluginVersion;
		}
        
        private void OnDestroy()
        {
	        if (AudiomobPlugin.Instance != null)
	        {
		        AudiomobPlugin.Instance.OnAdPlaybackStatusChanged -= OnAdPlaybackStatusChanged;
		        AudiomobPlugin.Instance.OnAdFailed -= OnAdFailed;
	        }
        }

        /// <summary>
        /// Request and play an audio ad using Audiomob.
        /// </summary>
		public void RequestAndPlayAudiomobAd()
		{
			infoText.text = "Ad Requested";
			if (AudiomobPlugin.Instance != null)
			{
				AudiomobPlugin.Instance.PlayAd(AudiomobPlugin.AdUnits.RewardedRectangle);
			}
		}

		private void OnAdPlaybackStatusChanged(AdSequence adSequence, AdPlaybackStatus adPlaybackStatus)
		{
			switch (adPlaybackStatus)
			{
				case AdPlaybackStatus.Started:
					/* Write code here to:
				 - Turn down your game volume.
				 - Turn off your game music.
				 - Give your players an instant reward? */
			
					infoText.text = "Ad Started Playing";
					break;
				case AdPlaybackStatus.Finished:
					/* Write code here to:
				   - Give your player a reward for listening to the ad? */

					infoText.text = "Ad Finished Playing";
					break;
				case AdPlaybackStatus.Canceled:
					infoText.text = "Ad Canceled";
					break;
				case AdPlaybackStatus.Skipped:
					infoText.text = "Ad Skipped";
					break;
				case AdPlaybackStatus.Stopped:
					infoText.text = "Ad Stopped";
					break;
			}
		}

		private void OnAdFailed(string adUnitId, AdFailureReason adFailureReason)
		{
			switch (adFailureReason)
			{
				case AdFailureReason.RequestFailed:
					infoText.text = "Ad Request Failed";
					break;
				case AdFailureReason.PlaybackFailed:
					infoText.text = "Ad Failed";
					break;
			}
		}
	}
}

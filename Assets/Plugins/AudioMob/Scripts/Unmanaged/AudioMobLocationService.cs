using Audiomob.Internal;
using UnityEngine;

namespace Audiomob.Unmanaged
{
	/// <summary>
	/// Location services for Audiomob.
	/// If location services are disabled, the location code won't be compiled to device.
	/// </summary>
	public class AudiomobLocationService : ILocationService
	{
		/// <summary>
		/// Specifies whether location service is enabled.
		/// </summary>
		public bool IsEnabledByUser
		{
			get
			{
#if AUDIOMOB_USE_LOCATION_SERVICES
                return Input.location.isEnabledByUser;
#else
				return false;
#endif
			}
		}

		/// <summary>
		/// Returns location service status.
		/// </summary>
		public GeoStatus Status
		{
			get
			{
#if AUDIOMOB_USE_LOCATION_SERVICES
                switch (Input.location.status)
                {
                    case LocationServiceStatus.Failed:
                        return GeoStatus.Failed;
                    case LocationServiceStatus.Initializing:
                        return GeoStatus.Initializing;
                    case LocationServiceStatus.Stopped:
                        return GeoStatus.Stopped;
                    case LocationServiceStatus.Running:
                        return GeoStatus.Running;
                    default:
                        return GeoStatus.Failed;
                }
#else
				return GeoStatus.Stopped;
#endif
			}
		}

		/// <summary>
		/// Starts location service updates.
		/// </summary>
		public void Start()
		{
#if AUDIOMOB_USE_LOCATION_SERVICES
            Input.location.Start();
#endif
		}

		/// <summary>
		/// Last measured device geographical location.
		/// </summary>
		public LocationInfo LastData
		{
			get
			{
#if AUDIOMOB_USE_LOCATION_SERVICES
                return Input.location.lastData;
#else
				return new LocationInfo();
#endif
			}
		}
	}
}

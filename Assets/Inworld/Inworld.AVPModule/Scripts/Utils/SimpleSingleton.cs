using UnityEngine;

//from https://github.com/lachlansleight/Lunity
namespace Lunity
{
	public class SimpleSingleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T _instance;
		public static T Instance
		{
			get
			{
				if (_instance == null) _instance = FindObjectOfType<T>();
				return _instance;
			}
		}

		protected virtual void OnDestroy()
		{
			_instance = null;
		}
	}
}
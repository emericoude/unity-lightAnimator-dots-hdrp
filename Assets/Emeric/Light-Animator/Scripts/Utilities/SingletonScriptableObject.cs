using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Emeric.LightAnimator
{
	public class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
	{
		private static T _instance = null;

		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					T[] results = Resources.FindObjectsOfTypeAll<T>();

					if (results.Length <= 0)
					{
						Debug.LogError("SingletonScriptableObject: There is no instance of type <" + typeof(T).ToString() + ">. Make sure it is inside a 'Resources' folder.");
						return null;
					}

					if (results.Length > 1)
					{
						Debug.LogError("SingletonScriptableObject: There is more than one instance of type<" + typeof(T).ToString() + ">. Make sure there is only one instance");
						return null;
					}

					_instance = results[0];
					_instance.hideFlags = HideFlags.DontUnloadUnusedAsset;
				}
				return _instance;
			}
		}
	}
}

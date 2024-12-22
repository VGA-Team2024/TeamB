using System;
using UnityEngine;

namespace TeamB.Develop
{
	public static class DebugManager
	{
		public static void Log(object log)
		{
#if UNITY_EDITOR
			Debug.Log(log);
#endif
		}

		public static void LogError(object log)
		{
#if UNITY_EDITOR
			Debug.LogError(log);
#endif
		}

		public static void LogException(Exception e)
		{
#if UNITY_EDITOR
			Debug.LogException(e);
#endif
		}
	}
}

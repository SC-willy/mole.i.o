using System;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
namespace Plugins.Baedrick.ColoredHeaderCreator
{
	public class ColoredHeader : MonoBehaviour
	{

		public HeaderSettings headerSettings = new HeaderSettings();

		private void OnValidate()
		{
			EditorApplication.delayCall += _OnValidate;
		}
		private void _OnValidate()
		{
			if (this == null)
			{
				return;
			}

			EditorApplication.RepaintHierarchyWindow();
		}

	}
}
#endif
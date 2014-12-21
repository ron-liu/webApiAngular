using System;
using System.ComponentModel;

namespace LeaveManager.Api.Infrastructure
{
	public static class Extentions
	{
		public static string GetDescription(this Enum value)
		{
			var field = value.GetType().GetField(value.ToString());
			if (field == null) return string.Empty;

			var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

			return attribute == null ? value.ToString() : attribute.Description;
		}
	}
}
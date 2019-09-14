using System.Linq;
using System.Reflection;

namespace Athylps.Core.Types
{
	public static class Roles
	{
		public static string Admin => "Admin";
		public static string User => "User";

		private static readonly string[] Array;

		static Roles()
		{
			Array = typeof(Roles).GetProperties(BindingFlags.Static | BindingFlags.Public)
				.Select(p => p.GetValue(null) as string)
				.Where(v => !string.IsNullOrWhiteSpace(v))
				.ToArray();
		}

		public static string[] ToArray()
		{
			return Array;
		}
	}
}

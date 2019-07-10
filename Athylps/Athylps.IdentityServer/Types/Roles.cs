using System.Linq;
using System.Reflection;

namespace Athylps.IdentityServer.Types
{
	public static class Roles
	{
		public static string Admin => "Admin";
		public static string User => "User";

		private static readonly string[] _array;

		static Roles()
		{
			_array = typeof(Roles).GetProperties(BindingFlags.Static | BindingFlags.Public)
				.Select(p => p.GetValue(null) as string)
				.Where(v => !string.IsNullOrWhiteSpace(v))
				.ToArray();
		}

		public static string[] ToArray()
		{
			return _array;
		}
	}
}

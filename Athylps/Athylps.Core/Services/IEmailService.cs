using System.Threading.Tasks;

namespace Athylps.Core.Services
{
	public interface IEmailService
	{
		Task SendConfirmationUrlAsync(string email, string url);
	}
}

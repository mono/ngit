using NSch;
using Sharpen;

namespace NSch
{
	public interface UserInfo
	{
		string GetPassphrase();

		string GetPassword();

		bool PromptPassword(string message);

		bool PromptPassphrase(string message);

		bool PromptYesNo(string message);

		void ShowMessage(string message);
	}
}

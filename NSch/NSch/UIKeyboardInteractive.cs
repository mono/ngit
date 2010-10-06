using NSch;
using Sharpen;

namespace NSch
{
	public interface UIKeyboardInteractive
	{
		string[] PromptKeyboardInteractive(string destination, string name, string instruction
			, string[] prompt, bool[] echo);
	}
}

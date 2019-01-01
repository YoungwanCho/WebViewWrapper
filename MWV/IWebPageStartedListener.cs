using System;

namespace MWV
{
	public interface IWebPageStartedListener
	{
		void OnWebPageStarted(string url);
	}
}
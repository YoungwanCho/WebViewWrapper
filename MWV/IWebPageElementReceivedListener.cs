using System;

namespace MWV
{
	public interface IWebPageElementReceivedListener
	{
		void OnWebPageElementReceived(string tag, string value, bool isInput);
	}
}
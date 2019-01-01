using System;

namespace MWV
{
	public interface IWebPageErrorListener
	{
		void OnWebPageError(PageErrorCode errorCode);
	}
}
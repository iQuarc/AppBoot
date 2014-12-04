using System;

namespace iQuarc.AppBoot.WebApi
{
	public interface IExceptionLogger
	{
		void Log(Exception exception);
	}
}
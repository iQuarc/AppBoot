using System;
using System.Diagnostics;

namespace iQuarc.AppBoot.WebApi
{
	internal class DebugExceptionLogger : IExceptionLogger
	{
		public void Log(Exception exception)
		{
			Debug.WriteLine(exception);
		}
	}
}
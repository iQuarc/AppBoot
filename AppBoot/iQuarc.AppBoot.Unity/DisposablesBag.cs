using System;
using System.Collections.Generic;

namespace iQuarc.AppBoot.Unity
{
	internal class DisposablesBag : IDisposable
	{
		private List<WeakReference> bag = new List<WeakReference>();
		private readonly object lockObj = new object();

		public void Add(IDisposable item)
		{
			lock (lockObj)
			{
				bag.Add(new WeakReference(item));
			}
		}

		public void Dispose()
		{
			lock (lockObj)
			{
				foreach (var reference in bag)
				{
					object item = reference.Target;
					IDisposable disposable = item as IDisposable;
					if (disposable != null)
						disposable.Dispose();
				}
			}
			bag = new List<WeakReference>();
		}
	}
}
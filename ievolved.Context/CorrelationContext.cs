
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace ievolved.Context
{
	[Serializable]
	public class CorrelationContext : IDisposable
	{
		public IContextKey Key { get; set; }

		public void Dispose() { }
	}
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ievolved.Context
{
	public interface IContextStore
	{
		string Type { get; }
		bool HasContext { get; }

		T Get<T>(string key);
		void Set<T>(string key, T value);
		void Free(string key);
	}
}

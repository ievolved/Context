
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ievolved.Context
{
	public interface IContextKey
	{
		string ID { get; set; }
		string Create(string sourceType = "");
	}
}

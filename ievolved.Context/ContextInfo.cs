
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ievolved.Context
{
	public class ContextInfo
	{
		public string Key { get; set; }
		public string Type { get; set; }
		public string Annotation { get; set; }
		public Dictionary<string, string> Headers { get; set; }
		public bool IsCloned { get; set; }
	}
}

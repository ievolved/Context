
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;


namespace ievolved.Context
{
	[DebuggerDisplay("{ID}")]
	[DebuggerTypeProxy(typeof(string))]
	[Serializable]
	public class UniqueContextKey : IContextKey
	{
		public string ID { get; set; }

		private static int counter;						// Cache the counter, it will be incremented appDomain-globally
		private static string uniquePart;               // Cache the unique part of the ID to avoid future re-computation


		public UniqueContextKey(string sourceType = "") {
			Create(sourceType);
		}

		public string Create(string sourceType = "") {
			Contract.Requires(sourceType != null);

			// We must guarantee that a context key already generated will not change.
			//
			if (ID != null) {
				return ID;
			}

			string result = "";

			// Format should be:
			//
			//  {SourceType}:{Year}{Month}{Day}{Counter}{Hour}{Minute}{Second}{ProcessID}>{Unique IPv4 Part HEX}
			//
			// Based on a BASE60 alphabet
			//
			var alphas = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz01234567";

			// Don't want exceptions if we overflow, just rollover again, the new ID will still be unique
			//
			int count = 0;
			unchecked {
				count = Interlocked.Increment(ref counter);
			}

			// Here we'll use the (deprecated) IPv4 IP address as the unique part, or fall back to the 
			//  GUID.  Whatever the case, we must guarantee uniqueness across all servers in the farm.
			//
			if (uniquePart == null) {
				var host = Dns.GetHostEntry(Dns.GetHostName());
				foreach (var address in host.AddressList) {
					if (address.AddressFamily == AddressFamily.InterNetwork) {
						uniquePart = address.Address.ToString("X");
						break;
					}
				}
			}

			// Get the unique MAC part of a GUID
			//
			if (uniquePart == null) {
				byte[] guid = Guid.NewGuid().ToByteArray();
				byte[] mac = new byte[6];

				for (int index = 10; index < 16; index++) {
					mac[index - 10] = guid[index];
				}
				
				uniquePart = Convert.ToBase64String(mac);
			}

			var now = DateTime.Now;

			// Formatted so data/time parts are base60, and any numerica parts are in-between
			//  to avoid using delimiters.  The final unique part is delimited by a right-angle
			//  '>'.  This format keeps it pleasant on the eye and machine parseable.
			//
			// NOTE: My previous implementation was able to avoid the ":" somehow, need to figure
			//  out how to avoid the delimiter.
			//
			result = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}>{9}",
				(sourceType != "") ? sourceType + ":" : "",		// {0} = Source Type
				alphas[now.Year - 2014],						// {1} = Year
				alphas[now.Month],								// {2} = Month
				alphas[now.Day],								// {3} = Day
				count,											// {4} = Counter
				alphas[now.Hour],								// {5} = Hour
				alphas[now.Minute],								// {6} = Minute
				alphas[now.Second],								// {7} = Second
				Process.GetCurrentProcess().Id,					// {8} = Process ID
				uniquePart										// {9} = Unique (IP/MAC/Etc)
			);

			ID = result;
			return result;
		}
	}
}

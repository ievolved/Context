
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

using ievolved.Context;


namespace ConsoleClientTest
{
	public class UniqueContextKeyTest
	{
		[Fact]
		public void ContextKey_Create_NotNULL() {
			// Test without sourceType
			//
			// FIXED: Wrong format when not passing in a sourceType.  Need to investigate.
			//
			IContextKey key = new UniqueContextKey();
			Assert.NotNull(key.ID);
			Assert.False(key.ID.StartsWith(":"));	// FIXED: Must not start with ":"

			// Test with SourceType
			//
			key = new UniqueContextKey("CON");
			Assert.True(key.ID.StartsWith("CON:"));
		}

		[Fact]
		public void ContextKey_CreateTwo_AreUnique() {
			// Test without sourceType
			//
			IContextKey key1 = new UniqueContextKey();
			IContextKey key2 = new UniqueContextKey();

			Assert.NotEqual(key1.ID, key2.ID);
			Assert.False(key1.ID.StartsWith(":"));
			Assert.False(key2.ID.StartsWith(":"));

			// Test with sourceType
			//
			key1 = new UniqueContextKey("CON");
			key2 = new UniqueContextKey("CON");

			Assert.NotEqual(key1.ID, key2.ID);
			Assert.True(key1.ID.StartsWith("CON:"));
			Assert.True(key2.ID.StartsWith("CON:"));
		}


		[Fact]
		public void ContextKey_CreateMany_AllUnique() {
			// Attempt to overload the counter via single thread to validate the 
			//  guarantee the UniqueContextKey.counter will always be unique
			//
			var count = 10000;
			var IDs = new List<string>(count);

			for (int index = 0; index < count; index++) {
				IContextKey key = new UniqueContextKey();
				IDs.Add(key.ID);
			}

			IDs.Sort();

			string previous = IDs[0];

			// In this sorted array, no two elements should be identical.  If so, there's a problem
			//  we the uniqueness guarantee.
			//
			for (int index = 1; index < count; index++) {
				Assert.False(IDs[index] == previous);
				previous = IDs[index];
			}
		}


		[Fact]
		public void ContextKey_CreateManyParallel_AllUnique() {
			//
			// FIXED: This is known to fail, there's something fishy with the Parallel.For that 
			//  Interlocked.Increment(...) does not work correctly.  Need to investigate.
			//
			// WARN: For unknown reason the Parallel.For does not iterate as many times as
			//  count.  Need to investigate.
			//
			// Attempt to overload the counter with many threads to validate the guarantee
			//  the UniqueContextKey.counter will always be unique.
			//
			var count = 10000;
			var IDs = new List<string>(count);
			
			Parallel.For(0, count, index => {
				IContextKey key = new UniqueContextKey();
				IDs.Add(key.ID);
			});

			IDs.Sort();

			// In this sorted array, no two elements should be identical.  If so, there's a problem
			//  we the uniqueness guarantee.
			//
			for (int index = 1; index < IDs.Count; index++) {
				var current = IDs[index];
				var previous = IDs[index - 1];
				
				Assert.NotEqual<string>(current, previous);
			}
		}
	}
}

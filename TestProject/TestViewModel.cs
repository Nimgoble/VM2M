using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelToModelConverterApp;

namespace TestProject
{
	public class TestChildViewModel
	{
		[PrimaryKey]
		public long? PrimaryKey { get; set; }
		public long? ParentID { get; set; }
		public string Name { get; set; }
		public int Capacity { get; set; }
	}
	public class TestViewModel
	{
		[PrimaryKey]
		public long? PrimaryKey { get; set; }
		public string Name { get; set; }
		public bool IsDone { get; set; }
		public TestChildViewModel TestSubObject { get; set; }
		public List<TestChildViewModel> Children { get; set; }
	}
}

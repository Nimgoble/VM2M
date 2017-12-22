using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
	public class TestChildModel
	{
		public long PrimaryKey { get; set; }
		public long ParentID { get; set; }
		public string Name { get; set; }
		public int Capacity { get; set; }
	}
	public class TestModel
	{
		public long PrimaryKey { get; set; }
		public string Name { get; set; }
		public string IsDoneIndicator { get; set; }
		public TestChildModel TestSubObject { get; set; }
		public List<TestChildModel> Children { get; set; }
	}
}

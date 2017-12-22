using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ViewModelToModelConverterApp;

namespace TestProject
{
	[TestClass]
	public class ConverterTests
	{
		[TestMethod]
		public void TestConversion_Existing_MatchingChildren()
		{
			var viewModel = new TestViewModel()
			{
				Name = "Test Model - Changed",
				IsDone = true,
				PrimaryKey = 1
			};
			viewModel.Children = new System.Collections.Generic.List<TestChildViewModel>()
			{
				new TestChildViewModel()
				{
					Name = "Child 1 - Modified",
					Capacity = 100,
					ParentID = 1,
					PrimaryKey = 1
				},
				new TestChildViewModel()
				{
					Name = "Child 2 - Modified",
					Capacity = 200,
					ParentID = 1,
					PrimaryKey = 2
				}
			};

			var model = new TestModel()
			{
				Name = "Test Model",
				IsDoneIndicator = false.ToIndicator(),
				PrimaryKey = 1
			};
			model.Children = new System.Collections.Generic.List<TestChildModel>()
			{
				new TestChildModel()
				{
					Name = "Child 1",
					Capacity = 50,
					ParentID = 1,
					PrimaryKey = 1
				},
				new TestChildModel()
				{
					Name = "Child 2",
					Capacity = 100,
					ParentID = 1,
					PrimaryKey = 2
				}
			};

			var converter = new ViewModelToModelConverter();
			converter.Convert(viewModel, model, typeof(TestModel));

			Assert.AreEqual(viewModel.Name, model.Name);
			Assert.AreEqual(viewModel.IsDone, model.IsDoneIndicator.FromIndicator());
			Assert.AreEqual(viewModel.Children.Count, model.Children.Count);
			for (int i = 0; i < viewModel.Children.Count; ++i)
			{
				var vmChild = viewModel.Children[i];
				var mChild = model.Children[i];
				Assert.AreEqual(vmChild.Name, mChild.Name);
				Assert.AreEqual(vmChild.Capacity, mChild.Capacity);
			}
		}
		[TestMethod]
		public void TestConversion_NotExisting()
		{
			var viewModel = new TestViewModel()
			{
				Name = "Test Model - Changed",
				IsDone = true,
				PrimaryKey = null
			};
			viewModel.Children = new System.Collections.Generic.List<TestChildViewModel>()
			{
				new TestChildViewModel()
				{
					Name = "Child 1 - Modified",
					Capacity = 100,
					ParentID = null,
					PrimaryKey = null
				},
				new TestChildViewModel()
				{
					Name = "Child 2 - Modified",
					Capacity = 200,
					ParentID = null,
					PrimaryKey = null
				}
			};

			var converter = new ViewModelToModelConverter();
			var model = (TestModel)converter.Convert(viewModel, typeof(TestModel));

			Assert.AreEqual(viewModel.Name, model.Name);
			Assert.AreEqual(viewModel.IsDone, model.IsDoneIndicator.FromIndicator());
			Assert.AreEqual(viewModel.Children.Count, model.Children.Count);
			for (int i = 0; i < viewModel.Children.Count; ++i)
			{
				var vmChild = viewModel.Children[i];
				var mChild = model.Children[i];
				Assert.AreEqual(vmChild.Name, mChild.Name);
				Assert.AreEqual(vmChild.Capacity, mChild.Capacity);
			}
		}
		[TestMethod]
		public void TestConversion_ModelHasChildrenToDelete()
		{
			var viewModel = new TestViewModel()
			{
				Name = "Test Model - Changed",
				IsDone = true,
				PrimaryKey = 1
			};
			viewModel.Children = new System.Collections.Generic.List<TestChildViewModel>()
			{
				new TestChildViewModel()
				{
					Name = "Child 1 - Modified",
					Capacity = 100,
					ParentID = 1,
					PrimaryKey = 1
				},
				new TestChildViewModel()
				{
					Name = "Child 2 - Modified",
					Capacity = 200,
					ParentID = 1,
					PrimaryKey = 2
				}
			};

			var model = new TestModel()
			{
				Name = "Test Model",
				IsDoneIndicator = false.ToIndicator(),
				PrimaryKey = 1
			};
			model.Children = new System.Collections.Generic.List<TestChildModel>()
			{
				new TestChildModel()
				{
					Name = "Child 1",
					Capacity = 50,
					ParentID = 1,
					PrimaryKey = 1
				},
				new TestChildModel()
				{
					Name = "Child 2",
					Capacity = 100,
					ParentID = 1,
					PrimaryKey = 2
				},
				new TestChildModel()
				{
					Name = "Child 3",
					Capacity = 100,
					ParentID = 1,
					PrimaryKey = 3
				}
			};

			var converter = new ViewModelToModelConverter();
			converter.Convert(viewModel, model, typeof(TestModel));

			Assert.AreEqual(viewModel.Name, model.Name);
			Assert.AreEqual(viewModel.IsDone, model.IsDoneIndicator.FromIndicator());
			Assert.AreEqual(viewModel.Children.Count, model.Children.Count);
			for (int i = 0; i < viewModel.Children.Count; ++i)
			{
				var vmChild = viewModel.Children[i];
				var mChild = model.Children[i];
				Assert.AreEqual(vmChild.Name, mChild.Name);
				Assert.AreEqual(vmChild.Capacity, mChild.Capacity);
			}
		}
		[TestMethod]
		public void TestConversion_ViewModelHasChildrenToAdd()
		{
			var viewModel = new TestViewModel()
			{
				Name = "Test Model - Changed",
				IsDone = true,
				PrimaryKey = 1
			};
			viewModel.Children = new System.Collections.Generic.List<TestChildViewModel>()
			{
				new TestChildViewModel()
				{
					Name = "Child 1 - Modified",
					Capacity = 100,
					ParentID = 1,
					PrimaryKey = 1
				},
				new TestChildViewModel()
				{
					Name = "Child 2 - Modified",
					Capacity = 200,
					ParentID = 1,
					PrimaryKey = 2
				},
				new TestChildViewModel()
				{
					Name = "Child 3 - New",
					Capacity = 100,
					ParentID = 1,
					PrimaryKey = null
				}
			};

			var model = new TestModel()
			{
				Name = "Test Model",
				IsDoneIndicator = false.ToIndicator(),
				PrimaryKey = 1
			};
			model.Children = new System.Collections.Generic.List<TestChildModel>()
			{
				new TestChildModel()
				{
					Name = "Child 1",
					Capacity = 50,
					ParentID = 1,
					PrimaryKey = 1
				},
				new TestChildModel()
				{
					Name = "Child 2",
					Capacity = 100,
					ParentID = 1,
					PrimaryKey = 2
				}
			};

			var converter = new ViewModelToModelConverter();
			converter.Convert(viewModel, model, typeof(TestModel));

			Assert.AreEqual(viewModel.Name, model.Name);
			Assert.AreEqual(viewModel.IsDone, model.IsDoneIndicator.FromIndicator());
			Assert.AreEqual(viewModel.Children.Count, model.Children.Count);
			for (int i = 0; i < viewModel.Children.Count; ++i)
			{
				var vmChild = viewModel.Children[i];
				var mChild = model.Children[i];
				Assert.AreEqual(vmChild.Name, mChild.Name);
				Assert.AreEqual(vmChild.Capacity, mChild.Capacity);
			}
		}
		[TestMethod]
		public void TestConversion_ViewModelHasSubObject()
		{
			var viewModel = new TestViewModel()
			{
				Name = "Test Model - Changed",
				IsDone = true,
				PrimaryKey = 1,
				TestSubObject = new TestChildViewModel()
				{
					Name = "SubObject - Changed",
					Capacity = 500,
					ParentID = 1,
					PrimaryKey = 5
				}
			};
			viewModel.Children = new System.Collections.Generic.List<TestChildViewModel>();

			var model = new TestModel()
			{
				Name = "Test Model",
				IsDoneIndicator = false.ToIndicator(),
				PrimaryKey = 1,
				TestSubObject = new TestChildModel()
				{
					Name = "SubObject",
					Capacity = 300,
					ParentID = 1,
					PrimaryKey = 5
				}
			};
			model.Children = new System.Collections.Generic.List<TestChildModel>();

			var converter = new ViewModelToModelConverter();
			converter.Convert(viewModel, model, typeof(TestModel));

			Assert.AreEqual(viewModel.Name, model.Name);
			Assert.AreEqual(viewModel.IsDone, model.IsDoneIndicator.FromIndicator());
			Assert.AreEqual(viewModel.TestSubObject.Name, model.TestSubObject.Name);
			Assert.AreEqual(viewModel.TestSubObject.Capacity, model.TestSubObject.Capacity);
			Assert.AreEqual(viewModel.TestSubObject.ParentID, model.TestSubObject.ParentID);
			Assert.AreEqual(viewModel.Children.Count, model.Children.Count);
			//for (int i = 0; i < viewModel.Children.Count; ++i)
			//{
			//	var vmChild = viewModel.Children[i];
			//	var mChild = model.Children[i];
			//	Assert.AreEqual(vmChild.Name, mChild.Name);
			//	Assert.AreEqual(vmChild.Capacity, mChild.Capacity);
			//}
		}
		[TestMethod]
		public void TestConversion_ViewModelDoesNotHaveSubObjectButModelDoes()
		{
			var viewModel = new TestViewModel()
			{
				Name = "Test Model - Changed",
				IsDone = true,
				PrimaryKey = 1,
				TestSubObject = null
			};
			viewModel.Children = new System.Collections.Generic.List<TestChildViewModel>();

			var model = new TestModel()
			{
				Name = "Test Model",
				IsDoneIndicator = false.ToIndicator(),
				PrimaryKey = 1,
				TestSubObject = new TestChildModel()
				{
					Name = "SubObject",
					Capacity = 300,
					ParentID = 1,
					PrimaryKey = 5
				}
			};
			model.Children = new System.Collections.Generic.List<TestChildModel>();

			var converter = new ViewModelToModelConverter();
			converter.Convert(viewModel, model, typeof(TestModel));
			//converter.MapProperty((vm) => vm.PrimaryKey, (m) => m.PrimaryKey);

			Assert.AreEqual(viewModel.Name, model.Name);
			Assert.AreEqual(viewModel.IsDone, model.IsDoneIndicator.FromIndicator());
			Assert.AreEqual(viewModel.TestSubObject, model.TestSubObject);
			Assert.AreEqual(viewModel.Children.Count, model.Children.Count);
			//for (int i = 0; i < viewModel.Children.Count; ++i)
			//{
			//	var vmChild = viewModel.Children[i];
			//	var mChild = model.Children[i];
			//	Assert.AreEqual(vmChild.Name, mChild.Name);
			//	Assert.AreEqual(vmChild.Capacity, mChild.Capacity);
			//}
		}
	}

	[TestClass]
	public class FluentConverterTests
	{
		[TestMethod]
		public void TestConversion_Existing_MatchingChildren()
		{
			//var viewModel = new TestViewModel()
			//{
			//	Name = "Test Model - Changed",
			//	IsDone = true,
			//	PrimaryKey = 1
			//};
			//viewModel.Children = new System.Collections.Generic.List<TestChildViewModel>()
			//{
			//	new TestChildViewModel()
			//	{
			//		Name = "Child 1 - Modified",
			//		Capacity = 100,
			//		ParentID = 1,
			//		PrimaryKey = 1
			//	},
			//	new TestChildViewModel()
			//	{
			//		Name = "Child 2 - Modified",
			//		Capacity = 200,
			//		ParentID = 1,
			//		PrimaryKey = 2
			//	}
			//};

			//var model = new TestModel()
			//{
			//	Name = "Test Model",
			//	IsDoneIndicator = false.ToIndicator(),
			//	PrimaryKey = 1
			//};
			//model.Children = new System.Collections.Generic.List<TestChildModel>()
			//{
			//	new TestChildModel()
			//	{
			//		Name = "Child 1",
			//		Capacity = 50,
			//		ParentID = 1,
			//		PrimaryKey = 1
			//	},
			//	new TestChildModel()
			//	{
			//		Name = "Child 2",
			//		Capacity = 100,
			//		ParentID = 1,
			//		PrimaryKey = 2
			//	}
			//};

			var converter = new FluentViewModelToModelConverter<TestViewModel, TestModel>();
			converter.UpdateProperties((e) => new List<object>() { e.Name, e.IsDone, e.TestSubObject, e.Children });

			//Assert.AreEqual(viewModel.Name, model.Name);
			//Assert.AreEqual(viewModel.IsDone, model.IsDoneIndicator.FromIndicator());
			//Assert.AreEqual(viewModel.Children.Count, model.Children.Count);
			//for (int i = 0; i < viewModel.Children.Count; ++i)
			//{
			//	var vmChild = viewModel.Children[i];
			//	var mChild = model.Children[i];
			//	Assert.AreEqual(vmChild.Name, mChild.Name);
			//	Assert.AreEqual(vmChild.Capacity, mChild.Capacity);
			//}
		}
	}
}

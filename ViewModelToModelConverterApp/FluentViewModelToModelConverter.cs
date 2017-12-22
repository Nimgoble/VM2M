using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ViewModelToModelConverterApp
{
	public class FluentViewModelToModelConverter<From, To> : ViewModelToModelConverter where To : class, new()
	{
		public To Convert(From from, Action<object, Type> deleteAction = null) 
		{
			return (To)Convert((object)from, typeof(To), deleteAction);
		}
		public To Convert(From from, To to, Action<object, Type> deleteAction = null)
		{
			if (to == null)
				to = Activator.CreateInstance<To>();
			return (To)Convert(from, to, typeof(To), deleteAction);
		}
		public FluentViewModelToModelConverter<From, To> CopyAllMembers(bool copyAllMembers)
		{
			this.copyAllMembers = copyAllMembers;
			return this;
		}
		public FluentViewModelToModelConverter<From, To> MapProperty<FromPropertyType, ToPropertyType>(Expression<Func<From, FromPropertyType>>fromProperty, Expression<Func<To, ToPropertyType>> toProperty)
		{
			//fromProperty.GetPropertyInfo()
			return this;
		}

		public FluentViewModelToModelConverter<From, To> UpdateProperties(Expression<Func<From, List<object>>> fromProperty)
		{
			var infos = fromProperty.GetPropertyInfos();
			return this;
		}
		/// <summary>
		/// Trying to find a way to do this:
		/// converter.UpdateProperties((e) => e.Name, e.IsDone, e.TestSubObject, e.Children);
		/// </summary>
		/// <param name=""></param>
		/// <returns></returns>
		//public FluentViewModelToModelConverter<From, To> UpdateProperties(Expression<Func<> fromProperty)
		//{
		//	var infos = fromProperty.GetPropertyInfos();
		//	return this;
		//}
	}
}

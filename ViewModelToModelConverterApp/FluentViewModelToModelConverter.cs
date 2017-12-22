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
			fromProperty.GetPropertyInfo()
			return this;
		}
	}
}

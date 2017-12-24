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
		private Action<object, Type> deleteAction = null;
		private List<PropertyMapping> explicitPropertyMappings = new List<PropertyMapping>();
		private HashSet<PropertyInfo> updateProperties = new HashSet<PropertyInfo>();
		public To Convert(From from) 
		{
			return (To)Convert((object)from, typeof(To), deleteAction);
		}
		public To Convert(From from, To to)
		{
			if (to == null)
				to = Activator.CreateInstance<To>();
			return (To)Convert(from, to, typeof(To), deleteAction);
		}
		public FluentViewModelToModelConverter<From, To> WithDeleteAction(Action<object, Type> deleteAction)
		{
			this.deleteAction = deleteAction;
			return this;
		}
		public FluentViewModelToModelConverter<From, To> CopyAllMembers(bool copyAllMembers)
		{
			this.copyAllMembers = copyAllMembers;
			return this;
		}
		//The thought was to nest the typed converters and then have a method that returns the parent when we want to go back to 
		//fluently describing things on the parent.
		//public FluentViewModelToModelConverter<SubFrom, SubTo> WithSubType<SubFrom, SubTo>(Expression<Func<SubFrom, object>> fromProperty, Expression<Func<SubTo, object>> toProperty)
		//{
		//}
		public FluentViewModelToModelConverter<From, To> MapProperty<FromPropertyType, ToPropertyType>(Expression<Func<From, FromPropertyType>>fromProperty, Expression<Func<To, ToPropertyType>> toProperty)
		{
			explicitPropertyMappings.Add(new PropertyMapping() { FromType = typeof(From), FromProperty = fromProperty.GetPropertyInfo(), ToType = typeof(To), ToProperty = toProperty.GetPropertyInfo() });
			return this;
		}
		/// <summary>
		/// Example: converter.UpdateProperties((e) => new List<object>() { e.Name, e.IsDone, e.TestSubObject, e.Children });
		/// </summary>
		/// <param name="fromProperty"></param>
		/// <returns></returns>
		//public FluentViewModelToModelConverter<From, To> UpdateProperties(Expression<Func<From, List<object>>> fromProperty)
		//{
		//	var infos = fromProperty.GetPropertyInfos();
		//	return this;
		//}
		/// <summary>
		/// Example: converter.UpdateProperties(e => e.Name, e => e.IsDone, e => e.Children);
		/// </summary>
		/// <param name="fromProperties"></param>
		/// <returns></returns>
		public FluentViewModelToModelConverter<From, To> UpdateProperties(params Expression<Func<From, object>>[] fromProperties)
		{
			var propertyInfos = from e in fromProperties select e.GetPropertyInfo();
			foreach(var propertyInfo in propertyInfos)
				updateProperties.Add(propertyInfo);
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

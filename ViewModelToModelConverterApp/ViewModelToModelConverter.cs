using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ViewModelToModelConverterApp
{
	public class ViewModelToModelConverter
	{
		public class PropertyMapping
		{
			public Type FromType { get; set; }
			public PropertyInfo FromProperty { get; set; }
			public Type ToType { get; set; }
			public PropertyInfo ToProperty { get; set; }
			public ConversionRule ConversionRule { get; set; }
			public override bool Equals(object obj)
			{
				if (!(obj is PropertyMapping))
					return false;
				var castedObj = obj as PropertyMapping;
				return 
					this.FromType == castedObj.FromType && 
					this.FromProperty == castedObj.FromProperty && 
					this.ToType == castedObj.ToType && 
					this.ToProperty == castedObj.ToProperty;
			}
			public override int GetHashCode()
			{
				return Tuple.Create(FromType, FromProperty, ToType, ToProperty).GetHashCode();
			}
		}
		#region ConversionRules
		public class ConversionRule
		{
			public ConversionRule() { }
			public ConversionRule(Func<PropertyInfo, PropertyInfo, bool> qualifierMethod, Func<PropertyInfo, object, object> getValueMethod)
			{
				this.QualifierMethod = qualifierMethod;
				this.GetValueMethod = getValueMethod;
			}
			public Func<PropertyInfo, PropertyInfo, bool> QualifierMethod { get; set; }
			public Func<PropertyInfo, object, object> GetValueMethod { get; set; }
		}
		List<ConversionRule> conversionRules = new List<ConversionRule>();
		private static ConversionRule indicatorConversionRule = new ConversionRule
		(
			(from, to) =>
			{
				return
					from.PropertyType == typeof(bool) &&
					to.PropertyType == typeof(string) &&
					to.Name.ToLower().Contains(from.Name.ToLower()) &&
					to.Name.ToLower().Contains("indicator");
			},
			(fromProperty, from) => { return ((bool)fromProperty.GetValue(from)).ToIndicator(); }
		);
		#endregion

		#region Private/Protected Members
		protected bool copyAllMembers = true;
		#endregion

		#region Constructors
		public ViewModelToModelConverter()
		{
			conversionRules.Add(indicatorConversionRule);
		}
		public ViewModelToModelConverter(List<ConversionRule> conversionRules)
		{
			this.conversionRules = conversionRules;
			this.conversionRules.Add(indicatorConversionRule);
		}
		#endregion

		#region Public Methods
		public object Convert(object from, Type toType, Action<object, Type> deleteAction = null)
		{
			object rtn = Activator.CreateInstance(toType);
			return Convert(from, rtn, toType, deleteAction);
		}
		public object Convert(object from, object to, Type toType, Action<object, Type> deleteAction = null)
		{
			if (to == null)
				to = Activator.CreateInstance(toType);
			CopyProperties(from, to, deleteAction);
			return to;
		}
		#endregion

		#region Non-Public Methods
		private void MapProperties(Type fromType, Type toType)
		{

		}
		protected virtual List<PropertyMapping> GetPropertyMappings()
		{

		}
		protected virtual List<PropertyInfo> GetPropertiesForType(Type type)
		{
			return type.GetProperties().ToList();
		}
		protected void CopyProperties(object from, object to, Action<object, Type> deleteAction = null)
		{
			var fromProperties = GetPropertiesForType(from.GetType());
			var toProperties = GetPropertiesForType(to.GetType());
			bool hasConversionRules = conversionRules.Any();
			foreach (var fromProperty in fromProperties)
			{
				var foundProperty = FindCorrespondingProperty(fromProperty, toProperties);
				var correspondingToProperty = foundProperty.Item1;
				var relevantConversionRule = foundProperty.Item2;
				if (correspondingToProperty == null)
					continue;

				//Copy collections
				if (IsPropertyACollection(fromProperty))
				{
					if (!IsPropertyACollection(correspondingToProperty))
						continue;
					CopyCollection(from, fromProperty, to, correspondingToProperty, deleteAction);
				}
				else
				{
					var value = (relevantConversionRule != null) ? relevantConversionRule.GetValueMethod(fromProperty, from) : fromProperty.GetValue(from);
					try
					{
						//Copy simple types
						if (IsSimpleType(fromProperty.PropertyType) && IsSimpleType(correspondingToProperty.PropertyType))
						{
							correspondingToProperty.SetValue(to, value);
						}
						else
						{
							//Copy null objects
							var toValue = correspondingToProperty.GetValue(to);
							if (value == null)
								correspondingToProperty.SetValue(to, null);
							else
							{
								//Copy non-null objects
								Convert(value, toValue, correspondingToProperty.PropertyType, deleteAction);
							}
						}
					}
					catch { }
				}
			}
		}
		protected virtual (PropertyInfo, ConversionRule) FindCorrespondingProperty(PropertyInfo fromProperty, IEnumerable<PropertyInfo> toProperties)
		{
			var rtn = toProperties.SingleOrDefault(x => x.Name == fromProperty.Name);
			ConversionRule conversionRule = null;
			if (rtn == null)
			{
				foreach (var rule in conversionRules)
				{
					rtn = toProperties.SingleOrDefault(toProperty => rule.QualifierMethod(fromProperty, toProperty));
					if (rtn != null)
					{
						conversionRule = rule;
						break;
					}
				}
			}
			return (rtn, conversionRule);
		}
		private void CopyCollection(object from, PropertyInfo fromProperty, object to, PropertyInfo toProperty, Action<object, Type> deleteAction = null)
		{
			Type fromType = fromProperty.PropertyType.GetGenericArguments()[0];
			Type toType = toProperty.PropertyType.GetGenericArguments()[0];
			var fromCollection = (IEnumerable)fromProperty.GetValue(from);
			if (fromCollection == null)
				fromCollection = (IEnumerable)Activator.CreateInstance(typeof(List<>).MakeGenericType(fromType));

			var fromKeyProperties = fromType.GetProperties().Where(x => x.GetCustomAttribute<PrimaryKeyAttribute>() != null).ToList();
			if (!fromKeyProperties.Any())
				return; //Can't match up elements without keys.

			//Item1: ViewModel key property
			//Item2: Corresponding model key property
			var keyMappings = from fromKey in fromKeyProperties where toType.GetProperties().SingleOrDefault(x => x.Name == fromKey.Name) != null select Tuple.Create(fromKey, toType.GetProperties().SingleOrDefault(x => x.Name == fromKey.Name));

			//Need to be able to add to it, so it needs to be IList
			var toCollection = (IList)toProperty.GetValue(to);
			IList toRemove = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(toType));
			if (toCollection == null)
			{
				toCollection = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(toType));
				toProperty.SetValue(to, toCollection);
			}

			//Find the models in the model's collection that don't have a corresponding viewmodel in the viewmodel's collection
			foreach (var toElement in toCollection)
			{
				var keyValues = (from mapping in keyMappings select Tuple.Create(mapping.Item1, mapping.Item2.GetValue(toElement))).ToList();
				var matchingElement = FindMatchingObject(fromCollection, keyValues);
				if (matchingElement == null)
					toRemove.Add(toElement);
			}
			//Remove the ones that are no longer there.
			foreach (var remove in toRemove)
			{
				toCollection.Remove(remove);
				deleteAction?.Invoke(remove, toType);
			}

			foreach (var fromElement in fromCollection)
			{
				var keyValues = (from mapping in keyMappings select Tuple.Create(mapping.Item2, mapping.Item1.GetValue(fromElement))).ToList();
				var matchingElement = FindMatchingObject(toCollection, keyValues);
				bool notFound = matchingElement == null;
				matchingElement = Convert(fromElement, matchingElement, toType);
				if (notFound)
					toCollection.Add(matchingElement);
			}
		}
		protected virtual object FindMatchingObject(IEnumerable targetCollection, List<Tuple<PropertyInfo, object>> keyValues)
		{
			object rtn = null;
			foreach (var target in targetCollection)
			{
				if (ObjectIsAMatch(target, keyValues))
				{
					rtn = target;
					break;
				}
			}

			return rtn;
		}
		protected bool ObjectIsAMatch(object target, List<Tuple<PropertyInfo, object>> keyValues)
		{
			return !keyValues.Any(keyValue => !keyValue.Item1.GetValue(target).Equals(keyValue.Item2));
		}
		protected bool IsPropertyACollection(PropertyInfo property)
		{
			return (!typeof(String).Equals(property.PropertyType) &&
				typeof(IEnumerable).IsAssignableFrom(property.PropertyType));
		}
		/// <summary>
		/// https://stackoverflow.com/questions/2442534/how-to-test-if-type-is-primitive
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		protected bool IsSimpleType(Type type)
		{
			return
				type.IsPrimitive ||
				new Type[] {
			typeof(Enum),
			typeof(String),
			typeof(Decimal),
			typeof(DateTime),
			typeof(DateTimeOffset),
			typeof(TimeSpan),
			typeof(Guid)
				}.Contains(type) ||
				System.Convert.GetTypeCode(type) != TypeCode.Object ||
				(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && IsSimpleType(type.GetGenericArguments()[0]))
				;
		}
		#endregion
	}
}
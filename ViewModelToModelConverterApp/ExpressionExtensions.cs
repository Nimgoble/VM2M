namespace ViewModelToModelConverterApp
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;

	/// <summary>
	/// Extension for <see cref="Expression"/>.
	/// </summary>
	public static class ExpressionExtensions
	{
		/// <summary>
		/// Converts an expression into a <see cref="MemberInfo"/>.
		/// </summary>
		/// <param name="expression">The expression to convert.</param>
		/// <returns>The member info.</returns>
		public static MemberInfo GetMemberInfo(this Expression expression)
		{
			if (expression is MemberExpression)
				return ((MemberExpression)expression).Member;
			if(expression is UnaryExpression)
			{
				var unaryExpression = (UnaryExpression)expression;
				return ((MemberExpression)unaryExpression.Operand).Member;
			}
			else if(expression is LambdaExpression)
			{
				var lambda = (LambdaExpression)expression;

				MemberExpression memberExpression;
				if (lambda.Body is UnaryExpression)
				{
					var unaryExpression = (UnaryExpression)lambda.Body;
					memberExpression = (MemberExpression)unaryExpression.Operand;
				}
				else
				{
					memberExpression = (MemberExpression)lambda.Body;
				}

				return memberExpression.Member;
			}

			return null;
		}

		public static List<MemberInfo> GetMemberInfos(this Expression expression)
		{
			var lambda = (LambdaExpression)expression;
			var rtn = new List<MemberInfo>();

			ListInitExpression listInitExpression = (ListInitExpression)lambda.Body;
			//listInitExpression.Reduce();
			foreach(var i in listInitExpression.Initializers)
			{
				foreach(var a in i.Arguments)
				{
					rtn.Add(a.GetMemberInfo());
				}
			}

			return rtn;
		}

		public static PropertyInfo GetPropertyInfo(this Expression expression)
		{
			return (PropertyInfo)GetMemberInfo(expression);
		}

		public static List<PropertyInfo> GetPropertyInfos(this Expression expression)
		{
			var memberInfos = expression.GetMemberInfos();
			return (from m in memberInfos select (PropertyInfo)m).ToList();
		}
	}
}

using FluentValidation;
using HotChocolate.Types.Descriptors;
using HotChocolate.Types.Descriptors.Definitions;
using System.Reflection;

namespace wa_api.GraphQL.Middlewares.Validate
{
	public class UseValidateAttribute<T> : ArgumentDescriptorAttribute
	{
		private readonly Type _type;

		public UseValidateAttribute()
		{
			_type = typeof(T);
		}

		protected override void OnConfigure(IDescriptorContext context, IArgumentDescriptor descriptor, ParameterInfo parameter)
		{
			descriptor.Extend().OnBeforeCreate(definition =>
			{
				definition.ContextData[ValidationConstants.Validators] = _type;
			});
		}
	}
}

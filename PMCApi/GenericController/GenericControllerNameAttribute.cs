using System;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Test.Controllers;

namespace Test
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]

    public class GenericControllerNameAttribute : Attribute, IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.GetGenericTypeDefinition() == typeof(GenericController<>))
            {
                var entityType = controller.ControllerType.GenericTypeArguments[0];
                controller.ControllerName = entityType.Name;
            }
        }
    }
}
using System;
using System.Linq.Expressions;

namespace Puppetry.Puppeteer.Conditions
{
    internal class Component : Condition
    {
        private readonly string _componentName;
        private readonly string _propertyName;
        private readonly string _propertyValue;
        private string _actualComponent;
        private Expression<Func<string, bool>> _condition;

        public Component(string componentNameName)
        {
            _componentName = componentNameName;
        }
        
        public Component(string componentNameName, Expression<Func<string, bool>> condition)
        {
            _componentName = componentNameName;
            _condition = condition;
        }
        
        public Component(string componentNameName, string propertyName, string propertyValue)
        {
            _componentName = componentNameName;
            _propertyName = propertyName;
            _propertyValue = propertyValue;
        }

        public override bool Invoke<T>(T gameObject)
        {
            CurentGameObject = gameObject;

            _actualComponent = gameObject.GetComponent(_componentName);
            //Want to check if property with specific name and value of the component exist
            if (!string.IsNullOrEmpty(_propertyName))
                return _actualComponent.Contains($"{_propertyName}\":{_propertyValue}");
            //Check specific condition
            if (_condition != null)
                _condition.Compile().Invoke(_actualComponent);
            //So we check just if component exist
            return !string.IsNullOrEmpty(_actualComponent);
        }

        protected override string DescribeExpected()
        {
            if (string.IsNullOrEmpty(_propertyName) && string.IsNullOrEmpty(_propertyValue))
                return $"has component {_componentName}";
            if (_condition != null)
                return $"has component {_componentName} with condition {_condition}";

            return $"Component {_componentName} has property {_propertyName} with value {_propertyValue}";
        }

        protected override string DescribeActual()
        {
            if (string.IsNullOrEmpty(_propertyName) && string.IsNullOrEmpty(_propertyValue))
                return $"does not have component {_componentName}";
            if (_condition != null)
                return $"condition is not fulfilled. \nActual component is {_actualComponent ?? "null"}";

            return $"does not have given component. \nActual component is {_actualComponent ?? "null"}";
        }
    }

    public static partial class Have
    {
        public static Condition Component(string componentName)
        {
            return new Component(componentName);
        }

        public static Condition ComponentWithCondition(string componentName, Expression<Func<string, bool>> condition)
        {
            return new Component(componentName, condition);
        }
        
        public static Condition ComponentWithPropertyAndValue(string componentName, string propertyName, string propertyValue)
        {
            return new Component(componentName, propertyName, propertyValue);
        }
    }
}


using Metalama.Attributes;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Filters
{
    [CompileTime]
    public static class SensitiveParameterFilter
    {
        private static readonly string[] _sensitiveNames = new[] { "password", "credential", "pwd", "pass", "login" };

        public static bool IsSensitive(IParameter parameter)
        {
            if (parameter.Attributes.OfAttributeType(typeof(NotLoggedAttribute)).Any())
            {
                return true;
            }

            if (_sensitiveNames.Any(n => parameter.Name.ToLowerInvariant().Contains(n)))
            {
                return true;
            }

            return false;
        }
    }
}

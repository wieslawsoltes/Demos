using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Processing.Core
{
    internal class CandidateMethod
    {
        private object _target;
        public MethodInfo Method { get; set; }
        public List<object> ParameterValues { get; set; }

        public CandidateMethod(object target)
        {
            _target = target;
        }

        public object Invoke()
        {
            return Method.Invoke(_target, ParameterValues.ToArray());
        }
    }
}

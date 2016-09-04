using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Processing.Core
{
    public static class Compiler
    {
        internal static List<CandidateMethod> FindOperations(this object context, string name)
        {
            return context
                .GetType()
                .GetMethods()
                .Where(m => m.IsDefined(typeof(GeometryCommandAttribute)))
                .Where(m => m.GetCustomAttribute<GeometryCommandAttribute>().Command.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                .Select(m => new CandidateMethod(context)
                {
                    Method = m,
                    ParameterValues = new List<object>()
                })
                .OrderByDescending(o => o.Method.GetParameters().Length)
                .ToList();
        }

        public static void Execute(this object context, string data)
        {
            if (string.IsNullOrEmpty(data))
                return;

            var parts = new Queue<string>(Regex.Split(data, @"\s+"));
            var currentOperations = new List<CandidateMethod>();

            CandidateMethod lastCandidate = null;
            int paramNumber = 0;

            while (parts.Count != 0)
            {
                string p = parts.Dequeue();
                paramNumber++;

                foreach (var candidate in currentOperations.ToList())
                {
                    var parameters = candidate.Method.GetParameters();
                    if (parameters.Length > paramNumber - 1)
                    {
                        var parameter = parameters[paramNumber - 1];
                        if (parameter.ParameterType == typeof(double))
                        {
                            double result;
                            if (!double.TryParse(p, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                            {
                                currentOperations.Remove(candidate);
                                if (currentOperations.Count == 0)
                                {
                                    lastCandidate = candidate;
                                }
                            }
                            else
                            {
                                candidate.ParameterValues.Add(result);
                            }
                        }
                        else if (parameter.ParameterType == typeof(int))
                        {
                            int result;
                            if (!int.TryParse(p, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                            {
                                currentOperations.Remove(candidate);
                                if (currentOperations.Count == 0)
                                {
                                    lastCandidate = candidate;
                                }
                            }
                            else
                            {
                                candidate.ParameterValues.Add(result);
                            }
                        }
                        else if (parameter.ParameterType == typeof(bool))
                        {
                            bool result;
                            if (!bool.TryParse(p, out result))
                            {
                                currentOperations.Remove(candidate);
                                if (currentOperations.Count == 0)
                                {
                                    lastCandidate = candidate;
                                }
                            }
                            else
                            {
                                candidate.ParameterValues.Add(result);
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException("Not supported parameter type.");
                        }
                    }
                    else
                    {
                        currentOperations.Remove(candidate);
                        if (currentOperations.Count == 0)
                        {
                            lastCandidate = candidate;
                        }
                    }
                }

                if (lastCandidate != null)
                {
                    lastCandidate.Invoke();
                    lastCandidate = null;
                }

                if (currentOperations.Count == 0)
                {
                    currentOperations = FindOperations(context, p);
                    if (currentOperations.Count == 0)
                    {
                        throw new InvalidOperationException("Geometry command : " + p + " is unknown.");
                    }

                    paramNumber = 0;
                    continue;
                }
            }

            var lastOperation = currentOperations.LastOrDefault();
            if (lastOperation != null)
            {
                lastOperation.Invoke();
            }
        }
    }
}

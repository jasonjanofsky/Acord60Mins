using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Acord60Mins
{
	public class Reflection
	{
		
		/// <summary>
		/// Runs a method by name using reflection.
		/// </summary>
		/// <param name="dllPath">The path to the DLL that the class is located in.</param>
		/// <param name="className">The full namespace and class name to run.</param>
		/// <param name="methodName">The method to run.</param>
		/// <param name="paramters">An array of objects that matches the arguments of the method being run.</param>
		/// <returns>The return of the method call.</returns>
		public static object RunMethod(string dllPath, string className, string methodName, params Object[] parameters) {
			Assembly _Assemblies = Assembly.LoadFrom(dllPath);

			Type _Type = null;
			try
			{
				_Type = _Assemblies.GetType(className);
			}
			catch (Exception)
			{
				throw new Exception("Could not find assembly name.");
			}

			MethodInfo _MethodInfo = null;
			try
			{
				_MethodInfo = _Type.GetMethod(methodName);
			}
			catch (Exception)
			{
				throw new Exception("Could not find method name.");
			}

			Object _InvokeParam1 = Activator.CreateInstance(_Type);
			try
			{
				return _MethodInfo.Invoke(_InvokeParam1, parameters);
			}
			catch (Exception ex)
			{
				if (_InvokeParam1 == null)
				{
					throw new Exception("Method reference failed.");
				}
				if (parameters == null)
				{
					throw new Exception("Parameters are null.");
				}
				throw ex;
			}
		}

	}
}
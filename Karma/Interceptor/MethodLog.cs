using Castle.DynamicProxy;
using System;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;
using Karma.Data;

namespace Karma.Interceptor
{
    public class MethodLog : Attribute, IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            try
            {
                string module = invocation.Method.DeclaringType.FullName;
                string method = invocation.Method.Name;
                JsonSerializerOptions options = new()
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true
                };

                string args = getArgsWithoutContext(invocation, options);

                Log.Logger.Information($"Calling method: {method} " +
                                       $" in module: {module}\n" +
                                       $"with args: {args}\n");


                invocation.Proceed();
                string returnValue = JsonSerializer.Serialize(invocation.ReturnValue, options);

                Log.Logger.Information($"Method finished successfully, returned: {returnValue}\n");
                
            }
            catch(Exception e)
            {
                Log.Logger.Information($"Method failed, thrown exception: {JsonSerializer.Serialize(e)}\n");
            }
        }

        private string getArgsWithoutContext(IInvocation invocation, JsonSerializerOptions options)
        {
            string args = "";
            foreach (var obj in invocation.Arguments)
            {
                if (obj is not KarmaContext)
                {
                    args += JsonSerializer.Serialize(obj, options) + ",\n";
                }
            }
            return "(" + args.Remove(args.Length - 1, 1) + ")";
        }
    }
}

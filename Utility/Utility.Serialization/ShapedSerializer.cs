using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Serialization
{
    public class ShapedSerializer : IObjectSerializer
    {
        public T GetObject<T>(IObjectReader source)
        {
            return Helper<T>.GetObject(source);
        }

        public void GetObjectData<T>(IObjectWriter target, T instance)
        {
            Helper<T>.GetObjectData(target, instance);
        }

        private static class Helper<T>
        {
            public static Func<Exception>? ExceptionFactory { get; }

            private static readonly Action<IObjectWriter, T>? getObjectDataImpl;
            private static readonly Func<IObjectReader, T>? getObjectImpl;

            static Helper()
            {
                MethodInfo? serializeMethod;
                MethodInfo? deserializeMethod;
                ConstructorInfo? deserializeConstructor = null;

                ShapedSerializerAttribute? attribute = typeof(T).GetCustomAttribute<ShapedSerializerAttribute>(false);
                bool bindByOrder = !(attribute?.BindByName ?? false);

                ExceptionFactory = ResolveSerializeDeserialize(out serializeMethod, out deserializeMethod, out deserializeConstructor, bindByOrder);

                if (ExceptionFactory != null)
                    return;

                getObjectDataImpl = Serialization(serializeMethod!);
                getObjectImpl = Deserialization(deserializeMethod, deserializeConstructor, serializeMethod!, bindByOrder);
            }

            private static Func<IObjectReader, T> Deserialization(MethodInfo? method, ConstructorInfo? constructor, MethodInfo serializationMethod, bool bindByOrder)
            {
                //note that we know that Serialize and Deserialize/.ctr have matching signatures w.r.t. bindByOrder
                //we do not have extra parameters and all parameters have a match
                //--> even with bindByName we are sure that none of the parameters causes trouble in that regard
                var readerParameter = Expression.Parameter(typeof(IObjectReader));
                MethodInfo readMethodGeneric = typeof(IObjectReader).GetMethod("Read")!;
                List<Expression>? block = null;
                List<ParameterExpression>? variables = null;

                Expression[] parameterValues;

                if (!bindByOrder)
                {
                    var locals = new Dictionary<string, ParameterExpression>();
                    //if binding is per name, we'll first have to retrieve the values from the reader
                    //and afterwards reconstruct the instance from these values. Therefore, we first
                    //declare the locals with the values

                    var serializationParameters = serializationMethod.GetParameters();
                    int i = 0;

                    if ((serializationMethod.CallingConvention & CallingConventions.HasThis) != CallingConventions.HasThis)
                        i++;

                    int j = 0;


                    block = new List<Expression>();
                    variables = new List<ParameterExpression>();

                    for (; i < serializationParameters.Length; i++, j++)
                    {
                        var p = serializationParameters[i];
                        //first, we declare a variable in which we are going to store the value corresponding to the parameter
                        var local = Expression.Variable(p.ParameterType.GetElementType()!);

                        //we read to that variable
                        var readCall = Expression.Call(readerParameter, readMethodGeneric.MakeGenericMethod(p.ParameterType.GetElementType()!));
                        block.Add(Expression.Assign(local, readCall));
                        variables.Add(local);

                        //we store it in the parameter map
                        locals[p.Name!] = local;
                    }

                    //now we just have to add the call. We reorder the parameters
                    parameterValues = Array.ConvertAll<ParameterInfo, Expression>(
                        method != null ? method.GetParameters() : constructor!.GetParameters(),
                        (p) => locals[p.Name!]);
                }
                else
                {
                    //binding by order is straightforward
                    parameterValues = Array.ConvertAll<ParameterInfo, Expression>(
                        method != null ? method.GetParameters() : constructor!.GetParameters(),
                        (p) =>
                        {
                            var readMethod = readMethodGeneric.MakeGenericMethod(p.ParameterType);

                            return Expression.Call(readerParameter, readMethod);
                        });
                }

                //call whichever option is the one to go with
                Expression deserialization = constructor == null ? (Expression)Expression.Call(method!, parameterValues) : Expression.New(constructor, parameterValues);

                //if we have preceding variable assignments, we view the whole thing as a block
                if (block != null)
                {
                    block.Add(deserialization);
                    deserialization = Expression.Block(deserialization.Type, variables, block.ToArray());
                }

                return Expression.Lambda<Func<IObjectReader, T>>(deserialization, readerParameter).Compile();
            }

            private static Action<IObjectWriter, T> Serialization(MethodInfo method)
            {
                var writerParameter = Expression.Parameter(typeof(IObjectWriter));
                var instanceParameter = Expression.Parameter(typeof(T));
                var writeMethod = typeof(IObjectWriter).GetMethod("Write")!;

                var parameterValues = new List<ParameterExpression>();
                var variables = new List<ParameterExpression>();

                Expression? callInstanceParameter = null;

                int i = 0;

                if ((method.CallingConvention & CallingConventions.HasThis) == CallingConventions.HasThis)
                {
                    //instance method ==> use callInstanceParameter to call the Serialize method on
                    callInstanceParameter = instanceParameter;
                }
                else
                {
                    //static method ==> we have an additional parameter value to send to the type
                    parameterValues.Add(instanceParameter);
                    i++;
                }

                var parameters = method.GetParameters();

                for (; i < parameters.Length; i++)
                {
                    var variable = Expression.Variable(parameters[i].ParameterType.GetElementType()!);

                    variables.Add(variable);
                    parameterValues.Add(variable);
                }

                var block = new List<Expression>();

                //first, we call to the Serialize-method and retrieve the out values in variables
                block.Add(Expression.Call(callInstanceParameter, method, parameterValues.ToArray()));

                //then, we feed each to the writer
                foreach (var v in variables)
                {
                    block.Add(Expression.Call(writerParameter, writeMethod.MakeGenericMethod(v.Type), v));
                }

                return Expression.Lambda<Action<IObjectWriter, T>>(Expression.Block(variables, block), writerParameter, instanceParameter).Compile();
            }

            private static Func<Exception>? ResolveSerializeDeserialize(out MethodInfo? serializeMethod,
                out MethodInfo? deserializeMethod,
                out ConstructorInfo? deserializeConstructor,
                bool bindByOrder = false)
            {
                serializeMethod = null;
                deserializeMethod = null;
                deserializeConstructor = null;
                Type[]? signature = null;

                try
                {
                    serializeMethod = typeof(T).GetMethod("Serialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                }
                catch (AmbiguousMatchException ex)
                {
                    return () => new BindingException("Multiple Serialize methods found.", ex);
                }

                if (serializeMethod == null)
                {
                    return () => new BindingException("The type does not provide a valid Serialize method.");
                }

                var parameters = serializeMethod.GetParameters();

                int i = 0;
                int j = 0;

                //if not an instance method, the first parameter must indicate the instance to serialize
                //we do not allow the parameter to be an out or ref-parameter
                if ((serializeMethod.CallingConvention & CallingConventions.HasThis) != CallingConventions.HasThis)
                {
                    if (parameters[0].ParameterType != typeof(T) || parameters[0].ParameterType.IsByRef)
                    {
                        return () => new BindingException("The first parameter of a static serialization method must be the instance to serialize and an in-parameter.");
                    }
                    i++;
                }

                signature = new Type[parameters.Length - i];

                for (; i < parameters.Length; i++, j++)
                {
                    var p = parameters[i];

                    if (!p.IsOut)
                    {
                        return () => new BindingException($"Parameter {p.Name} is not an out-parameter on the serializer.");
                    }

                    signature[j] = p.ParameterType.GetElementType()!;
                }

                //try to fetch a deserialization method
                //there are two options: bind considering the order of the Serialize method or bind by name
                if (bindByOrder)
                {
                    try
                    {
                        deserializeMethod = typeof(T).GetMethod("Deserialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, signature, null);

                        if (deserializeMethod != null && deserializeMethod.ReturnType != typeof(T))
                            return () => new BindingException($"Deserialize method return type is not {typeof(T)}.");
                        else if (deserializeMethod == null)
                        {
                            //does any Deserialize method exist?
                            //If one exists there must be one that matches the signature to prevent implementation errors
                            try
                            {
                                deserializeMethod = typeof(T).GetMethod("Deserialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                            }
                            catch (AmbiguousMatchException)
                            {
                                return () => new BindingException($"Deserialize-signature mismatching Serialize-signature.");
                            }

                            if (deserializeMethod != null)
                            {
                                return () => new BindingException($"Deserialize-signature mismatching Serialize-signature.");
                            }
                        }
                    }
                    catch (AmbiguousMatchException ex)
                    {
                        return () => new BindingException("Multiple matching static Deserialize-methods found.", ex);
                    }
                }
                else
                {
                    var deserializeMethods = typeof(T)
                        .GetMember("Deserialize", MemberTypes.Method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                        .OfType<MemberInfo>()
                        .Cast<MethodBase>()
                        .Concat(typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                        .ToArray();

                    var serializeParameters = parameters!.Select((s) => (s.Name!, s.ParameterType.GetElementType()!));

                    //for non-static Serialize-methods, we'll have to skip the first parameter (the instance on which to invoke)
                    if ((serializeMethod.CallingConvention & CallingConventions.HasThis) != CallingConventions.HasThis)
                        serializeParameters = serializeParameters.Skip(1);

                    var namedSignature = new HashSet<(string, Type)>(serializeParameters);

                    MethodBase? match = null;

                    foreach (var candidate in deserializeMethods)
                    {
                        //if we find a Deserialize-method, we ignore the constructors
                        if (candidate.MemberType == MemberTypes.Constructor && (match != null && match.MemberType == MemberTypes.Method))
                            break;

                        if (namedSignature.SetEquals(candidate.GetParameters().Select((s) => (s.Name!, s.ParameterType))))
                        {
                            if (match != null)
                                if (candidate.MemberType == MemberTypes.Constructor)
                                    return () => new BindingException("Multiple matching static Deserialize-constructors found.");
                                else
                                    return () => new BindingException("Multiple matching static Deserialize-methods found.");
                            match = candidate;
                        }
                    }

                    deserializeMethod = match as MethodInfo;
                    deserializeConstructor = match as ConstructorInfo;
                }

                //if we fail to fetch a deserialization method, we try to find a constructor instead unless we already did (bind by name)
                if (deserializeMethod == null)
                {
                    if (deserializeConstructor == null)
                        deserializeConstructor = typeof(T).GetConstructor(signature);

                    if (deserializeConstructor == null)
                    {
                        return () => new BindingException("Missing static Deserialize-method or deserialization constructor.");
                    }
                }

                return null;
            }

            public static T GetObject(IObjectReader source)
            {
                if (ExceptionFactory != null)
                    throw ExceptionFactory();

                return getObjectImpl!(source);
            }

            public static void GetObjectData(IObjectWriter target, T instance)
            {
                if (ExceptionFactory != null)
                    throw ExceptionFactory();

                getObjectDataImpl!(target, instance);
            }
        }
    }
}

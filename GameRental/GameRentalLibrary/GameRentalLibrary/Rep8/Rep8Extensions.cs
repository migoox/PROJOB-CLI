using GameRental.Rep0;
using GameRental.Rep8;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GameRental.Extensions
{
    public static class Rep8ToolsExtension
    {
        public static string[] GetVariableFromStack(this IStackRepresentation stack, string variableName)
        {
            var array = stack.Data.Item2.ToArray();
            int startIndex = 0;
            int count = 0;
            for (int i = 0; i < array.Length; ++i)
            {
                if (array[i] == variableName)
                {
                    count = Convert.ToInt32(array[i + 1]);
                    startIndex = i + 2;
                }
            }
            var result = new string[count];
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = array[startIndex + i];
            }
            return result;
        }

        public static void SetVariableOnStack(this IStackRepresentation stack, string variableName, string[] value)
        {
            var array = stack.Data.Item2.ToArray();
            int startIndex = 0;
            int count = 0;
            
            var newStack = new Stack<string>();
            for (int i = 0; i < array.Length; ++i)
            {
                newStack.Push(array[i]);
                if (array[i] == variableName)
                {
                    newStack.Push(Convert.ToString(value.Length));
                    for (int j = 0; j < value.Length; j++)
                    {
                        newStack.Push(value[j]);
                    }
                    i += Convert.ToInt32(array[i + 1]) + 1;
                }
            }

            stack.Data.Item2.Clear();
            foreach (var elem in newStack)
            {
                stack.Data.Item2.Push(elem);
            }

        }

        public static List<T> GetCollectionFromStack<T>(this IStackRepresentation stack, string variableName)
            where T : IDatabaseEntity
        {
            var elems = GetVariableFromStack(stack, variableName);
            List<T> list = new List<T>();
            var dictionary = (Dictionary<int, T>)Database.Instance.GetTable<T>();

            for (int i = 0; i < elems.Length; i++)
            {
                int id = Convert.ToInt32(elems[i]);
                T? result = dictionary.FirstOrDefault(r => r.Key == id).Value;

                if (result != null) list.Add(result);
            }

            return list;
        }

        public static void SetCollectionOnStack<T>(this IStackRepresentation stack, string variableName, List<T> collection)
            where T : IDatabaseEntity
        {
            string[] indices = new string[collection.Count];
            int i = 0;
            foreach (int id in collection.Select(x => x.Id))
            {
                indices[i] = id.ToString();
                ++i;
            }

            SetVariableOnStack(stack, variableName, indices);
        }
    }
}

﻿// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Util.Scanning
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;


    public interface IAssemblyScanner
    {
        /// <summary>
        /// Optional user-supplied diagnostic description of this scanning operation
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Add an Assembly to the scanning operation
        /// </summary>
        /// <param name="assembly"></param>
        void Assembly(Assembly assembly);

        /// <summary>
        /// Add an Assembly by name to the scanning operation
        /// </summary>
        /// <param name="assemblyName"></param>
        void Assembly(string assemblyName);

        /// <summary>
        /// Add the Assembly that contains type T to the scanning operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void AssemblyContainingType<T>();

        /// <summary>
        /// Add the Assembly that contains type to the scanning operation
        /// </summary>
        /// <param name="type"></param>
        void AssemblyContainingType(Type type);

        /// <summary>
        /// Exclude types that match the Predicate from being scanned
        /// </summary>
        /// <param name="exclude"></param>
        void Exclude(Func<Type, bool> exclude);

        /// <summary>
        /// Exclude all types in this nameSpace or its children from the scanning operation
        /// </summary>
        /// <param name="nameSpace"></param>
        void ExcludeNamespace(string nameSpace);

        /// <summary>
        /// Exclude all types in this nameSpace or its children from the scanning operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void ExcludeNamespaceContainingType<T>();

        /// <summary>
        /// Only include types matching the Predicate in the scanning operation. You can 
        /// use multiple Include() calls in a single scanning operation
        /// </summary>
        /// <param name="predicate"></param>
        void Include(Func<Type, bool> predicate);

        /// <summary>
        /// Only include types from this nameSpace or its children in the scanning operation.  You can 
        /// use multiple Include() calls in a single scanning operation
        /// </summary>
        /// <param name="nameSpace"></param>
        void IncludeNamespace(string nameSpace);

        /// <summary>
        /// Only include types from this nameSpace or its children in the scanning operation.  You can 
        /// use multiple Include() calls in a single scanning operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void IncludeNamespaceContainingType<T>();

        /// <summary>
        /// Exclude this specific type from the scanning operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void ExcludeType<T>();

        void TheCallingAssembly();

        void AssembliesFromApplicationBaseDirectory();

        void AssembliesAndExecutablesFromPath(string path);
        void AssembliesFromPath(string path);

        void AssembliesAndExecutablesFromPath(string path, Func<Assembly, bool> assemblyFilter);

        void AssembliesFromPath(string path, Func<Assembly, bool> assemblyFilter);
        void ExcludeFileNameStartsWith(params string[] startsWith);
        void IncludeFileNameStartsWith(params string[] startsWith);
        void AssembliesAndExecutablesFromApplicationBaseDirectory();
        Task<TypeSet> ScanForTypes();
    }
}
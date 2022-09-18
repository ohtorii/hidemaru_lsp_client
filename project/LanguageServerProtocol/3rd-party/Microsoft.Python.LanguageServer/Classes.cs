﻿//Modifyed 2021 ohtorii.


// Copyright(c) Microsoft Corporation
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABILITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System;

namespace Microsoft.Python.LanguageServer.Protocol
{
    /// <summary>
    /// Required layout for the initializationOptions member of initializeParams
    /// </summary>
    sealed class PythonInitializationOptions
    {
        public struct Interpreter
        {
            public sealed class InterpreterProperties
            {
                public string Version;
                public string InterpreterPath;
                public string DatabasePath;
            }
            public InterpreterProperties properties;
        }
        public Interpreter interpreter;

        /// <summary>
        /// Paths to search when attempting to resolve module imports.
        /// </summary>
        public string[] searchPaths = Array.Empty<string>();

        /// <summary>
        /// Paths to search for module stubs.
        /// </summary>
        public string[] typeStubSearchPaths = Array.Empty<string>();

        /// <summary>
        /// Glob pattern of files and folders to exclude from loading
        /// into the Python analysis engine.
        /// </summary>
        public string[] excludeFiles = Array.Empty<string>();

        /// <summary>
        /// Glob pattern of files and folders under the root folder that
        /// should be loaded into the Python analysis engine.
        /// </summary>
        public string[] includeFiles = Array.Empty<string>();

        /// <summary>
        /// Path to a writable folder where analyzer can cache its data.
        /// </summary>
        public string cacheFolderPath;
    }
}

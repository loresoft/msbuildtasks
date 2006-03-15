//Copyright © 2006, Jonathan de Halleux
//http://blog.dotnetwiki.org/default,month,2005-07.aspx

using System;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

// $Id$

namespace MSBuild.Community.Tasks.Schema
{
    internal abstract class TaskVisitorBase<T> 
        where T : ITask
    {
        private T parent;
        private Assembly taskAssembly;
        public TaskVisitorBase(T parent, Assembly taskAssembly )
        {
            this.parent = parent;
            this.taskAssembly = taskAssembly;
        }

        public T Parent
        {
            get { return this.parent; }
        }

        public Assembly TaskAssembly
        {
            get { return this.taskAssembly; }
            set { this.taskAssembly = value; }
        }

        protected IEnumerable<Type> GetTaskTypes()
        {
            foreach (Type type in this.TaskAssembly.GetExportedTypes())
            {
                if (!typeof(ITask).IsAssignableFrom(type))
                    continue;
                yield return type;
            }
        }

        protected IEnumerable<PropertyInfo> GetProperties(Type taskType)
        {
            foreach (PropertyInfo property in taskType.GetProperties())
            {
                if (property.DeclaringType != taskType)
                    continue;
                yield return property;
            }
        }
    }
}

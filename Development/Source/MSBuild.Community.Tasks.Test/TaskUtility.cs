// $Id$

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks.Test
{
    public class TaskUtility
    {

        public static TaskItem[] StringArrayToItemArray(string[] items)
        {
            TaskItem[] taskItems = new TaskItem[items.Length];
            
            for (int i = 0; i < items.Length; i++)
            {
                taskItems[i] = new TaskItem(items[i]);
            }
            return taskItems;
        }
    }
}

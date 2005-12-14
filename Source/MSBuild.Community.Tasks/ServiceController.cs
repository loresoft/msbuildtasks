using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Service = System.ServiceProcess;

namespace MSBuild.Community.Tasks
{
    /// <summary>
    /// Task that can control a Windows service.
    /// </summary>
    public class ServiceController : Task
    {

        public ServiceController()
        {

        }

        private string _serviceName;

        public string ServiceName
        {
            get { return _serviceName; }
            set { _serviceName = value; }
        }

        private string _machineName;

        public string MachineName
        {
            get { return _machineName; }
            set { _machineName = value; }
        }

        private string _action;

        public string Action
        {
            get { return _action; }
            set { _action = value; }
        }

        private double _timeout;

        public double Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }            
        
        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns><see langword="true"/> if the task ran successfully; 
        /// otherwise <see langword="false"/>.</returns>
        public override bool Execute()
        {
            // get handle to service
            Service.ServiceController controller = new Service.ServiceController(_serviceName, _machineName);
            //TODO ...

            
            return true;
        }


    }
}

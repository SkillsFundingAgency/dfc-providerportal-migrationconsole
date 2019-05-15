using Dfc.CourseDirectory.Models.Interfaces.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.CourseDirectory.ApprenticeshipMigrationTool.Models
{
    public class Provideralias : IProvideralias
    {
        public object ProviderAlias { get; set; }
        public object LastUpdated { get; set; }
    }
}

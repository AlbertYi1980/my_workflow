using System.Data.Entity;
using System.Data.SqlServerCe;

namespace WorkflowHost.Database
{
    public class WorkflowDefinitionContext : DbContext
    {
        public WorkflowDefinitionContext(): 
            base(new SqlCeConnection("Data Source=e:\\MyDatabase.sdf;Persist Security Info=False;"), contextOwnsConnection: true)
        {

        }
        public DbSet<WorkflowDefinition> WorkflowDefinitions { get; set; }

    }

}
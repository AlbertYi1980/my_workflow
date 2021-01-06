using System.Activities;

namespace Mirror.Workflows.CustomActivities.MirrorCrud
{
    public abstract class MirrorCrud<T> : NativeActivity<T>
    {
        public InArgument<string> MirrorBase { get; set; }
        public InArgument<string>   TenantId { get; set; }
        public string ModelKey { get; set; }

       
        
    }
}
using System;
using System.Activities;
using System.Net.Http;
using System.Text;

namespace WorkflowCore.Activities
{
    public sealed class MirrorDelete : MirrorCrud
    {
        protected override string CrudType => "delete";
    }
}
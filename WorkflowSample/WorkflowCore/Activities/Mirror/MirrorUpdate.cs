using System;
using System.Activities;
using System.Net.Http;
using System.Text;

namespace WorkflowCore.Activities
{
    public sealed class MirrorUpdate : MirrorCrud
    {
        protected override string CrudType => "update";
    }
}
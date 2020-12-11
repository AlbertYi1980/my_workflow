using System;
using System.Activities;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace WorkflowCore.Activities
{
    public class MirrorDetail : MirrorCrud
    {
        protected override string CrudType => "detail";
    }
}
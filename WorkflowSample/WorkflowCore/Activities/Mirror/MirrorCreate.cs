using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows.Markup;

namespace WorkflowCore.Activities
{
    public sealed class MirrorCreate : MirrorCrud
    {
        protected override string CrudType => "create";
    }
}
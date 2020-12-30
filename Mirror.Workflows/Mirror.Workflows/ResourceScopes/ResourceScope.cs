namespace Mirror.Workflows.Activities.Scopes
{
    /// <summary>
    /// 使用类型域区分系统，全局，各租户的类型
    /// 类型域内任意类型信息变化，都会重置类型域缓存信息
    /// </summary>
    public struct ResourceScope
    {
        public ResourceScopeType ResourceScopeType { get; set; }
        public string Id { get; set; }
    }
}
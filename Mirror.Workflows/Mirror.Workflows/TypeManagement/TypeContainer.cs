namespace Mirror.Workflows.TypeManagement
{

    public enum TypeScopeType{
        System,
        Global,
        Tenant,
    }
    
    /// <summary>
    /// 使用类型域区分系统，全局，各租户的类型
    /// 类型域内任意类型信息变化，都会重置类型域缓存信息
    /// </summary>
    public struct TypeScope
    {
        public TypeScopeType Type { get; set; }
        public string Id { get; set; }
    }
    
    public class TypeContainer
    {
        
    }

    public interface ISystemTypeProvider
    {
        
    }
}
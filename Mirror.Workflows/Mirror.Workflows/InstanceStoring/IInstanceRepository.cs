using System;

namespace Mirror.Workflows.InstanceStoring
{
    public interface IInstanceRepository
    {
        void Save(Guid id, InstanceDataPackage dataPackage);
        InstanceDataPackage Load(Guid id);
        void Delete(Guid id);
    }
}
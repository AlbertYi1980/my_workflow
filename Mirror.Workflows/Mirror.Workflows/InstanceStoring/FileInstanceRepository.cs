using System;
using System.IO;

namespace Mirror.Workflows.InstanceStoring
{
    class FileInstanceRepository : IInstanceRepository
    {
        private readonly string _baseDir;

        public FileInstanceRepository(string baseDir)
        {
            _baseDir = baseDir;
            Directory.CreateDirectory(baseDir);
        }

        private string GetDataPath(Guid id)
        {
            return $"{_baseDir}\\{id}_Data";
        }

        private string GetMetadataPath(Guid id)
        {
            return $"{_baseDir}\\{id}_Metadata";
        }

        public void Save(Guid id, InstanceDataPackage dataPackage)
        {
            File.WriteAllText(GetDataPath(id), dataPackage.Data);
            File.WriteAllText(GetMetadataPath(id), dataPackage.Metadata);
        }

        public InstanceDataPackage Load(Guid id)
        {
            var data = File.ReadAllText(GetDataPath(id));
            var metadata = File.ReadAllText(GetMetadataPath(id));
            return new InstanceDataPackage(metadata, data);
        }

        public void Delete(Guid id)
        {
            File.Delete(GetDataPath(id));
            File.Delete(GetMetadataPath(id));
        }
    }
}
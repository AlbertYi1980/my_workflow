using System;
using System.Activities.Runtime.DurableInstancing;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Mirror.Workflows.InstanceStoring
{
    public  class StrongTypeJsonFileRepository : IInstanceRepository
    {
        private readonly string _baseDir;
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        };
        public StrongTypeJsonFileRepository(string baseDir)
        {
            _baseDir = baseDir;
            Directory.CreateDirectory(baseDir);
        }

        private string GetDataPath(Guid id)
        {
            return $"{_baseDir}\\{id:N}_Data";
        }

        private string GetMetadataPath(Guid id)
        {
            return $"{_baseDir}\\{id:N}_Metadata";
        }

        public void Save(Guid id, InstanceDataPackage dataPackage)
        {
            File.WriteAllText(GetDataPath(id), Serialize(dataPackage.Data));
            File.WriteAllText(GetMetadataPath(id), Serialize(dataPackage.Metadata));
        }

      

        public InstanceDataPackage Load(Guid id)
        {
            var data = File.ReadAllText(GetDataPath(id));
            var metadata = File.ReadAllText(GetMetadataPath(id));
            return new InstanceDataPackage(Deserialize(metadata), Deserialize( data));
        }

      
        public void Delete(Guid id)
        {
            File.Delete(GetDataPath(id));
            File.Delete(GetMetadataPath(id));
        }
        
        private string Serialize(IDictionary<string, InstanceValue> obj)
        {
            return JsonConvert.SerializeObject( obj, _jsonSerializerSettings);
        }
        private Dictionary<string, InstanceValue> Deserialize(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, InstanceValue>>( json, _jsonSerializerSettings);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

      
    }
}
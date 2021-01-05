using System;
using System.Activities;
using System.Activities.Runtime.DurableInstancing;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Mirror.Workflows.InstanceStoring;


namespace Mirror.Workflows.Repositories
{
    public  class StrongTypeJsonFileRepository : IInstanceRepository
    {
        private readonly string _baseDir;
        private readonly List<Type> _knownTypes = new List<Type>();

        private void InitializeKnownTypes(IEnumerable<Type> knownTypesForDataContractSerializer)
        {


            Assembly sysActivitiesAssembly = typeof(Activity).GetTypeInfo().Assembly;
            Type[] typesArray = sysActivitiesAssembly.GetTypes();

            // Remove types that are not decorated with a DataContract attribute
            foreach (Type t in typesArray)
            {
                TypeInfo typeInfo = t.GetTypeInfo();
                if (typeInfo.GetCustomAttribute<DataContractAttribute>() != null)
                {
                    _knownTypes.Add(t);
                }
            }

            if (knownTypesForDataContractSerializer != null)
            {
                foreach (Type knownType in knownTypesForDataContractSerializer)
                {
                    _knownTypes.Add(knownType);
                }
            }
        }
        
        public StrongTypeJsonFileRepository(string baseDir)
        {
            _baseDir = baseDir;
            Directory.CreateDirectory(baseDir);
            InitializeKnownTypes(null);
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
        
        private string Serialize(Dictionary<string, InstanceValue> obj)
        {
            var settings = new DataContractSerializerSettings
            {
                PreserveObjectReferences = true,
                KnownTypes = _knownTypes
            };
            var serializer = new DataContractSerializer(typeof(Dictionary<string, InstanceValue>), settings);
            using var stream = new MemoryStream();
      
            serializer.WriteObject(stream, obj);
            return Convert.ToBase64String(stream.GetBuffer());
        }
        private Dictionary<string, InstanceValue> Deserialize(string s)
        {
            try
            {  
                
                DataContractSerializerSettings settings = new DataContractSerializerSettings
                {
                    PreserveObjectReferences = true,
                    KnownTypes = _knownTypes
                };
                var serializer = new DataContractSerializer(typeof(Dictionary<string, InstanceValue>), settings);
                using var stream = new MemoryStream(Convert.FromBase64String(s));
                return (Dictionary<string, InstanceValue>) serializer.ReadObject(stream);
            }
            catch (Exception e)
            {
          
                throw;
            }
        }

      
    }
}
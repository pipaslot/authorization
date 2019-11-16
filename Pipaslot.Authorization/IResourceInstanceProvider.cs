using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pipaslot.Authorization
{
    public interface IResourceInstanceProvider
    {
        Task<ICollection<ResourceInstance>> GetAllInstancesAsync();
    }

    public class ResourceInstance
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

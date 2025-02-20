using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Data
{
    public class User
    {
        [DataMember] public int Id { get; set; }
        [DataMember] public string Name { get; set; }
    }
}

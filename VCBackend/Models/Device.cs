using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCBackend.Models
{
    public interface Device
    {
        int Id { get; }
        String DeviceId { get; }
        String AccessToken { get; set; }
        Boolean HasFullAccess();
    }
}

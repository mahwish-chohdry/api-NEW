using System;
using System.Collections.Generic;
using System.Text;
using Xavor.SD.BusinessLayer.Concrete;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer.Interfaces
{
    public interface IEnvironmentstandardsBL
    {

      
        IEnumerable<Environmentstandards> GetEnvironmentstandards();
       
    }
}

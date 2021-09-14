using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer
{
    public interface IPersonaBL
    {
        IEnumerable<Persona> GetPersonas();
        bool DeletePersona(int personaId);
        Persona GetPersona(int Id);
        Persona GetPersonaByName(string personaName);
        IQueryable<Persona> QueryPersona();
        Persona InsertPersona(Persona persona);
        Persona UpdatePersona(Persona persona);
    }
}

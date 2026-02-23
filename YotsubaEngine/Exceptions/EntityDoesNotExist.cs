using System;
using System.Collections.Generic;
using System.Text;

namespace YotsubaEngine.Exceptions
{
    public class EntityDoesNotExist : Exception
    {
        const string ERROR = "LA ENTIDAD QUE BUSCASTE NO EXISTE!!";
        public EntityDoesNotExist(string method, string reference) : base($"{ERROR} El error surgio en el metodo {method} al buscar la entidad por \"{reference}\"")
        {
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YotsubaEngine.Exceptions
{
    /// <summary>
    /// Exception raised when a component is added to the wrong entity index.
    /// Exception que se produce cuando un componente ingresa un índice diferente del id de la entidad que se le asoció. 
    /// (Es un error del GameEngine)
    /// </summary>
    public class AddComponentInDiferentEntityIndexException : Exception
    {
        /// <summary>
        /// Creates the exception with a cause message.
        /// Primer constructor que recibe la causa de la exception
        /// </summary>
        /// <param name="cause">Cause message. Mensaje de causa.</param>
        public AddComponentInDiferentEntityIndexException(string cause) : base(cause) { }

        /// <summary>
        /// Creates the exception with a cause message and inner exception.
        /// Segundo Constructor que recibe la causa y una excepcion como parametros
        /// </summary>
        /// <param name="cause">Cause message. Mensaje de causa.</param>
        /// <param name="ex">Inner exception. Excepción interna.</param>
        public AddComponentInDiferentEntityIndexException(string cause, Exception ex) : base(cause, ex) { }

    }
}

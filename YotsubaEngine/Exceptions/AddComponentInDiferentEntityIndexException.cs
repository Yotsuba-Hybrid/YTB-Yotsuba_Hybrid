using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YotsubaEngine.Exceptions
{
    /// <summary>
    /// Excepción que se produce cuando un componente ingresa un índice diferente del id de la entidad que se le asoció.
    /// <para>Exception raised when a component is added to the wrong entity index.</para>
    /// </summary>
    public class AddComponentInDiferentEntityIndexException : Exception
    {
        /// <summary>
        /// Primer constructor que recibe la causa de la excepción.
        /// <para>Creates the exception with a cause message.</para>
        /// </summary>
        /// <param name="cause">Mensaje de causa. <para>Cause message.</para></param>
        public AddComponentInDiferentEntityIndexException(string cause) : base(cause) { }

        /// <summary>
        /// Segundo constructor que recibe la causa y una excepción como parámetros.
        /// <para>Creates the exception with a cause message and inner exception.</para>
        /// </summary>
        /// <param name="cause">Mensaje de causa. <para>Cause message.</para></param>
        /// <param name="ex">Excepción interna. <para>Inner exception.</para></param>
        public AddComponentInDiferentEntityIndexException(string cause, Exception ex) : base(cause, ex) { }

    }
}

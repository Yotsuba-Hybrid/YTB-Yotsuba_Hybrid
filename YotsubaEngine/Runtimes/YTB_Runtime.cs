using System;
using System.Collections.Generic;
using System.Text;
using YotsubaEngine.Core.System.Contract;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.HighestPerformanceTypes;

namespace YotsubaEngine.Runtime
{
    public abstract class YTB_Runtime : ISystem
    {
        /// <summary>
        /// Entidades con Rigibody
        /// </summary>
        public YTB<int> Entities { get; set; }

        /// <summary>
        /// Entity Manager de la escena actual
        /// </summary>
        private EntityManager CurrentEntityManager { get; set; }

        /// <summary>
        /// Metodo para setear el Entity Manager de la escena actual, necesario para acceder a los componentes de las entidades
        /// </summary>
        /// <param name="entityManager"></param>
        public void SetEntityManager(EntityManager entityManager)
        {

        }
    }
}

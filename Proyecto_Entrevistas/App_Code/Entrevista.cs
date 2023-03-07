using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Entrevistas.App_Code
{
    internal class Entrevista
    {
        private int id, vacante, prospecto;
        private DateTime fecha_entrevista;
        private string notas;
        private bool reclutado;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        public int Vacante
        {
            get { return vacante; }
            set { vacante = value; }
        }
        public int Prospecto
        {
            get { return prospecto; }
            set { prospecto = value; }
        }
        public DateTime Fecha_Entrevista
        {
            get { return fecha_entrevista; }
            set { fecha_entrevista = value; }
        }
        public string Notas
        {
            get { return notas; }
            set { notas = value; }
        }
        public bool Reclutado
        {
            get { return reclutado; }
            set { reclutado = value; }
        }
    }
}

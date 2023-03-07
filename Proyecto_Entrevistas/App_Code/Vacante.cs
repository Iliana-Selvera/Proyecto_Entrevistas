using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Entrevistas.App_Code
{
    internal class Vacante
    {
        private int id;
        private string area;
        private double sueldo;
        private bool activo;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        public string Area
        {
            get { return area; }
            set { area = value; }
        }
        public double Sueldo
        {
            get { return sueldo; }
            set { sueldo = value; }
        }
        public bool Activo
        {
            get { return activo; }
            set { activo = value; }
        }
    }
}

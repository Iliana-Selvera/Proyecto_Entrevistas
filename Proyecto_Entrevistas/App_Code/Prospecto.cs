using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Entrevistas.App_Code
{
    internal class Prospecto
    {
        private int id;
        private string nombre, correo;
        private DateTime fecha_registro;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        public string Nombre  
        {
            get { return nombre; }   
            set { nombre = value; }  
        }
        public string Correo
        {
            get { return correo; }
            set { correo = value; }
        }
        public DateTime Fecha_Registro
        {
            get { return fecha_registro; }
            set { fecha_registro = value; }
        }
    }
}

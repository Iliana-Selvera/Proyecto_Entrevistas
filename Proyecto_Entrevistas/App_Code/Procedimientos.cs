using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Entrevistas.App_Code
{
    static public class Procedimientos
    {
        static public object IdentificarClase(string titulo)
        {
            object obj;
            try
            {
                if (titulo == "Entrevista")
                {
                    obj = new Entrevista();
                }
                else
                {
                    if (titulo == "Vacante")
                    {
                        obj = new Vacante();
                    }
                    else
                    {
                        obj = new Prospecto();
                    }
                }
            }
            catch (Exception x)
            {
                throw x;
            }
            return obj;
        }

        public static bool ConvertirTipo<T>(this object objeto, out T resultado)
        {
            try
            {
                if (objeto == null)
                {
                    resultado = default(T);
                    return true;
                }
                else
                {
                    if (objeto is T)
                    {
                        resultado = (T)objeto;
                        return true;
                    }
                    else
                    {
                        resultado = (T)Convert.ChangeType(objeto, typeof(T));
                        return true;
                    }
                }
            }
            catch (Exception x)
            {
                resultado = default(T);
                return false;
                throw x;
            }

        }
    }
}

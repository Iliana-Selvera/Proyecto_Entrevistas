using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Proyecto_Entrevistas.App_Code;
//using System.Linq;

namespace Proyecto_Entrevistas
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                ConsultarTablas();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void ConsultarTablas()
        {
            try
            {
                Procedimientos_Sql procedimientos_Sql = new Procedimientos_Sql();
                foreach(TabItem tab in TabCTablas.Items)
                {
                    string tabla = tab.Header.ToString();                    
                    if(tabla=="Entrevista")
                    {
                        GridEntrevistas.ItemsSource = JsonConvert.DeserializeObject<List<Entrevista>>(procedimientos_Sql.ConsultarTabla(tabla));
                    }
                    else
                    {
                        if (tabla == "Vacante")
                        {
                            GridVacantes.ItemsSource = JsonConvert.DeserializeObject<List<Vacante>>(procedimientos_Sql.ConsultarTabla(tabla));
                        }
                        else
                        {
                            GridProspectos.ItemsSource = JsonConvert.DeserializeObject<List<Prospecto>>(procedimientos_Sql.ConsultarTabla(tabla));
                        }
                    }                    
                }
                procedimientos_Sql = null;
            }
            catch(Exception x)
            {
                throw x;
            }
        }

        private string BuscarTitulo()
        {
            string titulo = string.Empty;
            try
            {
                TabItem tabI = TabCTablas.SelectedItem as TabItem;
                if (tabI != null)
                {
                    titulo = tabI.Header.ToString();
                }
                else
                {
                    throw new Exception("Por favor seleccione una de las pestañas disponibles.");
                }
                tabI = null;
            }
            catch (Exception x)
            {
                throw x;
            }
            return titulo;
        }

        private void AgregarCampos()
        {
            try
            {
                RemoverControles();
                foreach (var propiedad in Procedimientos.IdentificarClase(BuscarTitulo()).GetType().GetProperties())
                {
                    AgregarControl(propiedad.Name, propiedad.PropertyType.Name);
                }
                AgregarControl("Guardar", "Event");

            }
            catch (Exception x)
            {
                throw x;
            }
        }

        private void AgregarControl(string nombreCampo, string tipoDato)
        {
            try
            {
                if (tipoDato != "Event")
                {
                    TextBlock nombreCampoControl = new TextBlock();
                    nombreCampoControl.Text = nombreCampo.Replace("_", " ");
                    nombreCampoControl.Margin = new Thickness(5, 5, 5, 5);
                    StackPCampos.Children.Add(nombreCampoControl);
                    nombreCampoControl = null;
                }
                switch (tipoDato)
                {
                    case "Int32":
                    case "String":
                    case "Double":
                        Procedimientos_Sql procedimientos_Sql = new Procedimientos_Sql();
                        TextBox textoCampoControl = new TextBox();
                        textoCampoControl.Name = "Textbox" + nombreCampo;
                        textoCampoControl.Uid = textoCampoControl.Name;
                        textoCampoControl.Margin = new Thickness(5, 5, 5, 5);
                        if(nombreCampo == "Id")
                        {
                            textoCampoControl.Text = Convert.ToString(procedimientos_Sql.ConsultarSiguienteID(BuscarTitulo()));
                            textoCampoControl.IsEnabled= false;
                        }
                        if(nombreCampo == "Prospecto" || nombreCampo == "Vacante")
                        {                            
                            AutoCompleteBox autoCompleteBox = new AutoCompleteBox();
                            autoCompleteBox.Name = "Autocompletebox" + nombreCampo;
                            autoCompleteBox.Uid = autoCompleteBox.Name;
                            autoCompleteBox.FilterMode = AutoCompleteFilterMode.Contains;
                            autoCompleteBox.ItemsSource = CrearSource(nombreCampo);
                            textoCampoControl.Visibility = Visibility.Hidden;                            
                            Binding binding = new Binding("Text");
                            binding.Source = autoCompleteBox;
                            binding.Mode = BindingMode.TwoWay;
                            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                            textoCampoControl.SetBinding(TextBox.TextProperty, binding);
                            binding = null;
                            StackPCampos.Children.Add(autoCompleteBox);
                            autoCompleteBox = null;
                        }
                        StackPCampos.Children.Add(textoCampoControl);
                        textoCampoControl = null;
                        procedimientos_Sql = null;
                        break;
                    case "DateTime":
                        DatePicker fechaCampoControl = new DatePicker();
                        fechaCampoControl.Name = "Datepicker" + nombreCampo;
                        fechaCampoControl.Uid = fechaCampoControl.Name;
                        fechaCampoControl.Margin = new Thickness(5, 5, 5, 5);
                        StackPCampos.Children.Add(fechaCampoControl);
                        fechaCampoControl = null;
                        break;
                    case "Boolean":
                        CheckBox boolCampoControl = new CheckBox();
                        boolCampoControl.Name = "Checkbox" + nombreCampo;
                        boolCampoControl.Uid = boolCampoControl.Name;
                        boolCampoControl.Margin = new Thickness(5, 5, 5, 5);
                        StackPCampos.Children.Add(boolCampoControl);
                        boolCampoControl = null;
                        break;
                    case "Event":
                        Button botonCampoControl = new Button();
                        botonCampoControl.Name = "Button" + nombreCampo;
                        botonCampoControl.Uid = botonCampoControl.Name;
                        botonCampoControl.Content = nombreCampo;
                        if (nombreCampo == "Guardar") { botonCampoControl.Click += ButtonGuardar_Click; }
                        botonCampoControl.Margin = new Thickness(5, 5, 5, 5);
                        StackPCampos.Children.Add(botonCampoControl);
                        break;
                }
            }
            catch (Exception x)
            {
                throw x;
            }
        }

        private void RemoverControles()
        {
            try
            {
                while (StackPCampos.Children.Count > 0)
                {
                    StackPCampos.Children.RemoveAt(StackPCampos.Children.Count - 1);
                }
            }
            catch (Exception x)
            {
                throw x;
            }
        }

        private void ButtonGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ValidarInformacion(Procedimientos.IdentificarClase(BuscarTitulo()));
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ValidarInformacion(object objetoClase)
        {
            try
            {
                string mensajeError = string.Empty;
                foreach (var propiedad in objetoClase.GetType().GetProperties())
                {
                    UIElement control = BuscarControl(propiedad.Name, propiedad.PropertyType.Name);
                    if (control != null)
                    {
                        object valor = null;
                        switch (propiedad.PropertyType.Name)
                        {
                            case "Int32":
                                int valorEntero;
                                string valorTexto = ((TextBox)control).Text.ToString();
                                if (!(((TextBox)control).IsVisible))
                                { if (valorTexto.Contains("|") && valorTexto.IndexOf('|')>0) { valorTexto = valorTexto.Substring(0, valorTexto.IndexOf('|') - 1).Trim(); } }
                                if (!(Procedimientos.ConvertirTipo(valorTexto, out valorEntero)))
                                {
                                    mensajeError += "\nEl valor necesita ser una cantidad numerica, sin letras o puntos decimales. Campo: " + propiedad.Name;
                                }
                                valor = valorEntero;
                                break;
                            case "String":
                                valor = string.Empty;
                                if ((!(Procedimientos.ConvertirTipo(((TextBox)control).Text.ToString(), out valor)))|| ((TextBox)control).Text.ToString() == string.Empty)
                                {
                                    mensajeError += "\nEl valor necesita ser texto. Campo: " + propiedad.Name;
                                }
                                break;
                            case "Double":
                                double valorDoble;
                                if (!(Procedimientos.ConvertirTipo(((TextBox)control).Text.ToString(), out valorDoble)))
                                {
                                    mensajeError += "\nEl valor necesita ser una cantidad numerica. Campo: " + propiedad.Name;
                                }
                                valor = valorDoble;
                                break;
                            case "DateTime":
                                DateTime valorFecha;
                                if (!(Procedimientos.ConvertirTipo(((DatePicker)control).SelectedDate, out valorFecha)))
                                {
                                    mensajeError += "\nFecha no valida. Campo: " + propiedad.Name;
                                }
                                valor = valorFecha;
                                break;
                            case "Boolean":                                
                                valor = ((CheckBox)control).IsChecked;
                                break;
                        }
                        objetoClase.GetType().GetProperty(propiedad.Name).SetValue(objetoClase, valor);
                    }
                    control = null;
                }
                if (mensajeError == string.Empty)
                {
                    //guardar
                    Procedimientos_Sql procedimientos_Sql = new Procedimientos_Sql();
                    if(procedimientos_Sql.AgregarRegistroSQL(objetoClase))
                    {
                        MessageBox.Show("Registro guardado correctamente.", "Guardar", MessageBoxButton.OK, MessageBoxImage.Information);
                        ConsultarTablas();
                    }
                    else
                    {
                        throw new Exception("El registro no se guardo correctamente. Por favor intente de nuevo.");
                    }
                    procedimientos_Sql = null;
                }
                else
                {
                    throw new Exception("Por favor valide la siguiente informacion." + mensajeError);
                }
            }
            catch (Exception x)
            {
                throw x;
            }
        }

        private UIElement BuscarControl(string nombreControl, string tipoDato)
        {
            UIElement control;
            try
            {
                string comando = string.Empty;
                switch (tipoDato)
                {
                    case "Int32":
                    case "String":
                    case "Double":
                        comando = "Textbox";
                        break;
                    case "DateTime":
                        comando = "Datepicker";
                        break;
                    case "Boolean":
                        comando = "Checkbox";
                        break;
                    case "Event":
                        comando = "Button";
                        break;
                }
                control = StackPCampos.Children.Cast<UIElement>().Where(x => x.Uid == (comando + nombreControl)).FirstOrDefault();
            }
            catch (Exception x)
            {
                throw x;
            }
            return control;
        }

        private void TabCTablas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                AgregarCampos();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonMostrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                object row = ((Button)e.Source).DataContext;
                foreach (var propiedad in row.GetType().GetProperties())
                {
                    UIElement control = BuscarControl(propiedad.Name, propiedad.PropertyType.Name);
                    if (control != null)
                    {                        
                        switch (propiedad.PropertyType.Name)
                        {
                            case "Int32":   
                            case "String":
                            case "Double":
                                ((TextBox)control).Text = Convert.ToString(propiedad.GetValue(row));
                                break;
                            case "DateTime":
                                ((DatePicker)control).SelectedDate = Convert.ToDateTime(propiedad.GetValue(row));
                                break;
                            case "Boolean":
                                ((CheckBox)control).IsChecked = Convert.ToBoolean(propiedad.GetValue(row));
                                break;
                        }                        
                    }
                    control = null;
                }
                row = null;
                    
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                object row = ((Button)e.Source).DataContext;
                string mensaje = string.Empty;
                foreach (var propiedad in row.GetType().GetProperties())
                {
                    if (mensaje.Length > 0) { mensaje += "\n"; }
                    mensaje += propiedad.Name + ": " + propiedad.GetValue(row);
                }
                if(MessageBox.Show("Esta seguro que desea eliminar el siguiente registro?:\n"+mensaje, "Eliminar", MessageBoxButton.YesNo, MessageBoxImage.Warning)==MessageBoxResult.Yes)
                {
                    //eliminar
                    Procedimientos_Sql procedimientos_Sql = new Procedimientos_Sql();
                    if(procedimientos_Sql.EliminarRegistro(row))
                    {

                        MessageBox.Show("Registro eliminado correctamente.", "Eliminar", MessageBoxButton.OK, MessageBoxImage.Information);
                        ConsultarTablas();
                    }
                    else
                    {
                        throw new Exception("El registro no se elimino correctamente. Por favor intente de nuevo.");
                    }
                    procedimientos_Sql = null;
                }
                row = null;
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private List<string> CrearSource(string nombreCampo)
        {
            List<string> listaAutoComplete = new List<string>();
            try
            {
                if(nombreCampo=="Vacante")
                {
                    List<Vacante> listaVacantes = (List<Vacante>)GridVacantes.ItemsSource;
                    foreach (Vacante renglon in listaVacantes)
                    {
                        listaAutoComplete.Add(Convert.ToString(renglon.Id)+" | "+renglon.Area);
                    }
                    listaVacantes = null;
                }
                else
                {
                    if (nombreCampo == "Prospecto")
                    {
                        List<Prospecto> listaProspecto = (List<Prospecto>)GridProspectos.ItemsSource;
                        foreach (Prospecto renglon in listaProspecto)
                        {
                            listaAutoComplete.Add(Convert.ToString(renglon.Id) + " | " + renglon.Nombre);
                        }
                        listaProspecto = null;
                    }                        
                }
            }
            catch(Exception x)
            {
                throw x;
            }
            return listaAutoComplete;
        }
    }
}

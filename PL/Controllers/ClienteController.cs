using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.ComponentModel;
using System.Reflection;


namespace PL.Controllers
{
    public class ClienteController : Controller
    {
        //
        // GET: /Cliente/
        public ActionResult GetAll()
        {
            //ML.Categoria categoria = new ML.Categoria();

            ML.Result resultCliente = new ML.Result();
            resultCliente.Objects = new List<Object>();
            ML.Cliente cliente = new ML.Cliente();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:50817/api/");

                var responseTask = client.GetAsync("Cliente");
                responseTask.Wait();

                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<ML.Result>();
                    readTask.Wait();

                    foreach (var resultItem in readTask.Result.Objects)
                    {
                        ML.Cliente resultItemList = Newtonsoft.Json.JsonConvert.DeserializeObject<ML.Cliente>(resultItem.ToString());
                        resultCliente.Objects.Add(resultItemList);
                    }
                }

            }
            cliente.Clientes = new List<Object>();
            cliente.Clientes = resultCliente.Objects;
            return View(cliente);
        }
        [HttpGet]
        public ActionResult Form(int? IdCliente)
        {
            ML.Cliente cliente = new ML.Cliente();

            if (IdCliente == null)
            {
                ViewBag.Titulo = "Registrar Cliente";
                ViewBag.Accion = "Guardar";

                ML.Result result = new ML.Result();
                result = BL.Cliente.GetAll();

                return View(cliente);
            }
            else
            {
                ViewBag.Titulo = "Actualizar Cliente";
                ViewBag.Accion = "Actualizar";
                cliente.IdCliente = IdCliente.Value;


                var result = BL.Cliente.GetById(cliente);

                if (result.Object != null)
                {
                    ML.Cliente Cliente = new ML.Cliente();
                    Cliente.IdCliente = ((ML.Cliente)result.Object).IdCliente;
                    Cliente.Rfc = ((ML.Cliente)result.Object).Rfc;
                    Cliente.NumeroEmpleado = ((ML.Cliente)result.Object).NumeroEmpleado;
                    Cliente.Nombre = ((ML.Cliente)result.Object).Nombre;
                    Cliente.FechaDeControl = ((ML.Cliente)result.Object).FechaDeControl;
                    Cliente.Salario = ((ML.Cliente)result.Object).Salario;


                    return View(Cliente);
                }
                else
                {
                    ViewBag.Message = result.ErrorMessage;
                    return PartialView("ValidationModal");
                }
            }
        }

        [HttpPost]
        public ActionResult Form(ML.Cliente cliente)
        {


            if (cliente.IdCliente == 0)
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:50817/api/cliente");

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<ML.Cliente>("cliente/Add", cliente);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("GetAll");
                    }
                }
                return View("GetAll");

            }
            else
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:50817/api/cliente");

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<ML.Cliente>("cliente/Update", cliente);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("GetAll");
                    }
                }

                return View("GetAll");

            }
        }


        [HttpGet]
        public ActionResult Delete(int IdCliente)
        {
            ML.Cliente cliente = new ML.Cliente();
            ML.Result resultListProduct = new ML.Result();
            cliente.IdCliente = IdCliente;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:50817/api/cliente");

                //HTTP POST
                var postTask = client.GetAsync("cliente/Delete/" + cliente.IdCliente);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    resultListProduct = BL.Cliente.GetAll();
                    return RedirectToAction("GetAll", resultListProduct);
                }
            }


            resultListProduct = BL.Cliente.GetAll();

            return View("GetAll", resultListProduct);

        }
        [HttpPost]
        public ActionResult Update(HttpPostedFileBase archivoTXT)
        {
            ML.Result result = new ML.Result();
            ML.Cliente cliente = new ML.Cliente();
            DataTable DtErrores = new DataTable();
            ML.Result ErrorresResult = new ML.Result();
            ErrorresResult.Objects = new List<object>();
            result.Objects = new List<object>();
            int NumeroErrores = 0;

            int NumeroRegistro = 0;

            cliente.Clientes = new List<object>();
            cliente.Errores = new List<object>();
            try
            {

                string resultado = new StreamReader(archivoTXT.InputStream).ReadToEnd();

                string[] rows = resultado.Split('\n');



                cliente.Errores = new List<object>();
                foreach (string s in rows)
                {

                    if (s != "")
                    {
                        NumeroRegistro++;
                        ML.Cliente cliente1 = new ML.Cliente();

                        string[] columns = s.Split('|');
                        BL.Cliente.ValidarDatos(columns, result);

                        if (result.Correct == true)
                        {
                            cliente1.Rfc = columns[0].ToString();
                            cliente1.NumeroEmpleado = int.Parse(columns[1].ToString());
                            cliente1.Nombre = columns[2].ToString();
                            cliente1.FechaDeControl = DateTime.ParseExact(columns[3].ToString(), "dd/MM/yyyy HH:mm", null);
                            cliente1.Salario = decimal.Parse(columns[4].ToString());

                            cliente.Clientes.Add(cliente1);
                        }

                        else
                        {
                            NumeroErrores++;
                            cliente1.NumeroDeRegistro = NumeroRegistro;
                            cliente1.DetalleError = result.ErrorMessage;
                            cliente.Errores.Add(cliente1);





                        }

                    }
                    else
                    {
                        break;
                    }
                }

                if (NumeroErrores >= 1 || cliente.NumeroDeRegistro == 1)
                {

                    NumeroErrores++;
                    foreach (var obj in cliente.Errores)
                    {


                        return View("Update", cliente);
                    }

                }
                else
                {
                    foreach (var obj in cliente.Clientes)
                    {

                        BL.Cliente.Add((ML.Cliente)obj);

                    }
                    return RedirectToAction("GetAll");
                }
            }




            catch (Exception ex)
            {
                ViewBag.mensaje = "Se produjo un error : " + ex.Message;
            }

            return RedirectToAction("GetAll", "Cliente");
        }
        public FileResult DowloandTxt(int IdCliente)
        {
            ML.Cliente cliente = new ML.Cliente();
            ML.Result result = new ML.Result();

            cliente.IdCliente = IdCliente;
            var Result = BL.Cliente.GetById(cliente);

            if (Result.Object != null)
            {

                cliente.IdCliente = ((ML.Cliente)Result.Object).IdCliente;
                cliente.Rfc = ((ML.Cliente)Result.Object).Rfc;
                cliente.NumeroEmpleado = ((ML.Cliente)Result.Object).NumeroEmpleado;
                cliente.Nombre = ((ML.Cliente)Result.Object).Nombre;
                cliente.FechaDeControl = ((ML.Cliente)Result.Object).FechaDeControl;
                cliente.Salario = ((ML.Cliente)Result.Object).Salario;
            }
            string info = cliente.IdCliente + '|' + cliente.Rfc + '|' + cliente.NumeroEmpleado + '|' + cliente.Nombre + '|' + cliente.FechaDeControl + '|' + +cliente.Salario;
            var ByteArray = Encoding.ASCII.GetBytes(Convert.ToString(info));
            var stream = new MemoryStream(ByteArray);

            return File(stream, "text/plain", "cliente.txt");
        }

        public FileResult DowloandXml(int IdCliente)
        {
            ML.Cliente cliente = new ML.Cliente();
            ML.Result result = new ML.Result();


            cliente.IdCliente = IdCliente;
            var Result = BL.Cliente.GetById(cliente);

            if (Result.Object != null)
            {

                cliente.IdCliente = ((ML.Cliente)Result.Object).IdCliente;
                cliente.Rfc = ((ML.Cliente)Result.Object).Rfc;
                cliente.NumeroEmpleado = ((ML.Cliente)Result.Object).NumeroEmpleado;
                cliente.Nombre = ((ML.Cliente)Result.Object).Nombre;
                cliente.FechaDeControl = ((ML.Cliente)Result.Object).FechaDeControl;
                cliente.Salario = ((ML.Cliente)Result.Object).Salario;
            }
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(cliente.GetType());
            x.Serialize(Console.Out, cliente);
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode = xmlDoc.CreateElement("Registros");
            xmlDoc.AppendChild(rootNode);

            XmlNode RegistroNode = xmlDoc.CreateElement("Registro");
            XmlAttribute attribute = xmlDoc.CreateAttribute("IdCliente");
            attribute.Value = cliente.IdCliente.ToString();
            RegistroNode.Attributes.Append(attribute);

            attribute = xmlDoc.CreateAttribute("RFC");
            attribute.Value = cliente.Rfc;
            RegistroNode.Attributes.Append(attribute);

            attribute = xmlDoc.CreateAttribute("NumeroDeEmpleado");
            attribute.Value = cliente.NumeroEmpleado.ToString();
            RegistroNode.Attributes.Append(attribute);

            attribute = xmlDoc.CreateAttribute("Nombre");
            attribute.Value = cliente.Nombre;
            RegistroNode.Attributes.Append(attribute);

            attribute = xmlDoc.CreateAttribute("FechaDeControl");
            attribute.Value = cliente.FechaDeControl.ToString();
            RegistroNode.Attributes.Append(attribute);

            attribute = xmlDoc.CreateAttribute("Salario");
            attribute.Value = cliente.Salario.ToString();
            RegistroNode.Attributes.Append(attribute);

            rootNode.AppendChild(RegistroNode);

            string dirPath = Server.MapPath("~/XML/");
            if (!System.IO.Directory.Exists(dirPath))
            {
                System.IO.Directory.CreateDirectory(dirPath);
            }
            string physicalPath = Server.MapPath("~/XMl/");
            xmlDoc.Save(physicalPath + "CreateXML.xml");

            string Path = Server.MapPath("~/XML/") + "CreateXMl.xml";
            byte[] FileBytes = System.IO.File.ReadAllBytes(Path);
            string FileName = "createXML.xml";
            return File(Path, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
        }

    }
}
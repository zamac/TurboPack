using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PL_API.Controllers
{
    public class ClienteController : ApiController
    {
        // GET api/cliente
        // GET api/subcategoria
        [HttpGet]
        [Route("api/cliente")]
        public IHttpActionResult GetAll()
        {
            ML.Result result = BL.Cliente.GetAll();
            if (result.Correct)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }

        }
        [HttpGet]
        [Route("api/cliente/{IdCliente}")]
        // GET api/subcategoria/5
        public IHttpActionResult GetById(int IdCliente)
        {
            ML.Cliente cliente = new ML.Cliente();


            cliente.IdCliente = IdCliente;
            ML.Result result = BL.Cliente.GetById(cliente);
            if (result.Correct)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }


        [HttpPost]
        [Route("api/cliente/Add")]
        // POST api/subcategoria
        public IHttpActionResult Post([FromBody]ML.Cliente cliente)
        {

            ML.Result result = BL.Cliente.Add(cliente);
            if (result.Correct)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("api/cliente/Update")]
        // PUT api/subcategoria/5
        public IHttpActionResult Put([FromBody]ML.Cliente cliente)
        {
            var result = BL.Cliente.Update(cliente);

            if (result.Correct)
            {
                return Ok(result);
            }
            else //Error
            {
                return Content(HttpStatusCode.NotFound, result);
            }
        }



        [HttpGet]
        [Route("api/cliente/Delete/{IdCliente}")]
        // DELETE api/subcategoria/5
        public IHttpActionResult Delete(int IdCliente)
        {
            ML.Cliente cliente = new ML.Cliente();
            cliente.IdCliente = IdCliente;
            ML.Result result = BL.Cliente.Delete(cliente);

            if (result.Correct)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
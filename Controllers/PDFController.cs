using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using PDF.Data;
using PDF.Models;
using System.Data;

namespace PDF.Controllers
{
    public class PDFController : Controller
    {
        private readonly Contexto _contexo;

        public PDFController(Contexto contexto)
        {
            _contexo = contexto;
        }
        public IActionResult Index()
        {
            var pdffiles = new List<PdfFile>();
            using (SqlConnection con = new(_contexo.Conexion))
            {
                con.Open();
                using (SqlCommand cmd = new("sp_obtener_pdfs", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        pdffiles.Add(new PdfFile
                        {
                            Id = (int)reader["Id"],
                            Nombre = reader["Nombre"].ToString(),
                            Archivo = reader["Archivo"].ToString()
                        });
                    }
                }
            }
            return View(pdffiles);
        }
        public IActionResult Crear()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Crear(IFormFile file)
        {
            using (var memorystream = new MemoryStream())
            {
                file.CopyTo(memorystream);
                var fileBytes = memorystream.ToArray();
                var fileData = Convert.ToBase64String(fileBytes);

                using (SqlConnection con = new(_contexo.Conexion))
                {
                    con.Open();
                    using (SqlCommand cmd = new("sp_insertar_pdf", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Nombre", SqlDbType.VarChar).Value = file.FileName;
                        cmd.Parameters.Add("@Archivo", SqlDbType.VarChar).Value = fileData;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return RedirectToAction("Index");
        }

        public PdfFile? ObtenerPdf(int id)
        {
            using (SqlConnection con = new(_contexo.Conexion))
            {
                con.Open();
                using (SqlCommand cmd = new("sp_buscar_pdf", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        var fileName = reader.GetString(1);
                        var fileData = reader.GetString(2);

                        PdfFile pdf = new();
                        pdf.Id = id;
                        pdf.Nombre = fileName;
                        pdf.Archivo = fileData;
                        return pdf;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        [HttpGet]
        public IActionResult Descargar(int id)
        {
            PdfFile? pdf = ObtenerPdf(id);
            if (pdf != null && pdf.Archivo != null)
            {
                var fileBytes = Convert.FromBase64String(pdf.Archivo);
                var memorystream = new MemoryStream(fileBytes);
                return File(memorystream, "application/pdf", pdf.Nombre);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
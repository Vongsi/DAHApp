using DAHApp.Contexts;
using DAHApp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;

namespace DAHApp.Controllers.v2
{
    [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},ApiKey")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    public class SecuredController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public SecuredController(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> GetSecuredData()
        {
            return Ok("V2 - This Secured Data is available only for Authenticated Users.");
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PostSecuredData()
        {
            return Ok("V2 - This Secured Data is available only for Administrator - Authenticated Users.");
        }

        [HttpPost("upload-file")]
        //[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UploadFile(IFormFile formFile, string code)
        {
            string productCode = "ABX";
            string pathDir = _webHostEnvironment.WebRootPath + "\\Upload\\Product\\" + productCode;

            if (string.IsNullOrEmpty(pathDir))
            {
                return BadRequest();
            }

            if (!System.IO.Directory.Exists(pathDir))
            {
                System.IO.Directory.CreateDirectory(pathDir);
            }

            //https://stackoverflow.com/questions/25919387/converting-file-into-base64string-and-back-again 
            string pathFile = pathDir + "\\" + formFile.FileName.ToUpper();
            //string fileExtension = Path.GetExtension(pathFile);
            //byte[] fileContent = System.IO.File.ReadAllBytes(pathFile);
            //string fileString = Convert.ToBase64String(fileContent);

            if (System.IO.File.Exists(pathFile))
            {
                System.IO.File.Delete(pathFile);
            }

            using (FileStream fileStream = System.IO.File.Create(pathFile))
            {
                await formFile.CopyToAsync(fileStream);
            }

            return Ok(formFile.FileName);
        }

        [HttpPost("upload-files")]
        //[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UploadFiles(IFormFileCollection formFiles, string code)
        {
            string productCode = "ABX";
            string pathDir = _webHostEnvironment.WebRootPath + "\\Upload\\Product\\" + productCode;

            if (string.IsNullOrEmpty(pathDir))
            {
                return BadRequest();
            }

            if (!System.IO.Directory.Exists(pathDir))
            {
                System.IO.Directory.CreateDirectory(pathDir);
            }

            foreach(var formFile in  formFiles)
            {
                string pathFile = pathDir + "\\" + formFile.FileName.ToUpper();
                //string fileExtension = Path.GetExtension(pathFile);
                //byte[] fileContent = System.IO.File.ReadAllBytes(pathFile);
                //string fileString = Convert.ToBase64String(fileContent);

                if (System.IO.File.Exists(pathFile))
                {
                    System.IO.File.Delete(pathFile);
                }

                //using (FileStream fileStream = System.IO.File.Create(pathFile))
                //{
                //    await formFile.CopyToAsync(fileStream);
                //}

                DocumentModel doc = new DocumentModel();
                string extension = Path.GetExtension(pathFile);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await formFile.CopyToAsync(memoryStream);

                    doc.Name = formFile.FileName;
                    doc.Content = memoryStream.ToArray();
                    doc.Type = extension;
                }

                _context.DocumentModel.Add(doc);
                await _context.SaveChangesAsync();
            }

            return Ok("Total " + formFiles.Count.ToString() + " file(s) uploaded");
        }

        [HttpGet("download-files")]
        //[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetFiles(string code)
        //public async Task<Image> GetFiles(string code)             
        {
            var files = await _context.DocumentModel.ToListAsync<DocumentModel>();
            List<string> stringFiles = new List<string>();

            foreach (var file in files)
            {
                var fileContent = Convert.ToBase64String(file.Content);

                stringFiles.Add(fileContent);
            }

            //return Ok(stringFiles);
            //Image img;
            //using (MemoryStream memstr = new MemoryStream(files[0].Content))
            //{
            //    img = Image.FromStream(memstr);
            //    return img;
            //}
            //return img;

            return Ok(stringFiles);
            //return File(files[0].Content, "image/jpeg", files[0].Name);
        }

        [HttpGet("download-file")]
        //[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetFile(string id)
        //public async Task<Image> GetFiles(string code)             
        {
            var file = await _context.DocumentModel.FindAsync(Guid.Parse(id));
            List<string> stringFiles = new List<string>();

            var extension = file?.Type == ".JPG" ? "image/jpeg"
    : file?.Type == ".JPEG" ? "image/jpeg"
    : file?.Type == ".PNG" ? "image/png"
    : file?.Type == ".PDF" ? "application/pdf"
    : file?.Type == ".DOC" ? "application/msword"
    //: file?.Type == ".DOCX" ? "application/msword" : "application/octet-stream";
    : file?.Type == ".DOCX" ? "application/vnd.openxmlformats-officedocument.wordprocessingml.document" : "application/octet-stream";



            return File(file?.Content, extension, file?.Name);
        }
    }
}

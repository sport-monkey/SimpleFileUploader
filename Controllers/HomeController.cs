using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace SimpleFileUploader.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _destinationFolder;
        public HomeController(IConfiguration configuration)
        {
            _destinationFolder = configuration["destinationFolder"];
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            return View(false);
        }
        
        [HttpPost]
        public async Task<IActionResult> Index(List<IFormFile> files)
        {
            foreach (var formFile in files)
            {
                if (formFile.Length <= 0)
                {
                    continue;
                }

                var path = GenerateFilePath(formFile);

                await using var file = System.IO.File.Create(path);
                await formFile.CopyToAsync(file);
            }
            
            return View(true);
        }

        private string GenerateFilePath(IFormFile formFile)
        {
            var fileName = formFile.FileName;
            if (fileName.Length > 8)
            {
                fileName = fileName.Substring(fileName.Length - 8);
            }

            var path = Path.Combine(_destinationFolder, $"{Guid.NewGuid()}_{fileName}");
            return path;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Application.ViewModels;

namespace WebApplication1.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductsService _prodService;
        private readonly IWebHostEnvironment _host;
        public ProductsController(IProductsService prodService, IWebHostEnvironment host)
        {
            _host = host;
            _prodService = prodService;
        }

        public IActionResult Index()
        {
            var list = _prodService.GetProducts();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        { return View(); }

        [HttpPost][ValidateAntiForgeryToken]
        public IActionResult Create(IFormFile file, ProductViewModel data)
        {
           
            data.Description = HtmlEncoder.Default.Encode(data.Description);
     

            if (ModelState.IsValid)
            {

                string uniqueFilename;
                if (System.IO.Path.GetExtension(file.FileName) == ".jpg")
                {
                    //FF D8 >>>>> 255 216

                    byte[] whitelist = new byte[] { 255, 216 };

                    if (file != null)
                    {
                        using (var f = file.OpenReadStream())
                        {
                            /*int byte1 = f.ReadByte();
                            int byte2 = f.ReadByte();
                            */
                            byte[] buffer = new byte[2];  //how to read an x amount of bytes at 1 go
                            f.Read(buffer, 0, 2); //offset - how many bytes you would lke the pointer to skip

                            for (int i = 0; i < whitelist.Length; i++)
                            {
                                if (whitelist[i] == buffer[i])
                                {

                                }
                                else
                                {
                                    //the file is not acceptable
                                    ModelState.AddModelError("file", "File is not valid and acceptable");
                                    return View();
                                }

                            }
                            //...other reading of bytes happening
                            f.Position = 0;

                            //uploading the file
                            //correctness
                            uniqueFilename = Guid.NewGuid() + Path.GetExtension(file.FileName);
                            data.ImageUrl = uniqueFilename;

                            string absolutePath = _host.WebRootPath + @"\pictures\" + uniqueFilename;

                            using (FileStream fsOut = new FileStream(absolutePath, FileMode.CreateNew, FileAccess.Write))
                            {
                                f.CopyTo(fsOut);
                            }

                            f.Close();
                        }
                    }
                }

                if (data.Category.Id < 1 || data.Category.Id > 4 )
                {
                    ModelState.AddModelError("Category.Id", "Category is not valid");
                    return View(data);
                }

                //once the product has been inserted successfully in the db

                _prodService.AddProduct(data);


                TempData["message"] = "Product inserted successfully";
                return View();
            }
            else
            {
                ModelState.AddModelError("", "Check your input. Operation failed");
                return View(data);
            }
            
        
        }
    }
}

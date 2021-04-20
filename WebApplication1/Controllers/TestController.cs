using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Utility;

namespace WebApplication1.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {

            string cipher = Encryption.SymmetricEncrypt("this is a text which i would like to encrypt");

            string originalString = Encryption.SymmetricDecrypt(cipher);


            return View();
        }
    }
}

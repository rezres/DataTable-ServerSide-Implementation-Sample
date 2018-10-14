using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DataTable_ServerSide__Implementation_Sample.Controllers
{
    public class BaseController : Controller
    {

        protected IActionResult Error(string Message = "We are sorry, an error occurred while updating product info, our admin is notified and he will be working on solving this issue soon") {

            ViewBag.errorMsg = Message;
            return PartialView("_Error");
        }
        protected IActionResult Successful(string Message = "Updated Successfully")
        {
            ViewBag.successMsg = Message;
            return PartialView("_Success");
        }
    }
}
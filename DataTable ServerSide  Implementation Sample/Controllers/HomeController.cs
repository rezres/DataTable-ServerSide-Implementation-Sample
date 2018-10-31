using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTable_ServerSide__Implementation_Sample.Data.Model;
using DataTable_ServerSide__Implementation_Sample.Data.Requests;
using DataTable_ServerSide__Implementation_Sample.Interfaces;
using DataTable_ServerSide__Implementation_Sample.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace DataTable_ServerSide__Implementation_Sample.Controllers
{
    public class HomeController : BaseController
    {
        protected IRepo<Product> ProductRepo;
        protected IRepo<Category> CategoryRepo;
        protected ISpecification<Product> Specification;
        protected readonly ILogger<HomeController> ILogger;

        public HomeController(IRepo<Product> _repo, IRepo<Category> _CategoryRepo, ILogger<HomeController> _logger, ISpecification<Product> specification)
        {
            this.ProductRepo = _repo;
            this.CategoryRepo = _CategoryRepo;
            this.ILogger = _logger;
            this.Specification = specification;
        }

        public IActionResult Index()
        {
            return View();
        }
        // GET: Home/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                Specification.AddFilter(i => i.Id == id);
                Specification.AddInclude(e => e.MainCategory);
                Specification.AddInclude(z => z.SubCategory);
                var objList = await ProductRepo.GetFirstBySpecsAsync(Specification);
                return PartialView(objList);
            }
            catch (Exception e)
            {
                this.ILogger.LogError("Error Occurred While Running Details @ HomeController : \n" + e.Message);
                return BadRequest();
            }
        }

        public async Task<IActionResult> GetDTResponseAsync(DataTableOptions options)
        {
            try
            {
                Specification.AddInclude(e => e.MainCategory);
                Specification.AddInclude(e => e.SubCategory);

                return Ok(await ProductRepo.GetOptionResponseWithSpec(options, Specification));
            }
            catch (Exception e)
            {
                this.ILogger.LogError("Error Occurred While Running GetOptions @ HomeController : \n" + e.Message);
                return BadRequest();
            }
        }

        // GET: Home/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var categories = await CategoryRepo.GetAll();
                ViewBag.ddlCategories = new SelectList(categories, "ID", "Name");
                return PartialView();
            }
            catch (Exception e)
            {
                this.ILogger.LogError("Error Occurred While Running GetOptions @ HomeController : \n" + e.Message);
                return Error();

            }
        }

        // POST: Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product data)
        {
            try
            {
                var x = await ProductRepo.Add(data);
                return Successful("Product Added successfully");
            }
            catch (Exception e)
            {
                ILogger.LogError("Error Home\\Create \n Message : " + e.Message);
                return Error();
            }
        }

        //// GET: Home/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                Specification.AddInclude("MainCategory");
                Specification.AddInclude("SubCategory");
                Specification.AddFilter(e => e.Id == id);
                var objList = await ProductRepo.GetBySpecsAsync(Specification);
                var categories = await CategoryRepo.GetAll();
                ViewBag.ddlCategories = new SelectList(categories, "ID", "Name");

                return PartialView(objList.FirstOrDefault());
            }
            catch (Exception e)
            {
                ILogger.LogError("Error Home\\Edit \n \n Message : " + e.Message);

                return Error();
            }
        }

        //// POST: Home/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product data)
        {
            try
            {
                await ProductRepo.Update(data);
                return Successful("Product Info Updated Successfully");
            }
            catch (Exception e)
            {
                ILogger.LogError("Error Home\\Edit \n \n Message : " + e.Message);

                return Error();
            }
        }

        // GET: Home/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            Specification.AddInclude("MainCategory");
            Specification.AddInclude("SubCategory");
            Specification.AddFilter(e => e.Id == id);
            var objList = await ProductRepo.GetFirstBySpecsAsync(Specification);
            return PartialView(objList);
        }

        // POST: Home/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, Product data)
        {
            try
            {
                if (data.Id != id)
                    return Error();

                // TODO: Add delete logic here
                await ProductRepo.Delete(id);
                return Successful("Product Deleted Successfully");
            }
            catch (Exception e)
            {
                ILogger.LogError("Error @ Home\\Delete \n \n Message : " + e.Message);

                return Error();
            }
        }
    }
}

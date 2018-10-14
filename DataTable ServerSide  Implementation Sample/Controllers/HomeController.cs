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
        protected readonly ILogger<HomeController> ILogger;

        public HomeController(IRepo<Product> _repo, IRepo<Category> _CategoryRepo, ILogger<HomeController> _logger)
        {
            this.ProductRepo = _repo;
            this.CategoryRepo = _CategoryRepo;
            this.ILogger = _logger;
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
                var specs = new BaseSpecification<Product>(i => i.Id == id, e => e.MainCategory, z => z.SubCategory);
                var objList = await ProductRepo.GetFirstBySpecsAsync(specs);
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
                var specs = new BaseSpecification<Product>(e => e.MainCategory,e => e.SubCategory);
                return Ok(await ProductRepo.GetOptionResponseWithSpec(options, specs));
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
                var specs = new BaseSpecification<Product>(i => i.Id == id);
                specs.AddInclude("MainCategory");
                specs.AddInclude("SubCategory");
                var objList = await ProductRepo.GetBySpecsAsync(specs);
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
            var specs = new BaseSpecification<Product>(i => i.Id == id, i => i.MainCategory, e => e.SubCategory);
            var objList = await ProductRepo.GetBySpecsAsync(specs);

            return PartialView();
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

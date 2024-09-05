using Blog.Web.Data;
using Blog.Web.Models.Domain;
using Blog.Web.Models.ViewModels;
using Blog.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Runtime.CompilerServices;

namespace Blog.Web.Controllers
{
    [Authorize(Roles = "Admin")] // only logged users (admin) are able to access
    public class AdminTagsController : Controller
    {
        
        private readonly ITagRepository tagRepository;

        public AdminTagsController(ITagRepository tagRepository)
        {
            this.tagRepository = tagRepository;
        }

        //OR

        
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }


        [HttpPost]
        [ActionName("Add")]
        //Asynchronous method
        public async Task<IActionResult> Add(AddTagRequest addTagRequest)
        {
            ValidateAddTagRequest(addTagRequest); 

            if (ModelState.IsValid==false)
            {
                return View();
            }

            //Mapping the AddTagRequest to tag domain model
            var tag = new Tag
            {
                Name = addTagRequest.Name,
                DisplayName = addTagRequest.DisplayName
            };
            
            await tagRepository.AddAsync(tag);
            return RedirectToAction("List");
        }

        //Read Functionality
        [HttpGet]
        [ActionName("List")]
        public async Task<IActionResult> List()
        {
            //Use DbContext to read the tags
            var tags =await tagRepository.GetAllAsync();  



            return View(tags);
        }

        
        [HttpGet]
        public async  Task<IActionResult> Edit(Guid Id)
        {
            var tag= await tagRepository.GetAsync(Id);
            if (tag != null) {
                var editTagRequest = new EditTagRequest
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    DisplayName = tag.DisplayName
                };
                return View(editTagRequest);

            }

            return View(null);
        }
        
        
        [HttpPost]
        public async  Task<IActionResult> Edit(EditTagRequest editTagRequest)
        {

            var tag = new Tag
            {
                Id = editTagRequest.Id,
                Name = editTagRequest.Name,
                DisplayName = editTagRequest.DisplayName
            };

          var updatedaTag= await tagRepository.UpdateAsync(tag); 
         if (updatedaTag != null)
            {
                //show success notification
            }
          else
            {
                //show error notification
            }


            return RedirectToAction("Edit", new { id=editTagRequest.Id});

        }
       
        
        [HttpPost]
            public async Task<IActionResult> Delete(EditTagRequest editTagRequest)
            {
              var deletedTag=await tagRepository.DeleteAsync(editTagRequest.Id);    
              
             if (deletedTag != null)
            {
                //show success notification
                return RedirectToAction("List");
            }
             else
            {

            }
            //show an error notification
                return RedirectToAction("Edit", new { id = editTagRequest.Id });
            }

        private void ValidateAddTagRequest(AddTagRequest addTagRequest)
        {
            if (addTagRequest.Name is not null && addTagRequest.DisplayName is not null)
            {
                if (addTagRequest.Name == addTagRequest.DisplayName)
                {
                    ModelState.AddModelError("DisplayName", "Name can't be the same as Display Name!");
                }


            }
        }

    }
}

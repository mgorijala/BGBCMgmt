 
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web; 
using System.IO;
using BGBC.Model;
using System.Data;


namespace BGBC.Web.Controllers
{
    public class UploadFileController : Controller
    {
        private IRepository<Tenant, int> _uploadRepository;
        // GET: UploadFile
        public UploadFileController()
        {
            _uploadRepository = new TenantRepository();
        }

        public ActionResult Upload()
        {
            return View();
        }

        [HttpGet]
      public ActionResult uploadfiles(LeaseFile upload)
        {
            return View(upload);
        }
        [HttpPost]
        public ActionResult uploadfiles(HttpPostedFileBase file, Tenant tenant)
        {

            if (file == null)
            {
                ModelState.AddModelError("File", "Please Upload Your file");
            }
            else if (file.ContentLength > 0)
            {
                int MaxContentLength = 1024 * 1024 * 3; //3 MB
                string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", ".pdf", ".txt",".zip" };

                if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                {
                    ModelState.AddModelError("File", "Please file of type: " + string.Join(", ", AllowedFileExtensions));
                }

                else if (file.ContentLength > MaxContentLength)
                {
                    ModelState.AddModelError("File", "Your file is too large, maximum allowed size is: " + MaxContentLength + " MB");
                }
                else
                {

                   // int folder =tenant.TenantID;
                    var path = Server.MapPath("~/Attactment/");
                    var directory = new DirectoryInfo(path);
                    if (directory.Exists == false)
                    {
                        directory.Create();
                    }

                    //string fileName = string.Empty;
                    //string filePath = string.Empty;
                    //Byte[] bytes;
                    //FileStream fs;
                    //BinaryReader br;
                    //fileName = file.FileName;
                    //filePath = (path + System.Guid.NewGuid() + fileName);
                    //file.SaveAs(filePath);

                    //fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    //br = new BinaryReader(fs);
                    //bytes = br.ReadBytes(Convert.ToInt32(fs.Length));

                        ////TO:DO 
                    var fileName = Path.GetFileName(file.FileName);
                    var path1 = Path.Combine(Server.MapPath("~/Attactment/"), fileName);

                    //tenant.FileName = fileName;
                  //  FileInfo fInfo = new FileInfo(filePath);
                  //  String FolderName = fInfo.Directory.Name;
                  //  tenant.Path = "data:image/jpeg;base64," + Convert.ToBase64String(bytes);
  // tenant.Path = path1;
                    _uploadRepository.Add(tenant);
                    //file.SaveAs(path1);
                    //ModelState.Clear();
                        TempData["SucessMessage"] = "File uploaded successfully";
                        return RedirectToAction("uploadfiles", "UploadFile"); 
                }
            }

            return View(tenant);
        }
        //var File = Request.Files[0];

        //if (file != null && file.ContentLength > 0)
        //{
        //    var fileName = Path.GetFileName(file.FileName);
        //    var path = Path.Combine(Server.MapPath("~/Images/"), fileName); file.SaveAs(path);
        //}

        //return RedirectToAction("uploadfiles");

    }
}
 
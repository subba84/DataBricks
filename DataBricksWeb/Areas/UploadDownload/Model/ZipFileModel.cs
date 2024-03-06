using DataBricksWeb.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataBricksWeb.Areas.UploadDownload.Model
{
    public class ZipFileModel
    {
        public List<ZipUpload> ZipUploads { get; set; }
    }
}
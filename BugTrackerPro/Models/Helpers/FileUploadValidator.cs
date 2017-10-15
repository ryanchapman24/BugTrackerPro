using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BugTrackerPro.Models.Helpers
{
    public class FileUploadValidator
    {
        public static bool IsWebFriendlyFile(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return false;
            }

            if (file.ContentLength > 2 * 1024 * 1024 || file.ContentLength < 1024)
            {
                return false;
            }

            var ext = Path.GetExtension(file.FileName).ToLower();
            if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif" || ext == ".bmp" || ext == ".doc" || ext == ".docx" || ext == ".xls" || ext == ".xlsx" || ext == ".csv" || ext == ".txt" || ext == ".rtf" || ext == ".pdf" || ext == ".ppt" || ext == ".pptx" || ext == ".mp3" || ext == ".wma" || ext == ".wav" || ext == ".mp4" || ext == ".mov" || ext == ".avi" || ext == ".wmv")
            {
                return true;
            }

            return false;
        }
    }
}
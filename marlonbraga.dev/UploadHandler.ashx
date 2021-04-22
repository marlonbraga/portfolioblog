<%@ WebHandler Language="C#" Class="UploadHandler" %>

using System;
using System.Web;
using System.IO;

public class UploadHandler : IHttpHandler {
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        context.Response.Expires = -1;
        try
        {
            HttpPostedFile postedFile = context.Request.Files["file"];

            string savepath = "";

            savepath = context.Server.MapPath("Images");
            string filename = postedFile.FileName;
            if (!Directory.Exists(savepath))
                Directory.CreateDirectory(savepath);

            postedFile.SaveAs(savepath + @"\" + filename);


            //your can put your code logic here
            //and return the remote location of the uploaded image as response

            context.Response.Write("Images/" + filename);
            context.Response.StatusCode = 200;
        }
        catch (Exception ex)
        {
            context.Response.Write("Error: " + ex.Message);
        }

    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using System.IO;
//using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using MvcProject.Models;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Text;


namespace MvcProject.Controllers;

public class Action : Controller
{
    SqlConnection con;
    string? dbconnectionstr;

    private readonly IWebHostEnvironment _webHost;
    public Action(IWebHostEnvironment webHost)
    {
        var dbconfig = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json").Build();
        dbconnectionstr = dbconfig["ConnectionStrings:constr"];
        _webHost = webHost;
    }
    
    public bool SendEmail(string receiver)
    {
        bool chk = false;
        try
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("jonnybonesmma007@gmail.com");
            mail.To.Add(receiver);
            mail.IsBodyHtml = true;
            mail.Subject = "OTP";
            mail.Body = "Your OTP is :" + TempData["otp"];
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.Credentials = new NetworkCredential("jonnybonesmma007@gmail.com", "vloj oxvu abgp wrya");
            client.EnableSsl = true;
            client.Send(mail);
            chk = true;
        }
        catch (Exception)
        {

            throw;
        }
        return chk;
    }


    [HttpGet]
    public IActionResult Login()
    {
        ModelState.Clear();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginModel obj)
    {
        try
        {
            using (con = new SqlConnection(dbconnectionstr))
            {
                con.Open();
                if (ModelState.IsValid)
                {
                    SqlCommand cmd = new SqlCommand("sp_login", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@email", obj.email);
                    cmd.Parameters.AddWithValue("@password", obj.password);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        HttpContext.Session.SetString("UserName", dr["Name"].ToString());
                        HttpContext.Session.SetString("LoginTime", System.DateTime.Now.ToShortTimeString());
                        ModelState.Clear();
                        return View("Dashboard");
                    }
                    else
                    {
                        ViewBag.Message = "Wrong Credentials";
                        ModelState.Clear();

                        return View();
                    }
                }

                else
                {
                    ModelState.Clear();
                    return View();
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            con.Close();
        }

    }

    [HttpGet]
    public IActionResult Signup()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Signup(IFormFile img,ActionModel acc)
    { 
        try
        {

            using (MemoryStream ms = new MemoryStream())
            {
                img.CopyTo(ms);
                using (con = new SqlConnection(dbconnectionstr))
                {
                    SqlCommand cmd = new SqlCommand("sp_insert_reg", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@roll", Convert.ToInt32(acc.rollNo));
                    cmd.Parameters.AddWithValue("@_name", acc._name);
                    cmd.Parameters.AddWithValue("@email", acc.email);
                    cmd.Parameters.AddWithValue("@filename", Path.GetFileName(img.FileName));
                    cmd.Parameters.AddWithValue("@contentType", img.ContentType);
                    cmd.Parameters.AddWithValue("@data", ms.ToArray());
                    cmd.Parameters.AddWithValue("@password", acc.password);
                    cmd.Parameters.AddWithValue("@mobile", acc.mobile);
                    cmd.Parameters.AddWithValue("@gender", acc.gender);
                    cmd.Parameters.AddWithValue("@dept", acc.dept);
                    con.Open();
                    int dr = cmd.ExecuteNonQuery();
                    con.Close();
                    if (dr > 0)
                    {
                        ModelState.Clear();
                        return View("Login");
                    }
                    else
                    {

                        return View();
                    }
                }
            }

        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpGet]
    [SetSessionGlobally]
    public IActionResult Forgot()
    {
        return View();
    }

    [HttpPost]
    [SetSessionGlobally]
    public IActionResult Forgot(ForgotModel obj)
    {
        using(con =new SqlConnection(dbconnectionstr))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_forgot",con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@email",obj.email);
            TempData["email"] = (obj.email);
            TempData.Keep();

            SqlDataReader dr = cmd.ExecuteReader();
            if(dr.Read())
            {
                con.Close();
                return RedirectToAction("Otp", "Action");
                
            }
            else
            {
                con.Close();
                ViewBag.Message = "Enter Correct Email";
                ModelState.Clear();
                return View();
            }
            
        }
        
    }


    [SetSessionGlobally]
    public IActionResult Otp()
    {
        Random rand = new Random();
        TempData["otp"] = rand.Next(1111, 9999).ToString();
        TempData.Keep();
        //TempData["timestamp"] = DateTime.Now;
        bool result = SendEmail(Convert.ToString(TempData["email"]));
        if (result == true)
        {
            return View("Otp");
        }

        return View();
    }

    [SetSessionGlobally]
    public IActionResult VerifyOtp(ForgotModel obj)
    {
        int a = Convert.ToInt32(TempData["otp"]);
        //if ((DateTime.Now - Convert.ToDateTime(TempData["timestamp"])).TotalSeconds > 300)
        //{
        //    return BadRequest("OTP Timedout");
        //}
        if (obj.otp == Convert.ToString(a))
        {
            return RedirectToAction("ResetPaasword", "Action");
        }
        else
        {
            
            TempData.Keep();
            ViewBag.Message = "Incorrect OTP";
            ModelState.Clear();
            return View("Otp");
        }
    }

    
    [HttpGet]
    [SetSessionGlobally]
    public IActionResult ResetPaasword()
    {
        return View();
    }

    
    [HttpPost]
    [SetSessionGlobally]
    public IActionResult ResetPaasword(ForgotModel obj)
    {
        using(con = new SqlConnection(dbconnectionstr))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_reset_password",con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@password", obj.password);
            cmd.Parameters.AddWithValue("@email", Convert.ToString(TempData["email"]));
            int dr = cmd.ExecuteNonQuery();
            if(dr > 0)
            {
                con.Close();
                TempData.Remove("email");
                TempData.Remove("otp");
                return RedirectToAction("Login", "Action");
            }
            else
            {
                ViewBag.Message = "Something Wrong Happened";
                return View();
            }
        }
    }

    
    [HttpGet]
    [SetSessionGlobally]
    public IActionResult Dashboard()
    {
        return View();
    }


    [SetSessionGlobally]
    public IActionResult Home()
    {
        List<UpdateModel> obj = getAllData();
           
        return View(obj);
    }
    public List<UpdateModel> getAllData()
    {
        List<UpdateModel> obj = new List<UpdateModel>();
        try
        {
            using (con = new SqlConnection(dbconnectionstr))
            {
                SqlDataAdapter da = new SqlDataAdapter("Select * from signup", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    obj.Add(
                        new UpdateModel
                        {
                            roll_No = Convert.ToInt32(dr["Roll_No"].ToString()),
                            name = dr["Name"].ToString(),
                            email = dr["Email"].ToString(),
                            filename = dr["File_Name"].ToString(),
                            contentType = dr["File_Extension"].ToString(),
                            Data = (byte[])dr["Data"],
                            source = "data:image/png;base64," + Convert.ToBase64String((byte[])dr["Data"], 0, ((byte[])dr["Data"]).Length),
                            password = dr["Password"].ToString(),
                            mobile = dr["Mobile"].ToString(),
                            gender = dr["Gender"].ToString(),
                            dept = dr["Dept"].ToString()
                        }) ;
                }
                return obj;
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    
    [HttpGet]
    [SetSessionGlobally]
    public IActionResult Update(int id)
    {
        RegisterModel obj = getDataID(id);
        return View(obj);
    }


    [HttpPost]
    [SetSessionGlobally]
    [ValidateAntiForgeryToken]
    public IActionResult Update(int id,RegisterModel obj)
    {
        try
        {
            if (ModelState.IsValid)
            {


                using (con = new SqlConnection(dbconnectionstr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_update_reg", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@name", obj.name);
                    cmd.Parameters.AddWithValue("@email", obj.email);
                    cmd.Parameters.AddWithValue("@mobile", obj.mobile);
                    cmd.Parameters.AddWithValue("@gender", obj.gender);
                    cmd.Parameters.AddWithValue("@dept", obj.dept);
                    cmd.Parameters.AddWithValue("@roll", id);
                    int dr = cmd.ExecuteNonQuery();
                    if (dr > 0)
                    {
                        return RedirectToAction("Home", "Action");
                    }
                    else
                    {

                        return View();
                    }
                }

            }
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            con.Close();
        }

        return View(obj);
    }
    public RegisterModel getDataID(int Id)
    {
        RegisterModel obj = null;
        try
        {
            using (con = new SqlConnection(dbconnectionstr))
            {
                SqlCommand cmd = new SqlCommand("sp_getAllData_reg", con);
                cmd.CommandType= CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", Id);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    obj = new RegisterModel()
                    {
                        roll_No = Convert.ToInt32(dt.Rows[i]["Roll_No"].ToString()),
                        name = dt.Rows[i]["Name"].ToString(),
                        email = dt.Rows[i]["Email"].ToString(),
                        mobile = dt.Rows[i]["Mobile"].ToString(),
                        gender = dt.Rows[i]["Gender"].ToString(),
                        dept = dt.Rows[i]["Dept"].ToString()
                    };

                    return obj;
                }

            }

            return obj;
        }


        catch (Exception)
        {

            throw;
        }


    }

    [SetSessionGlobally]
    public IActionResult Delete(int id)
    {
        try
        {
            using (con = new SqlConnection(dbconnectionstr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_delete_reg", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                int dr = cmd.ExecuteNonQuery();
                if (dr > 0)
                {
                    return RedirectToAction("Home", "Action");
                }
                else
                {
                    return View();

                }
            }
           
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            con.Close();
        }
    }

    /*public IActionResult DropDown()
    {
        ViewBag.data = Getinfo();
        return View();
    }

    */
    public List<DropModel> Getinfo()
    {
        List<DropModel> obj = new List<DropModel>();
        using (con = new SqlConnection (dbconnectionstr))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select Roll_No from signup",con);
            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    obj.Add(new DropModel
                    {
                        roll_no = Convert.ToInt32(dr["Roll_No"].ToString())
                    });
                }
            }
            con.Close();
        }
        return obj;
    }

    /*public IActionResult GetDropData()
    {
        List<DropDownDataModel> obj = GetDataFromDropDown(Request.Form["Test"].ToString());
        return View(obj);
    }
    */
    /*public List<DropDownDataModel> GetDataFromDropDown(string? x)
    {
        using (con = new SqlConnection(dbconnectionstr))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select * from signup where Roll_no = @rollno");
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@rollno", Convert.ToInt32(x));
            SqlDataReader dr = cmd.ExecuteReader();
            List<DropDownDataModel> obj = new List<DropDownDataModel>();
            if(dr.HasRows)
            {
                while(dr.Read())
                {
                    obj.Add(new DropDownDataModel
                    {
                        roll_no = Convert.ToInt32(dr["Roll_No"]),
                        name = Convert.ToString(dr["Name"]),
                        email = Convert.ToString(dr["Email"]),
                        password = Convert.ToString(dr["Password"]),
                        mobile = Convert.ToInt64(dr["Mobile"]),
                        gender = Convert.ToString(dr["Gender"]),
                        dept = Convert.ToString(dr["Dept"])

                    }) ;
                }
                
            }
            con.Close();
            return obj;
        }
    }
    */


    [SetSessionGlobally]
    public IActionResult Resume()
    {
        ViewBag.data = Getinfo();
        return View();
    }

    [SetSessionGlobally]
    public IActionResult BrowseResume()
    {
        List<UploadResumeModel> obj = ResumeData(Request.Form["Test1"].ToString());
        return View(obj);
    }

    public List<UploadResumeModel> ResumeData(string? id)
    {
        using (con = new SqlConnection(dbconnectionstr))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_getAllData_reg");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@id", Convert.ToInt32(id));
            SqlDataReader dr = cmd.ExecuteReader();
            List<UploadResumeModel> obj = new List<UploadResumeModel>();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    TempData["var"] = dr["Roll_No"];
                    obj.Add(new UploadResumeModel
                    {
                        roll_no = Convert.ToInt32(dr["Roll_No"]),
                        name = Convert.ToString(dr["Name"]),
                        email = Convert.ToString(dr["Email"]),
                    });
                }

            }
            con.Close();
            return obj;
        }
    }


    [SetSessionGlobally]
    public IActionResult Upload(IFormFile file)
    {
        HolaModel obj = new HolaModel();
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file selected for upload...");
        }
        string fileName = Path.GetFileName(file.FileName);

        string contentType = file.ContentType;

        using (MemoryStream memoryStream = new MemoryStream())
        {
            file.CopyTo(memoryStream);

            using (con = new SqlConnection(dbconnectionstr))
            {
                bool flag = Check(Convert.ToInt32(TempData["var"]));
                obj = ModelInsert(Convert.ToInt32(TempData["var"]));
                if(flag)
                {
                    using (SqlCommand cmd = new SqlCommand("sp_update_resume"))
                    {
                        cmd.Connection = con;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@filename", fileName);
                        cmd.Parameters.AddWithValue("@ContentType", contentType);
                        cmd.Parameters.AddWithValue("@file", memoryStream.ToArray());
                        cmd.Parameters.AddWithValue("@id", obj.roll_no);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                else
                {
                    
                    using (SqlCommand cmd = new SqlCommand("sp_Insert_resume"))
                    {
                        cmd.Connection = con;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@roll", obj.roll_no);
                        cmd.Parameters.AddWithValue("@name", obj.name);
                        cmd.Parameters.AddWithValue("@email", obj.email);
                        cmd.Parameters.AddWithValue("@filename", fileName);
                        cmd.Parameters.AddWithValue("@ContentType", contentType);
                        cmd.Parameters.AddWithValue("@file", memoryStream.ToArray());
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }

                
            }
        }
        return RedirectToAction("ViewResume","Action");
    }

    public bool Check(int id)
    {
        using(SqlCommand cmd = new SqlCommand("Select * from resume where Roll_No = @id",con))
        {
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader(); ;
                
            if(dr.Read())
            {
                con.Close();
                return true;
            }
            else
            {
                con.Close();
                return false;
            }  
        }
    }
    public HolaModel ModelInsert(int  id)
    {
        HolaModel abc = new HolaModel();    
        using (SqlCommand cmd = new SqlCommand("Select Name,Email from signup where Roll_No = @roll", con))
        {
            cmd.Connection = con;
            con.Open();
            cmd.Parameters.AddWithValue("@roll", id);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    abc.roll_no = id;
                    abc.name = dr["Name"].ToString();
                    abc.email = dr["Email"].ToString();
                }
            }
            con.Close();
        }
        return abc;
    }


    [SetSessionGlobally]
    public IActionResult ViewResume()
    {
        List<UploadResumeModel> obj = GetResume();
        return View(obj);
    }
    public List<UploadResumeModel> GetResume()
    {
        using(con = new SqlConnection(dbconnectionstr))
        {
            using (SqlCommand cmd = new SqlCommand("Select * from resume ", con))
            {
                cmd.Connection = con;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                List<UploadResumeModel> obj = new List<UploadResumeModel>();
                if(dr.HasRows)
                {
                    while(dr.Read())
                    {
                        obj.Add(new UploadResumeModel
                        {
                            roll_no = Convert.ToInt32(dr["Roll_No"]),
                            name = dr["Name"].ToString(),
                            email = dr["Email"].ToString(),
                            File = Encoding.ASCII.GetBytes(dr["Data"].ToString()),
                            filename = dr["File_Name"].ToString(),
                        }) ;
                        
                    }
                }
                
                con.Close();
                return obj;
            }
        }
    }


    [SetSessionGlobally]
    public  IActionResult DownloadFile(int id)
    {
        //List<FileModel> files = new List<FileModel>();
        byte[] Data;
        string contentType;
        string Name;
        using (con = new SqlConnection(dbconnectionstr))
        {
            
            using (SqlCommand cmd = new SqlCommand("sp_get_resume"))
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    sdr.Read();
                    //Id = Convert.ToInt32(sdr["Roll_NO"]),
                    //VP
                    Name = sdr["File_Name"].ToString();
                    contentType = sdr["File_Extension"].ToString();
                    Data = (byte[])sdr["Data"];
                       
                    
                }
                con.Close();
            }
        }
        return File(Data,contentType,Name);
    }
}


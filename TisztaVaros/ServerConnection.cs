using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Xml.Linq;
using System.IO;

namespace TisztaVaros
{
    internal class ServerConnection
    {
        private HttpClient client = new HttpClient();
        public static bool login_ok, reg_ok;
        public static string a_token;
        public static int get_db;
        public static TV_User l_user;

        public async Task<List<TV_Report>> Server_Get_AllReports()
        {
            List<TV_Report> all_reports = new List<TV_Report>();
            string a_route = "/api/reports/getAllReports";
            string url = Get_URL() + a_route;
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = new HttpResponseMessage();
                response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseText = await response.Content.ReadAsStringAsync();
                //MessageBox.Show(responseText);
                File.WriteAllText(@"R:\All_reports.txt", responseText);
                all_reports = JsonConvert.DeserializeObject<List<TV_Report>>(responseText);
            }
            catch (Exception e)
            {
                MessageBox.Show(a_route + "\n\n" + e.Message);
            }
            return all_reports;
        }

        public async Task<List<TV_Cats>> Server_Get_Categories()
        {
            List<TV_Cats> all_cats = new List<TV_Cats>();
            string a_route = "/api/categories/list";
            string url = Get_URL() + a_route;
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = new HttpResponseMessage();
                response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseText = await response.Content.ReadAsStringAsync();
                all_cats = JsonConvert.DeserializeObject<List<TV_Cats>>(responseText);
            }
            catch (Exception e)
            {
                MessageBox.Show(a_route + "\n\n" + e.Message);
            }
            return all_cats;
        }
        public async Task<string> Server_AddNewCategory(string name, string def_instId)
        {
            string a_route = "/api/categories/create";
            string url = Get_URL() + a_route;
             var jsonData = new
            {
                categoryName = name,
                defaultInstitutionId=def_instId
            };
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = new HttpResponseMessage();
                string stringifiedJson = JsonConvert.SerializeObject(jsonData);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + a_token);
                StringContent sendThis = new StringContent(stringifiedJson, Encoding.UTF8, "Application/JSON");
                response = await client.PostAsync(url, sendThis);
                response.EnsureSuccessStatusCode();
                string responseText = await response.Content.ReadAsStringAsync();
                TV_Cats new_category = JsonConvert.DeserializeObject<TV_Cats>(responseText);
                return new_category.id;
            }
            catch (Exception e)
            {
                MessageBox.Show(a_route + "\n\n" + e.Message);
            }
            return "22";
        }
        public async Task<string> Server_ModifyCategory(string name, string def_instId)
        {
            string a_route = "/api/categories/modify";
            string url = Get_URL() + a_route;
            var jsonData = new
            {
                categoryName = name,
                defaultInstitutionId = def_instId
            };
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = new HttpResponseMessage();
                string stringifiedJson = JsonConvert.SerializeObject(jsonData);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + a_token);
                StringContent sendThis = new StringContent(stringifiedJson, Encoding.UTF8, "Application/JSON");
                response = await client.PostAsync(url, sendThis);
                response.EnsureSuccessStatusCode();
                string responseText = await response.Content.ReadAsStringAsync();
                TV_Cats mod_category = JsonConvert.DeserializeObject<TV_Cats>(responseText);
                return mod_category.id;
            }
            catch (Exception e)
            {
                MessageBox.Show(a_route + "\n\n" + e.Message);
            }
            return "22";
        }
        public async Task<int> Server_Get_ReportCat_db(string cat_id)
        {
            int a_out = -1;
            string a_route = "/api/reports/report_Cat_db";
            string url = Get_URL() + a_route;
            var jsonData = new
            {
                categoryId = cat_id,
            };
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = new HttpResponseMessage();
                string stringifiedJson = JsonConvert.SerializeObject(jsonData);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + a_token);
                StringContent sendThis = new StringContent(stringifiedJson, Encoding.UTF8, "Application/JSON");
                response = await client.PostAsync(url, sendThis);
                response.EnsureSuccessStatusCode();
                string responseText = await response.Content.ReadAsStringAsync();
                TVS_Found_db a_json = JsonConvert.DeserializeObject<TVS_Found_db>(responseText);
                a_out = a_json.found_db;
            }
            catch (Exception e)
            {
                MessageBox.Show(a_route + "\n\n" + e.Message);
            }
            return a_out;
        }
        public async Task<int> Server_Get_InstValami_db(string a_route, string inst_id)
        {
            int a_out = -1;
            string url = Get_URL() + a_route;
            var jsonData = new
            {
                institutionId = inst_id,
            };
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = new HttpResponseMessage();
                string stringifiedJson = JsonConvert.SerializeObject(jsonData);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + a_token);
                StringContent sendThis = new StringContent(stringifiedJson, Encoding.UTF8, "Application/JSON");
                response = await client.PostAsync(url, sendThis);
                response.EnsureSuccessStatusCode();
                string responseText = await response.Content.ReadAsStringAsync();
                TVS_Found_db a_json = JsonConvert.DeserializeObject<TVS_Found_db>(responseText);
                a_out = a_json.found_db;
            }
            catch (Exception e)
            {
                MessageBox.Show(a_route + "\n\n" + e.Message);
            }
            return a_out;
        }
        public async Task<string>Admin_DelUser(string a_email)
        {
            string a_route = "/api/auth/delete/" + a_email;
            string url = Get_URL() + a_route; 
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + a_token);
                HttpResponseMessage response = await client.DeleteAsync(url);
                response.EnsureSuccessStatusCode();
                string responseInString = await response.Content.ReadAsStringAsync();
                Message del_msg = JsonConvert.DeserializeObject<Message>(responseInString);
                return del_msg.message;    
            }
            catch (Exception e)
            {
                MessageBox.Show(a_route + "\n\n" + e.Message);
            }
            return "xx";
        }
        public async Task<string> Admin_DelCategory(string cat_id)
        {
            string a_route = "/api/categories/delete/" + cat_id;
            string url = Get_URL() + a_route;
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + a_token);
                HttpResponseMessage response = await client.DeleteAsync(url);
                response.EnsureSuccessStatusCode();
                string responseInString = await response.Content.ReadAsStringAsync();
                Message del_msg = JsonConvert.DeserializeObject<Message>(responseInString);
                return del_msg.message;
            }
            catch (Exception e)
            {
                MessageBox.Show(a_route + "\n\n" + e.Message);
            }
            return "xx";
        }
        public async Task<string> Admin_AddNewInst(TV_Inst a_inst)
        {
            string a_route = "/api/institutions/create";
            string url = Get_URL() + a_route;
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = new HttpResponseMessage();
                string stringifiedJson = JsonConvert.SerializeObject(a_inst);
                ;
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + a_token);
                StringContent sendThis = new StringContent(stringifiedJson, Encoding.UTF8, "Application/JSON");
                response = await client.PostAsync(url, sendThis);
                response.EnsureSuccessStatusCode();
                string responseText = await response.Content.ReadAsStringAsync();
                TV_Inst new_inst = JsonConvert.DeserializeObject<TV_Inst>(responseText);
                return new_inst.id;
            }
            catch (Exception e)
            {
                MessageBox.Show(a_route + "\n\n" + e.Message);
            }
            return "22";
        }
        public async Task<string> Admin_AddNewUser(string a_email, string a_name, string a_pws)
        {
            string a_route = "/api/auth/admin/register";
            string url = Get_URL() + a_route;
            var jsonData = new
            {
                username = a_name,
                email = a_email,
                password = a_pws,
                confirmPassword = a_pws
            };
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = new HttpResponseMessage();
                string stringifiedJson = JsonConvert.SerializeObject(jsonData);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + a_token);
                StringContent sendThis = new StringContent(stringifiedJson, Encoding.UTF8, "Application/JSON");
                response = await client.PostAsync(url, sendThis);
                response.EnsureSuccessStatusCode();
                string responseText = await response.Content.ReadAsStringAsync();
                string correctedText = responseText.Replace("userId", "id");
                TV_User new_user = JsonConvert.DeserializeObject<TV_User>(correctedText);
                return new_user.id;
            }
            catch (Exception e)
            {
                MessageBox.Show(a_route + "\n\n" + e.Message);
            }
            return "22";
        }
        public async Task<string> Check_ExistInst(string a_email, string a_name)
        {
            string a_route = "/api/admin/inst_chk";
            string url = Get_URL() + a_route;
            var jsonData = new
            {
                email = a_email == "" ? "*" : a_email,
                name = a_name == "" ? "*" : a_name
            };
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = new HttpResponseMessage();
                string stringifiedJson = JsonConvert.SerializeObject(jsonData);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + a_token);
                StringContent sendThis = new StringContent(stringifiedJson, Encoding.UTF8, "Application/JSON");
                response = await client.PostAsync(url, sendThis);
                string responseText = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                Message found = JsonConvert.DeserializeObject<Message>(responseText);
                return found.message;
            }
            catch (Exception e)
            {
                MessageBox.Show(a_route + "\n\n" + e.Message);
            }
            return "22";
        }
        public async Task<string> Check_ExistUser(string a_email, string a_name)
        {
            string a_route = "/api/admin/user_chk";
            string url = Get_URL() + a_route;
            var jsonData = new
            {
                email = a_email == "" ? "*" : a_email,
                username = a_name == "" ? "*" : a_name
            };
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = new HttpResponseMessage();
                string stringifiedJson = JsonConvert.SerializeObject(jsonData);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + a_token);
                StringContent sendThis = new StringContent(stringifiedJson, Encoding.UTF8, "Application/JSON");
                response = await client.PostAsync(url, sendThis);
                string responseText = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                Message found = JsonConvert.DeserializeObject<Message>(responseText);
                return found.message;
            }
            catch (Exception e)
            {
                MessageBox.Show(a_route + "\n\n" + e.Message);
            }
            return "22";
        }
        public async Task<List<TV_User>> Get_Workers(string inst_id, string honnan)
        {
            List<TV_User> all_workers = new List<TV_User>();
            string a_route = "/api/admin/user_inst/" + inst_id;
            string url = Get_URL() + a_route;
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + a_token);
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseText = await response.Content.ReadAsStringAsync();
                all_workers = JsonConvert.DeserializeObject<List<TV_User>>(responseText);
            }
            catch (Exception e)
            {
                MessageBox.Show(a_route + "\n\n" + e.Message);
            }
            return all_workers;
        }
        public async Task<List<TV_User>> Search_User(string a_name, string a_email)
        {
            List<TV_User> list_user = new List<TV_User>();
            string url = Get_URL() + "/api/admin/user_en";
            try
            {
                var jsonData = new
                {
                    name = a_name,
                    email = a_email
                };
                HttpClient client = new HttpClient();
                string stringifiedJson = JsonConvert.SerializeObject(jsonData);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + a_token);
                StringContent sendThis = new StringContent(stringifiedJson, Encoding.UTF8, "Application/JSON");
                HttpResponseMessage response = await client.PostAsync(url, sendThis);
                string responseText = await response.Content.ReadAsStringAsync();
                //MessageBox.Show(responseText);
                response.EnsureSuccessStatusCode();
                list_user = JsonConvert.DeserializeObject<List<TV_User>>(responseText);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return list_user;
        }
        public async Task<bool> AdminUpdate_Inst(TV_Inst a_inst)
        {
            HttpClient client = new HttpClient();
            string a_route = "/api/institutions/update/" + a_inst.id;
            string url = Get_URL() + a_route;
            try
            {
                string stringifiedJson = JsonConvert.SerializeObject(a_inst);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + a_token);
                StringContent sendThis = new StringContent(stringifiedJson, Encoding.UTF8, "Application/JSON");
                HttpResponseMessage response = await client.PutAsync(url, sendThis);
                string responseText = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(a_route + "\n\n" + e.Message);
                return false;
            }
        }

        public async Task<bool> AdminUpdate_User(TV_User a_user)
        {
            HttpClient client = new HttpClient();
            string a_route = "/api/admin/user_all";
            string url = Get_URL() + a_route;
            try
            {
                string stringifiedJson = JsonConvert.SerializeObject(a_user);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + a_token);
                StringContent sendThis = new StringContent(stringifiedJson, Encoding.UTF8, "Application/JSON");
                HttpResponseMessage response = await client.PutAsync(url, sendThis);
                string responseText = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(a_route + "\n\n" + e.Message);
                return false;
            }
        }
        public async Task<List<TV_Inst>> Get_Institutions()
        {
            List<TV_Inst> all_inst = new List<TV_Inst>();
            string a_route = "/api/institutions";
            string url = Get_URL() + a_route;
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseText = await response.Content.ReadAsStringAsync();
                all_inst = JsonConvert.DeserializeObject<List<TV_Inst>>(responseText);
            }
            catch (Exception e)
            {
                MessageBox.Show(a_route + "\n\n" + e.Message);
            }
            return all_inst;
        }
        public async Task<int> Server_Get_db(string a_route)
        {
            int a_out = -1;
            string url = Get_URL() + a_route;
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + a_token);
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseText = await response.Content.ReadAsStringAsync();
                TVS_Found_db a_json = JsonConvert.DeserializeObject<TVS_Found_db>(responseText);
                a_out = a_json.found_db;
            }
            catch (Exception e)
            {
                MessageBox.Show(a_route + "\n\n" + e.Message);
            }
            return a_out;
        }
        public async Task<TV_User> LoginUser(string a_email, string a_psw)
        {
            HttpClient client = new HttpClient();
            TV_User loggedUser = new();
            Message loginMessage = new Message();
            string responseText="";
            reg_ok = false;
            string url = Get_URL() + "/api/auth/login";
            try
            {
                var jsonData = new
                {
                    email = a_email,
                    password = a_psw
                };
                string stringifiedJson = JsonConvert.SerializeObject(jsonData);
                StringContent sendThis = new StringContent(stringifiedJson, Encoding.UTF8, "Application/JSON");
                HttpResponseMessage response = await client.PostAsync(url, sendThis);                
                responseText = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                TV_Token getToken = JsonConvert.DeserializeObject<TV_Token>(responseText);
                a_token = getToken.token;
                loggedUser = JsonConvert.DeserializeObject<TV_User>(Decode64(a_token.Split('.')[1]));
                login_ok = true;
            }
            catch (Exception e)
            {
                loginMessage = JsonConvert.DeserializeObject<Message>(responseText);
                if (loginMessage == null)
                {
                    loggedUser.message = e.Message;
                }
                else
                {
                    loggedUser.message = loginMessage.message + "\n\n" + e.Message;
                }
            }
            return loggedUser;
        }
        public async Task<Message> RegisterUser(string a_username, string a_email, string[] a_psw)
        {
            Message message = new Message();
            reg_ok = false;
            string url = Get_URL() + "/api/auth/register";
            var jsonData = new
            {
                username = a_username,
                email = a_email,
                password = a_psw[0],
                confirmPassword = a_psw[1]
            };
            string stringifiedJson = JsonConvert.SerializeObject(jsonData);
            StringContent sendThis = new StringContent(stringifiedJson, Encoding.UTF8, "Application/JSON");
            HttpResponseMessage response = await client.PostAsync(url, sendThis);
            string responseText = await response.Content.ReadAsStringAsync();
            message = JsonConvert.DeserializeObject<Message>(responseText);
            reg_ok = (int)response.StatusCode == 201;
            return message;
        }
        private string Get_URL()
        {
            if (App.local_y)
            {
                return "http://localhost:3002";
            }
            return "http://tisztavaros.hu:3000";
        }
        public static string Decode64(string text)
        {
            text = text.Replace('_', '/').Replace('-', '+');
            switch (text.Length % 4)
            {
                case 2:
                    text += "==";
                    break;
                case 3:
                    text += "=";
                    break;
            }
            return Encoding.UTF8.GetString(Convert.FromBase64String(text));
        }
    }
}

using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace InMemoryCaching.Services
{
    public class SessionManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public SessionManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;



        }
        public string CaptchaCode
        {
            get { return System.Text.Encoding.UTF8.GetString(_session.Get("CaptchaCode")); }
            set
            {
                if (value == null)
                    _session.Remove("CaptchaCode");
                else
                    _session.Set("CaptchaCode", System.Text.Encoding.UTF8.GetBytes(value));
            }
        }
        public string CaptchaCodeInShowPhotosAlbum
        {
            get
            {
                return System.Text.Encoding.UTF8.GetString(_session.Get("CaptchaCode1"));
            }
            set
            {
                if (value == null)
                    _session.Remove("CaptchaCode1");
                else
                    _session.Set("CaptchaCode1", System.Text.Encoding.UTF8.GetBytes(value));

            }
        }
        public string CaptchaCodeInPhotosComment
        {
            get
            {
                return System.Text.Encoding.UTF8.GetString(_session.Get("CaptchaCode2"));
            }
            set
            {
                if (value == null)
                    _session.Remove("CaptchaCode2");
                else
                    _session.Set("CaptchaCode2", System.Text.Encoding.UTF8.GetBytes(value));

            }
        }
        
 

    }

 
}

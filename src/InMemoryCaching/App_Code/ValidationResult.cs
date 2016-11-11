using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InMemoryCaching.App_Code
{
      public class ResultBundle
    {
        private List<string> _Messages;
        public bool IsSuccessful { get; set; } = true;
        public string[] Messages => _Messages.ToArray();
        public string FormattedMessages {
            get
            {
                return Messages.Aggregate("", (cur, next) => cur = (cur.Length > 0 ? (cur + "<br/>") : "") + next);
            }
        }
        public Object UserData { get; set; } = null;

        public ResultBundle()
        {
            _Messages = new List<string>();
        }

        public void AddMessage(string message)
        {
            _Messages.Add(message);
        }

        public static ResultBundle Success() { return new ResultBundle() { IsSuccessful = true }; }
        public static ResultBundle Failed() { return new ResultBundle() { IsSuccessful = false}; }
    }
}

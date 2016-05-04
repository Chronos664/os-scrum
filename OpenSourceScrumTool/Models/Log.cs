using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.WebPages;
using Newtonsoft.Json;

namespace OpenSourceScrumTool.Models
{

    [Table("Log")]
    public partial class Log : IModelContent
    {
        [Key]
        public int ID { get; set; }

        public int LogLevel { get; set; }

        [Required]
        public string LogMessage { get; set; }

        public int UserIDForAction { get; set; }

        public DateTime Time { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual User User { get; set; }

        public object ToDTO()
        {
            return new LogDTO()
            {
                LogID = ID,
                LogLevel = LogLevel,
                LogMessage = LogMessage,
                TimeOfLog = Time,
                UserID = UserIDForAction
            };
        }

        public object GetDetails()
        {
            LogDetailsDTO result = new LogDetailsDTO()
            {
                LogID = ID,
                LogLevel = LogLevel,
                LogMessage = LogMessage,
                TimeOfLog = Time,
                UserID = UserIDForAction,
                User = (UserDTO) User.ToDTO()

            };
            return result;
        }

        public void UpdateItem(object item)
        {
            //Item not updated, empty method to implement IModelContent
        }
    }

    public class LogDTO
    {
        public int LogID { get; set; }
        public int LogLevel { get; set; }
        public string LogMessage { get; set; }
        public int UserID { get; set; }
        public DateTime TimeOfLog { get; set; }

    }

    public class LogDetailsDTO : LogDTO
    {
        public UserDTO User { get; set; }
    }

}
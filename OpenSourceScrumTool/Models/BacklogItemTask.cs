using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using OpenSourceScrumTool.DAL;

namespace OpenSourceScrumTool.Models
{
    [Table("Tasks")]
    [Serializable]
    public partial class BacklogItemTask : IModelContent
    {
        [Key]
        public int ID { get; set; }

        public int ProductBacklogID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }

        public int? RemainingTime { get; set; }

        public int? CurrentUserID { get; set; }

        public int State { get; set; }
        public bool Archived { get; set; }

        [IgnoreDataMember]
        [JsonIgnore]
        public virtual ProductBacklogItem ParentProductBacklogItem { get; set; }


        public object ToDTO()
        {
            return new BacklogItemTaskDTO
            {
                BacklogItemTaskID = ID,
                ProductBacklogID = ProductBacklogID,
                Name = Name,
                Description = Description,
                RemainingTime = RemainingTime,
                CurrentUserID = CurrentUserID,
                State = State,
                Archived = Archived
            };
        }

        public object GetDetails()
        {
            BacklogItemTaskDetailsDTO result = new BacklogItemTaskDetailsDTO
            {
                BacklogItemTaskID = ID,
                ProductBacklogID = ProductBacklogID,
                Name = Name,
                Description = Description,
                RemainingTime = RemainingTime,
                CurrentUserID = CurrentUserID,
                State = State,
                Archived = Archived
            };
            if (result.CurrentUserID.HasValue)
            {
                using (DataAccessLayer modelAccess = new DataAccessLayer())
                {
                    result.CurrentUserInfo = (UserDTO)modelAccess.GetUser(result.CurrentUserID.Value).ToDTO();
                }
            }
            else
            {
                result.CurrentUserInfo = null;
            }
            return result;
        }

        public void UpdateItem(object item)
        {
            BacklogItemTask tempTask = (BacklogItemTask) item;
            ProductBacklogID = tempTask.ProductBacklogID;
            Name = tempTask.Name;
            Description = tempTask.Description;
            RemainingTime = tempTask.RemainingTime;
            CurrentUserID = tempTask.CurrentUserID;
            State = tempTask.State;
            Archived = tempTask.Archived;
        }
    }

    public class BacklogItemTaskDTO
    {
        public int BacklogItemTaskID { get; set; }
        public int ProductBacklogID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? RemainingTime { get; set; }
        public int? CurrentUserID { get; set; }
        public int State { get; set; }
        public bool Archived { get; set; }
    }

    public class BacklogItemTaskDetailsDTO : BacklogItemTaskDTO
    {
        public UserDTO CurrentUserInfo { get; set; }
    }
}

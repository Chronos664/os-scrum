using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using OpenSourceScrumTool.Utilities;

namespace OpenSourceScrumTool.Models
{
    [Serializable]
    public partial class ProductBacklogItem : IModelContent
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductBacklogItem()
        {
            Tasks = new HashSet<BacklogItemTask>();
        }
        [Key]
        public int ID { get; set; }

        public int ProjectID { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public byte[] Data { get; set; }

        public int? SprintID { get; set; }

        public int? EffortScore { get; set; }
        public int Priority { get; set; }

        public int State { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? DateFinished { get; set; }

        public virtual Iteration CurrentIteration { get; set; }

        public virtual Project Project { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BacklogItemTask> Tasks { get; set; }
        public bool Archived { get; set; }

        public object ToDTO()
        {
            return new ProductBacklogItemDTO
            {
                ProductBacklogItemID = ID,
                ProjectID = ProjectID,
                Name = Name,
                Description = Description,
                Data = Data,
                SprintID = SprintID,
                EffortScore = EffortScore,
                Priority = Priority,
                State = State,
                Archived = Archived,
                DateFinished = DateFinished.HasValue ? DateFinished.Value.ToString("d") : null
            };
        }

        public object GetDetails()
        {
            ProductBacklogItemDetailsDTO testDto = new ProductBacklogItemDetailsDTO
            {
                ProductBacklogItemID = ID,
                ProjectID = ProjectID,
                Name = Name,
                Data = Data ?? null,
                Description = Description ?? "",
                SprintID = SprintID ?? null,
                EffortScore = EffortScore ?? null,
                Priority = Priority,
                State = State,
                Archived = Archived,
                DateFinished = DateFinished.HasValue ? DateFinished.Value.ToString("d") : null
            };
            if (CurrentIteration != null)
                testDto.CurrentIteration = (IterationDTO)CurrentIteration.ToDTO() ?? null;
            if (Tasks != null)
                testDto.PBITasks = Tasks.GetDetails<BacklogItemTaskDetailsDTO>() ?? null;
            return testDto;
        }

        public void UpdateItem(object item)
        {
            ProductBacklogItem tempPBI = (ProductBacklogItem)item;
            Name = tempPBI.Name;
            Description = tempPBI.Description;
            Data = tempPBI.Data;
            SprintID = tempPBI.SprintID;
            EffortScore = tempPBI.EffortScore;
            Priority = tempPBI.Priority;
            State = tempPBI.State;
            Archived = tempPBI.Archived;
            DateFinished = tempPBI.DateFinished;
        }
    }

    public class ProductBacklogItemDTO
    {
        public int ProductBacklogItemID { get; set; }
        public int ProjectID { get; set; }
        public string Name { get; set; }
        public byte[] Data { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
        public int? SprintID { get; set; }
        public int? EffortScore { get; set; }
        public int Priority { get; set; }
        public int State { get; set; }
        public bool Archived { get; set; }
        public string DateFinished { get; set; }
    }

    public class ProductBacklogItemDetailsDTO : ProductBacklogItemDTO
    {
        public IterationDTO CurrentIteration { get; set; }
        public IEnumerable<BacklogItemTaskDetailsDTO> PBITasks { get; set; }
    }

}

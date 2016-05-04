using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using OpenSourceScrumTool.Utilities;

namespace OpenSourceScrumTool.Models
{
    [Serializable]
    public partial class Iteration : IModelContent
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Iteration()
        {
            ProductBacklogItems = new HashSet<ProductBacklogItem>();
        }
        [Key]
        public int ID { get; set; }

        public int ProjectID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime SprintStartDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime SprintEndDate { get; set; }

        [Required]
        [StringLength(50)]
        public string SprintName { get; set; }
        public bool Archived { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductBacklogItem> ProductBacklogItems { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Project Project { get; set; }

        public object ToDTO()
        {
            return new IterationDTO
            {
                IterationID = ID,
                ProjectID = ProjectID,
                SprintStartDate = SprintStartDate.ToString("d"),
                SprintEndDate = SprintEndDate.ToString("d"),
                SprintName = SprintName,
                Archived = Archived
            };
        }

        public object GetDetails()
        {
            return new IterationDetailsDTO
            {
                IterationID = ID,
                ProjectID = ProjectID,
                SprintStartDate = SprintStartDate.ToString("d"),
                SprintEndDate = SprintEndDate.ToString("d"),
                SprintName = SprintName,
                Archived = Archived,
                productBacklogItems = ProductBacklogItems.GetDetails<ProductBacklogItemDetailsDTO>().ToList()
            };
        }

        public void UpdateItem(object item)
        {
            Iteration tempIteration = (Iteration)item;
            ProjectID = tempIteration.ProjectID;
            SprintStartDate = tempIteration.SprintStartDate;
            SprintEndDate = tempIteration.SprintEndDate;
            SprintName = tempIteration.SprintName;
            Archived = tempIteration.Archived;
        }
    }

    public class IterationDTO
    {
        public int IterationID { get; set; }
        public int ProjectID { get; set; }
        public string SprintStartDate { get; set; }
        public string SprintEndDate { get; set; }
        public string SprintName { get; set; }
        public bool Archived { get; set; }
    }

    public class IterationDetailsDTO : IterationDTO
    {
        public List<ProductBacklogItemDetailsDTO> productBacklogItems { get; set; }
    }
}

using panasonic_machine_checker.data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace panasonic_machine_checker.Models
{

    public class KytMembersModel
    {
        public List<KytMembers>? KytMemberList { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
    public class KytMembers
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Required]
        public int KytId { get; set; }

        [ForeignKey("MemberId")]
        public virtual User Member { get; set; } = null!;
        
        [ForeignKey("KytId")]
        public virtual Kytform Kyt { get; set; } = null!;
    }
}

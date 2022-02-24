using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugRepro
{
    public class MySubEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [ForeignKey(nameof(MyEntityId))]
        public MyEntity MyEntity { get; set; }
        public int MyEntityId { get; set; }
    }
}
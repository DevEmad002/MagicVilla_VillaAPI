using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_VillaAPI.Models
{
    public class VillaNumber
    {
        [Key , DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VillaNo  { get; set; }

        [ForeignKey("Villa")]
        public int VillaID { get; set; } // id for villa
        public Villa Villa { get; set; } // this property is avalible to access class villa 
        public string SpecialDetails { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }


    }
}

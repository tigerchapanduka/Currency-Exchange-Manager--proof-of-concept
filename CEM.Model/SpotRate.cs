using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Nodes;

namespace CEM.Model
{
    [Table("spotrate")]
    public class SpotRate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("idspotrate")]
        public Int64 IdSpotRate { get; set; }

        [Column("disclaimer")]
        public string disclaimer { get; set; }

        [Column("license")]
        public string license { get; set; }

        [Column("timestamp")]
        public long timestamp { get; set; }

        [Column("rates")]
        public Rates rates { get; set; }
    }
}

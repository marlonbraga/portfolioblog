using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace marlonbraga.dev.Models {
	[Table("post")]
	public class Post {
		[Key, Display(Name = "Id")]
		public int IdPost { get; set; }
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
		public DateTime? PublicationDate { get; set; }
		[Required]
		public string Title { get; set; }
		public string TumbNail { get; set; }
		public string Description { get; set; }
		public string Content { get; set; }
		public string Tags { get; set; }
	}
}

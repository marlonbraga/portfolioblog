using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace marlonbraga.dev.Models {
	[Table("postTag")]
	public class PostTag {
		[Required]
		[Key]
		[ForeignKey("Tag")]
		public int IdTag { get; set; }
		public Tag Tag { get; set; }
		[Required]
		[Key]
		[ForeignKey("Post")]
		public int IdPost { get; set; }
		public Post Post { get; set; }
	}
}
